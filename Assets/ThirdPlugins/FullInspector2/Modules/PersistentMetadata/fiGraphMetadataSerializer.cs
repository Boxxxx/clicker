﻿using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public class fiGraphMetadataSerializer<TPersistentData> : ISerializationCallbackReceiver, fiIGraphMetadataStorage
       where TPersistentData : IGraphMetadataItemPersistent {

        [SerializeField]
        private string[] _keys;
        [SerializeField]
        private TPersistentData[] _values;
        [SerializeField]
        private UnityObject _target;

        public void RestoreData(UnityObject target) {
            _target = target;
            if (_keys != null && _values != null) {
                fiPersistentMetadata.GetMetadataFor(_target).Deserialize(_keys, _values);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (_target == null) return;

            var metadata = fiPersistentMetadata.GetMetadataFor(_target);
            if (metadata.ShouldSerialize()) {
                metadata.Serialize(out _keys, out _values);
            }
        }
    }

}