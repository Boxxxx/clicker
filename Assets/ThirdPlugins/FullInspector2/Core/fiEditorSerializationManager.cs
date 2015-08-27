using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// If a type extends this interface, then it will always be serialized when Unity sends a
    /// serialization request.
    /// </summary>
    public interface ISerializationAlwaysDirtyTag { }

    /// <summary>
    /// Manages the serialization of ISerializedObject instances. Unity provides a nice
    /// serialization callback, ISerializationCallbackReceiver, however, it often executes the
    /// callback methods on auxiliary threads. Some of the serializers have issues with this
    /// because they invoke unityObj == null, which is only available on the primary thread. To
    /// deal with this, this class defers deserialization so that it always executes on the
    /// primary thread.
    /// </summary>
    public static class fiEditorSerializationManager {
        // !! note !!
        // RunDeserializations is registered to EditorApplicatoin.update on application
        // initialization in fiEditorSerializationManagerEditorInjector

        /// <summary>
        /// Should serialization be disabled? This is used by the serialization migration system
        /// where after migration serialization should not happen automatically.
        /// </summary>
        [NonSerialized]
        public static bool DisableAutomaticSerialization;

        /// <summary>
        /// Items to deserialize. We use a queue because deserialization of one item may
        /// technically cause another item to be added to the queue.
        /// </summary>
        private static readonly Queue<ISerializedObject> _toDeserialize = new Queue<ISerializedObject>();

        /// <summary>
        /// Items that have been modified and should be saved again. Unity will happily send
        /// lots and lots of serialization requests, but most of them are unnecessary.
        /// </summary>
        // TODO: see if this is actually necessary since we just always reserialize all of the deserialized items
        private static readonly HashSet<ISerializedObject> _dirty = new HashSet<ISerializedObject>();

        /// <summary>
        /// Used internally to notify us that an object has been serialized. We update some metadata stored on the object
        /// upon each serialization so we can detect when prefab modifications are applied.
        /// </summary>
        public static void MarkSerialized(ISerializedObject obj) {
            if (fiUtility.IsEditor) {
                _deserializedObjects[obj] = new SerializedObjectSnapshot(obj);
            }
        }

        private static readonly Dictionary<ISerializedObject, SerializedObjectSnapshot> _deserializedObjects = new Dictionary<ISerializedObject, SerializedObjectSnapshot>();
        private static void DoSerialize(ISerializedObject obj) {
            SerializedObjectSnapshot originalData = null;
            _deserializedObjects.TryGetValue(obj, out originalData);

            var storedData = new SerializedObjectSnapshot(obj);

            if (originalData == null || originalData != storedData) {
                // Unity has gone behind our backs and changed the data stored inside of the serialized
                // object. This is likely due to prefab changes. We will simply deserialize the new data
                // so that we don't overwrite it.
                storedData.RestoreSnapshot(obj);
                _deserializedObjects[obj] = new SerializedObjectSnapshot(obj);
            }
            else {
                // Otherwise, serialize the existing object structure and update our deserialized state.
                obj.SaveState();
                _deserializedObjects[obj] = new SerializedObjectSnapshot(obj);
            }
        }

        private static void CheckForReset(ISerializedObject obj) {
            SerializedObjectSnapshot originalData = null;
            if (_deserializedObjects.TryGetValue(obj, out originalData)) {

                if (originalData.IsEmpty) return;

                if (obj.SerializedStateKeys == null || obj.SerializedStateKeys.Count == 0 ||
                    obj.SerializedStateValues == null || obj.SerializedStateValues.Count == 0) {

                    // Note: we do not clear out the keys; if we did, then we would not actually deserialize "null" onto them
                    // Note: we call SaveState() so we can fetch the keys we need to deserialize
                    obj.SaveState();
                    obj.SerializedStateValues.Clear();
                    for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                        obj.SerializedStateValues.Add(null);
                    }
                    obj.RestoreState();

                    fiRuntimeReflectionUtility.InvokeMethod(obj.GetType(), "Reset", obj, null);

                    obj.SaveState();
                    _deserializedObjects[obj] = new SerializedObjectSnapshot(obj);
                }
            }
        }

        /// <summary>
        /// Stores the state of a serialized object.
        /// </summary>
        private class SerializedObjectSnapshot {
            private readonly List<string> _keys;
            private readonly List<string> _values;
            private readonly List<UnityObject> _objectReferences;

            public SerializedObjectSnapshot(ISerializedObject obj) {
                _keys = new List<string>(obj.SerializedStateKeys);
                _values = new List<string>(obj.SerializedStateValues);
                _objectReferences = new List<UnityObject>(obj.SerializedObjectReferences);
            }

            public void RestoreSnapshot(ISerializedObject target) {
                target.SerializedStateKeys = new List<string>(_keys);
                target.SerializedStateValues = new List<string>(_values);
                target.SerializedObjectReferences = new List<UnityObject>(_objectReferences);
                target.RestoreState();
            }

            public bool IsEmpty {
                get {
                    return
                        _keys.Count == 0 ||
                        _values.Count == 0;
                }
            }

            public override bool Equals(object obj) {
                var snapshot = obj as SerializedObjectSnapshot;

                if (ReferenceEquals(snapshot, null)) return false;

                return
                    AreEqual(_keys, snapshot._keys) &&
                    AreEqual(_values, snapshot._values) &&
                    AreEqual(_objectReferences, snapshot._objectReferences);
            }

            public override int GetHashCode() {
                int hash = 13;
                hash = (hash * 7) + _keys.GetHashCode();
                hash = (hash * 7) + _values.GetHashCode();
                hash = (hash * 7) + _objectReferences.GetHashCode();
                return hash;
            }

            public static bool operator ==(SerializedObjectSnapshot a, SerializedObjectSnapshot b) {
                return Equals(a, b);
            }

            public static bool operator !=(SerializedObjectSnapshot a, SerializedObjectSnapshot b) {
                return Equals(a, b) == false;
            }

            private static bool AreEqual<T>(List<T> a, List<T> b) {
                if (a.Count != b.Count) return false;
                for (int i = 0; i < a.Count; ++i) {
                    if (EqualityComparer<T>.Default.Equals(a[i], b[i]) == false) {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Attempts to serialize the given object. Serialization will only occur if the object is
        /// dirty. After being serialized, the object is no longer dirty.
        /// </summary>
        public static void SubmitSerializeRequest(ISerializedObject obj) {
            lock (typeof(fiEditorSerializationManager)) {
                bool isDirty = _dirty.Contains(obj);

                // Serialization is disabled
                if (DisableAutomaticSerialization) {
                    return;
                }

                // The given object is the current inspected object and we havne't marked it as
                // dirty. There is no need to serialize it. This results in a large perf gain, as
                // Unity will send serialize requests continously to the inspected object.
                if (!isDirty && fiLateBindings.Selection.activeObject == GetGameObjectOrScriptableObjectFrom(obj)) {
                    return;
                }

                // The object is dirty or we have deserialized it. A serialize request has been submitted
                // so we should actually service it.
                if (isDirty || _deserializedObjects.ContainsKey(obj) || obj is ISerializationAlwaysDirtyTag) {
                    DoSerialize(obj);
                    _dirty.Remove(obj);
                }
            }
        }

        /// <summary>
        /// Fetches the associated GameObject/ScriptableObject from the given serialized object.
        /// </summary>
        private static UnityObject GetGameObjectOrScriptableObjectFrom(ISerializedObject obj) {
            if (obj is MonoBehaviour) {
                return ((MonoBehaviour)obj).gameObject;
            }
            return (UnityObject)obj;
        }

        /// <summary>
        /// Attempt to deserialize the given object. The deserialization will occur on the next
        /// call to RunDeserializations(). This does nothing if we are not an editor.
        /// </summary>
        public static void SubmitDeserializeRequest(ISerializedObject obj) {
            lock (typeof(fiEditorSerializationManager)) {
                if (fiUtility.IsEditor == false) return;

                _toDeserialize.Enqueue(obj);

                /* NOTE: Disabled for now, possibly useful in the future.
                // If we're on the main thread, then we should try to do some deserializations.
                if (fiUtility.IsMainThread) {
                    RunDeserializations();
                }
                */
            }
        }

        public static void RunDeserializations() {
            // We never run deserializations outside of the editor
            if (fiUtility.IsEditor == false) {
                return;
            }

            // Serialization is disabled
            if (DisableAutomaticSerialization) {
                return;
            }

            // Do not deserialize in the middle of a level load that might be running on another thread
            // (asynchronous) which can lead to a race condition causing the following assert:
            // ms_IDToPointer->find (obj->GetInstanceID ()) == ms_IDToPointer->end ()
            //
            // Very strange that the load is happening on another thread since RunDeserializations only
            // gets invoked from EditorApplication.update and EditorWindow.OnGUI.
            //
            // This method will get called again at a later point so there is no worries that we haven't
            // finished doing the deserializations.
            if (fiLateBindings.EditorApplication.isPlaying && Application.isLoadingLevel) {
                return;
            }

            while (_toDeserialize.Count > 0) {
                ISerializedObject item = _toDeserialize.Dequeue();

                // If we're in play-mode, then we don't want to deserialize anything as that can wipe
                // user-data. We cannot do this in SubmitDeserializeRequest because
                // EditorApplication.isPlaying can only be called from the main thread. However,
                // we *do* want to restore prefabs and general disk-based resources which will not have
                // Awake called.
                if (fiLateBindings.EditorApplication.isPlaying) {
                    // note: We do a null check against unityObj to also check for destroyed objects,
                    //       which we don't need to bother restoring. Doing a null check against an
                    //       ISerializedObject instance will *not* call the correct == method, so
                    //       we need to be explicit about calling it against UnityObject.
                    var unityObj = item as UnityObject;

                    if (unityObj == null ||
                        fiLateBindings.PrefabUtility.IsPrefab(unityObj) == false)
                        continue;
                }

                CheckForReset(item);

                item.RestoreState();
                _deserializedObjects[item] = new SerializedObjectSnapshot(item);
            }
        }

        public static void SetDirty(ISerializedObject obj) {
            _dirty.Add(obj);
        }
    }
}