using FullInspector.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public interface IBehaviorEditor {
        void Edit(Rect rect, UnityObject behavior);
        float GetHeight(UnityObject behavior);
        void SceneGUI(UnityObject behavior);
    }

    public abstract class BehaviorEditor<TBehavior> : IBehaviorEditor
        where TBehavior : UnityObject {

        protected abstract void OnEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata);
        protected abstract float OnGetHeight(TBehavior behavior, fiGraphMetadata metadata);
        protected abstract void OnSceneGUI(TBehavior behavior);

        public void SceneGUI(UnityObject behavior) {
            EditorGUI.BeginChangeCheck();

            // we don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field
            OnSceneGUI((TBehavior)behavior);

            // If the GUI has been changed, then we want to reserialize the current object state.
            // However, we don't bother doing this if we're currently in play mode, as the
            // serialized state changes will be lost regardless.
            if (EditorGUI.EndChangeCheck()) {
                // We want to call OnValidate even if we are in play-mode, though.
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                if (EditorApplication.isPlaying == false) {
                    Undo.RecordObject(behavior, "Scene GUI Modification");
                    fiEditorUtility.SetDirty(behavior);

                    var serializedObj = behavior as ISerializedObject;
                    if (serializedObj != null) {
#if UNITY_4_3
                        serializedObj.SaveState();
#endif
                    }
                }
            }
        }

        public void Edit(Rect rect, UnityObject behavior) {
            //-
            //-
            //-
            // Inspector based off of the property editor
            EditorGUI.BeginChangeCheck();

            // Run the editor
            OnEdit(rect, (TBehavior)behavior, fiPersistentMetadata.GetMetadataFor(behavior));

            // If the GUI has been changed, then we want to reserialize the current object state.
            // However, we don't bother doing this if we're currently in play mode, as the
            // serialized state changes will be lost regardless.
            if (EditorGUI.EndChangeCheck()) {
                // We want to call OnValidate even if we are in play-mode, though.
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                if (EditorApplication.isPlaying == false) {
                    Undo.RecordObject(behavior, "Inspector Modification");
                    fiEditorUtility.SetDirty(behavior);

                    var serializedObj = behavior as ISerializedObject;
                    if (serializedObj != null) {
#if UNITY_4_3
                        serializedObj.SaveState();
#endif
                    }
                }
            }
        }

        public float GetHeight(UnityObject behavior) {
            return OnGetHeight((TBehavior)behavior, fiPersistentMetadata.GetMetadataFor(behavior));
        }
    }
}