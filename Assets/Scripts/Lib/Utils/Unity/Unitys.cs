using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    public static class Debugs {
        public static void Log(string log, params object[] args) {
            string output = string.Format(log, args);
            Debug.Log(output);
        }

        public static void LogError(string log, params object[] args) {
            string output = string.Format(log, args);
            Debug.LogError(output);
        }

        public static void LogWarning(string log, params object[] args) {
            string output = string.Format(log, args);
            Debug.LogWarning(output);
        }
    }

    public static class Unitys {
        #region Invoke
        /// <summary>
        /// Invokes a delegate method after a certain time.
        /// </summary>
        public static void Invoke(this MonoBehaviour monoBehaviour, Action cb, float time = 0) {
            if (Maths.IsZero(time)) {
                cb();
            }
            else {
                monoBehaviour.StartCoroutine(_InvokeDelay(cb, time));
            }
        }

        /// <summary>
        /// Invokes a delegate method every specific time, until the cb returns false
        /// </summary>
        /// <param name="cb">task method, return true means continue loop, false means exit</param>
        /// <param name="time">loop time</param>
        public static void InvokeRepeating(this MonoBehaviour monoBehaviour, Func<bool> cb, float time, float repeatRate) {
            monoBehaviour.StartCoroutine(_InvokeLoop(cb, time,repeatRate));
        }

        /// <summary>
        /// Invokes a method in next frame.
        /// </summary>
        public static void NextTick(this MonoBehaviour monoBehaviour, string methodName) {
            monoBehaviour.StartCoroutine(_InvokeNextFrame(() => monoBehaviour.Invoke(methodName, 0)));
        }

        /// <summary>
        /// Invokes a delegate method in next frame
        /// </summary>
        public static void NextTick(this MonoBehaviour monoBehaviour, Action cb) {
            monoBehaviour.StartCoroutine(_InvokeNextFrame(cb));
        }

        /// <summary>
        /// Invokes a method after next physics simulation.
        /// </summary>
        public static void AfterPhysics(this MonoBehaviour monoBehaviour, string methodName) {
            monoBehaviour.StartCoroutine(_InvokeAfterPhysics(() => monoBehaviour.Invoke(methodName, 0)));
        }

        /// <summary>
        /// Invokes a delegate method after next physics simulation.
        /// </summary>
        public static void AfterPhysics(this MonoBehaviour monoBehaviour, Action cb) {
            monoBehaviour.StartCoroutine(_InvokeAfterPhysics(cb));
        }

        private static IEnumerator _InvokeAfterPhysics(Action cb) {
            yield return new WaitForFixedUpdate();
            cb();
        }

        private static IEnumerator _InvokeNextFrame(Action cb) {
            yield return null;
            cb();
        }

        private static IEnumerator _InvokeDelay(Action cb, float time) {
            yield return new WaitForSeconds(time);
            cb();
        }

        private static IEnumerator _InvokeLoop(Func<bool> cb, float time, float repeatRate) {
            yield return new WaitForSeconds(time);
            while(cb()) {
                yield return new WaitForSeconds(repeatRate);
            }
        }
        #endregion

        #region Component
        /// <summary>
        /// Gets component without error, if there is no such component, return null.
        /// </summary>
        public static T GetComponentSafe<T>(this GameObject obj) where T : Component {
            if (obj == null) {
                return null;
            }
            else {
                return obj.GetComponent<T>();
            }
        }

        /// <summary>
        /// Ensures a certain component of obj, if there is no such component, add one.
        /// </summary>
        public static T EnsureComponent<T>(this GameObject obj) where T : Component {
            var comp = obj.GetComponent<T>();
            if (comp != null) {
                return comp;
            }
            return obj.AddComponent<T>();
        }

        /// <summary>
        /// Gets all types of components in this GameObject.
        /// </summary>
        public static Component[] GetCompnents(this GameObject obj) {
            return obj.GetComponents<Component>();
        }

        /// <summary>
        /// Gets all unique type of components in this GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type[] GetComponentTypes(this GameObject obj) {
            HashSet<Type> ret = new HashSet<Type>();
            foreach (var component in GetCompnents(obj)) {
                ret.Add(component.GetType());
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Get specific components under a transform, within a depth.
        ///     depthLimit = -1: any depth, until leaf;
        ///     depthLimit = 0: only the transform itself;
        /// </summary>
        public static T[] GetComponentsWithinDepth<T>(Transform transform, int depthLimit = -1, bool includeInactive = false) where T : Component {
            List<T> ret = new List<T>();
            Queue<KeyValuePair<Transform, int>> queue = new Queue<KeyValuePair<Transform, int>>();
            if (transform != null && (includeInactive || transform.gameObject.activeInHierarchy)) {
                queue.Enqueue(Util.MakePair(transform, 0));
            }

            while (queue.Count > 0) {
                var pair = queue.Dequeue();
                var node = pair.Key;
                var dep = pair.Value;
                ret.AddRange(node.GetComponents<T>());
                if (depthLimit >= 0 && dep + 1 > depthLimit) {
                    continue;
                }
                for (int i = 0; i < node.childCount; i++) {
                    var child = node.GetChild(i);
                    if (includeInactive || child.gameObject.activeInHierarchy) {
                        queue.Enqueue(Util.MakePair(node.GetChild(i), dep + 1));
                    }
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Get all components under a transform, including itself and all children.
        /// </summary>
        public static T[] GetComponentsAll<T>(Transform transform, bool includeInactive = false) where T : Component {
            return GetComponentsWithinDepth<T>(transform, -1, includeInactive);
        }

        /// <summary>
        /// Get the first component under a transform, within a depth, using breath first order.
        ///     depthLimit = -1: any depth, until leaf;
        ///     depthLimit = 0: only the transform itself;
        /// </summary>
        public static T GetComponentWithinDepth<T>(Transform transform, int depthLimit = -1, bool includeInactive = false) where T : Component {
            Queue<KeyValuePair<Transform, int>> queue = new Queue<KeyValuePair<Transform, int>>();
            if (includeInactive || transform.gameObject.activeInHierarchy) {
                queue.Enqueue(Util.MakePair(transform, 0));
            }

            while (queue.Count > 0) {
                var pair = queue.Dequeue();
                var node = pair.Key;
                var dep = pair.Value;
                var component = node.GetComponent<T>();
                if (component != null) {
                    return component;
                }
                if (depthLimit >= 0 && dep > depthLimit) {
                    continue;
                }
                for (int i = 0; i < node.childCount; i++) {
                    var child = node.GetChild(i);
                    if (includeInactive || child.gameObject.activeInHierarchy) {
                        queue.Enqueue(Util.MakePair(node.GetChild(i), dep + 1));
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets specific components upwards from this transform to root.
        /// </summary>
        public static T[] GetComponentsUpwards<T>(Transform transform) where T : Component {
            var ret = new List<T>();
            Transform p = transform;
            do {
                var comp = p.GetComponent<T>();
                if (comp != null) {
                    ret.Add(comp);
                }
                p = p.parent;
            } while (p != null);
            return ret.ToArray();
        }

        /// <summary>
        /// Get the first component with type T upwards from this transform to root.
        /// </summary>
        public static T GetComponentUpwards<T>(Transform transform) where T : Component {
            Transform p = transform;
            do {
                var comp = p.GetComponent<T>();
                if (comp != null) {
                    return comp;
                }
                p = p.parent;
            } while (p != null);
            return null;
        }

        /// <summary>
        /// Copys a component and all its fields to another gameobject.
        /// </summary>
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component {
            return CopyComponent(original, destination) as T;
        }

        /// <summary>
        /// Copys a component and all its fields to another gameobject.
        /// </summary>
        public static Component CopyComponent(Component original, GameObject destination) {
            Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }
        #endregion

        #region Transform
        /// <summary>
        /// Flips the x axis of a transform.
        /// </summary>
        /// <param name="transform"></param>
        public static void Flip(this Transform transform) {
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        /// <summary>
        /// Drops all children under a transform, and set them to parent.
        /// </summary>
        /// <param name="transform"></param>
        public static void DropChildren(this Transform transform, Transform destination = null) {
            if (destination == null) {
                destination = transform.parent;
            }
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                child.parent = destination;
            }
        }

        /// <summary>
        /// Destroys all children under a transform.
        /// </summary>
        public static void DestroyChildren(this Transform transform, bool immediate = false) {
            for (int i = 0; i < transform.childCount; i++) {
                if (immediate) {
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
                }
                else {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// Gets ancestors from a transform to root, the order is root first.
        /// </summary>
        public static Transform[] GetAncestors(this Transform transform) {
            List<Transform> ret = new List<Transform>();

            while (transform != null) {
                ret.Add(transform);
                transform = transform.parent;
            }

            ret.Reverse();
            return ret.ToArray();
        }

        /// <summary>
        /// Gets children of a transform.
        /// </summary>
        public static Transform[] GetChildren(this Transform transform) {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++) {
                children.Add(transform.GetChild(i));
            }
            return children.ToArray();
        }

        /// <summary>
        /// Gets the string representation of ancestors of a transform.
        ///     the formation is: obj1/obj2/obj3/this transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static string GetAncestorPath(this Transform transform) {
            string path = "";
            foreach (var trans in GetAncestors(transform)) {
                path += "/" + trans.name;
            }
            return path;
        }
        #endregion

        #region GameObject
        public static T Instantiate<T>(T obj, Transform parent = null) where T : Component {
            return Instantiate(obj.gameObject, parent).GetComponent<T>();
        }
        public static T Instantiate<T>(T obj, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component {
            return Instantiate(obj.gameObject, position, rotation, parent).GetComponent<T>();
        }
        public static GameObject Instantiate(GameObject obj, Transform parent = null) {
            var newObj = UnityEngine.Object.Instantiate(obj) as GameObject;
            if (parent != null) {
                newObj.transform.parent = parent;
            }
            return newObj;
        }
        public static GameObject Instantiate(this GameObject obj, Vector3 position, Quaternion rotation, Transform parent = null) {
            var newObj = UnityEngine.Object.Instantiate(obj) as GameObject;
            if (parent != null) {
                newObj.transform.parent = parent;
            }
            newObj.transform.localPosition = position;
            newObj.transform.localRotation = rotation;
            return newObj;
        }

        public static GameObject CreateGameObject(string name, GameObject parent, Vector3 localPosition, params Type[] components) {
            var obj = new GameObject(name, components);
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = localPosition;
            return obj;
        }
        public static GameObject CreateGameObject(string name, GameObject parent, params Type[] components) {
            return CreateGameObject(name, parent, Vector3.zero, components);
        }
        public static GameObject EnsureGameObject(string name, GameObject parent) {
            var obj = parent.transform.FindChild(name);
            if (obj == null) {
                return CreateGameObject(name, parent);
            }
            else {
                return obj.gameObject;
            }
        }

        public static GameObject[] FindGameObjectsWithComponent<T>() where T : Component {
            HashSet<GameObject> ret = new HashSet<GameObject>();
            foreach (var comp in GameObject.FindObjectsOfType<T>()) {
                ret.Add(comp.gameObject);
            }
            return ret.ToArray();
        }
        public static GameObject[] FindGameObjectsWithLayer(int layer) {
            var ret = new List<GameObject>();
            foreach (var obj in UnityEngine.Object.FindObjectsOfType<GameObject>()) {
                if (obj.layer == layer) {
                    ret.Add(obj);
                }
            }
            return ret.ToArray();
        }
        public static GameObject[] FindGameObjectsWithLayer(this GameObject root, int layer, bool includeInactive = false) {
            var ret = new List<GameObject>();
            var queue = new Queue<GameObject>();
            queue.Enqueue(root);

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                if (node.layer == layer) {
                    ret.Add(node);
                }
                for (int i = 0; i < node.transform.childCount; i++) {
                    var child = node.transform.GetChild(i);
                    if (includeInactive || child.gameObject.activeSelf) {
                        queue.Enqueue(child.gameObject);
                    }
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Get all gameObjects under a gameObject, within a depthLimit.
        /// </summary>
        public static GameObject[] FindGameObjectsWithinDepth(this GameObject root, int depthLimit = 0, bool includeInactive = false) {
            List<GameObject> ret = new List<GameObject>();
            Queue<KeyValuePair<Transform, int>> queue = new Queue<KeyValuePair<Transform, int>>();
            queue.Enqueue(Util.MakePair(root.transform, 0));
            while (queue.Count > 0) {
                var pair = queue.Dequeue();
                var transform = pair.Key;
                var dep = pair.Value;
                ret.Add(transform.gameObject);
                if (depthLimit >= 0 && dep + 1 > depthLimit) {
                    continue;
                }
                for (int i = 0; i < transform.childCount; i++) {
                    var child = transform.GetChild(i);
                    if (includeInactive || transform.gameObject.activeInHierarchy) {
                        queue.Enqueue(Util.MakePair(transform.GetChild(i), dep + 1));
                    }
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Set child & Set parent methods, there are two kind of methods below.
        ///     with localPosition: directly set localPosition after set;
        ///     with keepLocalTransform: if it's true, the local attributes after new parent will be the same as the old parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="localPosition"></param>
        public static void SetChild(this GameObject parent, GameObject child, Vector3 localPosition) {
            child.transform.parent = parent.transform;
            child.transform.localPosition = localPosition;
        }
        public static void SetChild(this GameObject parent, Component child, Vector3 localPosition) {
            SetChild(parent, child.gameObject, localPosition);
        }
        public static void SetChild(this GameObject parent, GameObject child, bool keepLocalTransform = false) {
            if (keepLocalTransform) {
                var pos = child.transform.localPosition;
                var rotation = child.transform.localRotation;
                var scale = child.transform.localScale;

                child.transform.parent = parent.transform;

                child.transform.localPosition = pos;
                child.transform.localRotation = rotation;
                child.transform.localScale = scale;
            }
            else {
                child.transform.parent = parent.transform;
            }
        }
        public static void SetChild(this GameObject parent, Component child, bool keepLocalTransform = false) {
            SetChild(parent, child.gameObject, keepLocalTransform);
        }
        public static void SetParent(this GameObject child, GameObject parent, Vector3 localPosition) {
            parent.SetChild(child, localPosition);
        }
        public static void SetParnt(this GameObject child, Component parent, Vector3 localPosition) {
            parent.gameObject.SetChild(child, localPosition);
        }
        public static void SetParent(this GameObject child, GameObject parent, bool keepLocalTransform = false) {
            parent.SetChild(child, keepLocalTransform);
        }
        public static void SetParent(this GameObject child, Component parent, bool keepLocalTransform = false) {
            parent.gameObject.SetChild(child, keepLocalTransform);
        }
        #endregion

        #region Message
        /// <summary>
        /// Just like MonoBehaviour.SendMessage, but with arguments.
        /// </summary>
        public static void SendMessage(this GameObject obj, string method, SendMessageOptions options, params object[] args) {
            bool received = false;
            if (obj != null && obj.activeInHierarchy) {
                foreach (var component in obj.GetComponents<MonoBehaviour>()) {
                    var _method = component.GetType().GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (_method != null) {
                        try {
                            _method.Invoke(component, args);
                            received = true;
                        }
                        catch (System.Exception ex) {
                            Debug.LogError(ex.Message);
                        }
                    }
                }
            }
            if (!received && options == SendMessageOptions.RequireReceiver)
                Debug.LogWarning(method + " has no receiver!");
        }

        public static void SendMessage(this MonoBehaviour mono, string method, SendMessageOptions options, params object[] args) {
            SendMessage(mono.gameObject, method, options, args);
        }

        /// <summary>
        /// Just like MonoBehaviour.SendMessage, but with argument and will invoke no matter the object is actived or not.
        /// </summary>
        public static void SendMessageWhenever(this GameObject obj, string method, SendMessageOptions options, params object[] args) {
            bool received = false;
            if (obj != null) {
                foreach (var component in obj.GetComponents<MonoBehaviour>()) {
                    var _method = component.GetType().GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (_method != null) {
                        try {
                            _method.Invoke(component, args);
                            received = true;
                        }
                        catch {
                        }
                    }
                }
            }
            if (!received && options == SendMessageOptions.RequireReceiver)
                Debug.LogWarning(method + " has no receiver!");
        }

        public static void SendMessageWhenever(this MonoBehaviour mono, string method, SendMessageOptions options, params object[] args) {
            SendMessageWhenever(mono.gameObject, method, options, args);
        }
        #endregion

        #region Common
        public static void ClearConsole() {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
        #endregion

        #region Debug
        public static void DrawRect(Rect rect, Color color, float duration = 1f) {
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), color, duration);
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, duration);
            Debug.DrawLine(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.yMin), color, duration);
            Debug.DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMin, rect.yMin), color, duration);
        }

        public static void DrawGizmosIcon(Vector2 center, string name) {
            Gizmos.DrawIcon(center, name);
        }
        #endregion
    }
}
