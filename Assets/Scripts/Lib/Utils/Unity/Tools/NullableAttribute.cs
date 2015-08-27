using UnityEngine;
using System;
using System.Collections;

namespace Utils {
    [Serializable]
    public class NullableAttribute<T> {
        public bool setValue = false;
        public T value;

        public bool HasValue {
            get { return setValue; }
        }

        public T Value {
            get { return value; }
        }
    }

    [Serializable]
    public class NullableIntAttribute : NullableAttribute<int> { }
    [Serializable]
    public class NullableFloatAttribute : NullableAttribute<float> { }
    [Serializable]
    public class NullableDoubleAttribute : NullableAttribute<string> { }
    [Serializable]
    public class NullableBoolAttribute : NullableAttribute<bool> { }
    [Serializable]
    public class NullalbeStringAttribute : NullableAttribute<string> { }
}
