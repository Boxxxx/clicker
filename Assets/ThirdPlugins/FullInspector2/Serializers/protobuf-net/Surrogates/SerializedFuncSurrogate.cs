/*
 * I don't know of a good way to get support ATM. Sorry!
 *
using FullInspector.Internal;
using FullInspector.Modules.SerializableDelegates;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class BaseSerializationDelegateSurrogate {
        /// <summary>
        /// The container that will be used as a context when invoking the method.
        /// </summary>
        public UnityObject MethodContainer;

        /// <summary>
        /// The name of the method that will be invoked on the container.
        /// </summary>
        public string MethodName;


        // Here is the hacky serialization method that requires globals but actually works

        public static explicit operator BaseSerializationDelegate(BaseSerializationDelegateSurrogate surrogate) {
            return new BaseSerializationDelegate {
                MethodContainer = surrogate.MethodContainer,
                MethodName = surrogate.MethodName
            };
        }

        public static explicit operator BaseSerializationDelegateSurrogate(BaseSerializationDelegate reference) {
            return new BaseSerializationDelegateSurrogate {
                MethodContainer = reference.MethodContainer,
                MethodName = reference.MethodName
            };
        }

    }

    // iOS and WebPlayer have AOT compilers; MakeGenericType doesn't work so we remove it from
    // the compilation process.
#if !UNITY_IOS && !UNITY_WEBPLAYER
    public class BaseSerializerDelegateModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            /*
             * 
             * ... yea ... how do we handle this?
             * 
             * 
            foreach (Type unityObjectType in fiRuntimeReflectionUtility.GetUnityObjectTypes()) {
                if (TypeModelCreator.IsInIgnoredAssembly(unityObjectType)) {
                    continue;
                }

                var surrogateType = typeof(UnityObjectSurrogate<>).MakeGenericType(unityObjectType);
                SetSurrogate(model, unityObjectType, surrogateType);
            }
            */
/*
        }
    }
#endif
}*/