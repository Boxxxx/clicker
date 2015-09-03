using System.Linq;
using FullInspector.Serializers.FullSerializer;
using FullSerializer;
using System.Reflection;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Implements Full Inspector integration with Full Serializer, a .NET serializer that just
    /// works. Use Unity style annotations (such as [SerializeField]) to serialize your types.
    /// </summary>
    public class FullSerializerSerializer : BaseSerializer {
        private static readonly fsSerializer _serializer;

        /// <summary>
        /// Register the given converter for usage in the serializer.
        /// </summary>
        public static void AddConverter(fsConverter converter) {
            _serializer.AddConverter(converter);
        }

        /// <summary>
        /// Register the given processor for usage in the serializer.
        /// </summary>
        public static void AddProcessor(fsObjectProcessor processor) {
            _serializer.AddProcessor(processor);
        }

        static FullSerializerSerializer() {
            _serializer = new fsSerializer();
            _serializer.AddConverter(new UnityObjectConverter());
#if !UNITY_4_3
            _serializer.AddProcessor(new SerializationCallbackReceiverObjectProcessor());
#endif
        }

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            _serializer.Context.Set(serializationOperator);

            fsData data;
            var fail = _serializer.TrySerialize(GetStorageType(storageType), value, out data);
            if (EmitFailWarning(fail)) return null;

            return fsJsonPrinter.CompressedJson(data);
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            fsData data;
            var result = fsJsonParser.Parse(serializedState, out data);
            if (EmitFailWarning(result)) return null;

            _serializer.Context.Set(serializationOperator);

            object deserialized = null;
            result = _serializer.TryDeserialize(data, GetStorageType(storageType), ref deserialized);
            if (EmitFailWarning(result)) return null;

            return deserialized;
        }

        private static bool EmitFailWarning(fsResult result) {
            if (fiSettings.EmitWarnings && result.RawMessages.Any()) {
                Debug.LogWarning(result.FormattedMessages);
            }

            return result.Failed;
        }
    }
}