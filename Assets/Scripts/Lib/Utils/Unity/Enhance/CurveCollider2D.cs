using UnityEngine;
using System.Collections.Generic;

namespace Utils {
    public class CurveCollider2D : MonoBehaviour {
        public AnimationCurve curve;
        public Vector2 offset;
        public float sampleInterval = 0.1f;

        private EdgeCollider2D _edgeCollider = null;
        public EdgeCollider2D EdgeCollider {
            get {
                if (_edgeCollider == null) {
                    _edgeCollider = Unitys.EnsureComponent<EdgeCollider2D>(gameObject);
                }
                return _edgeCollider;
            }
        }

        void Awake() {
            DealApplyToEdgeCollider();
        }

        [ContextMenu("Apply")]
        void DealApplyToEdgeCollider() {
            EdgeCollider.points = GetSamplePoints(curve, offset);
        }

        private static float GetMaxTime(AnimationCurve curve) {
            if (curve.length == 0) {
                return 0;
            }
            return curve[curve.length - 1].time;
        }

        Vector2[] GetSamplePoints(AnimationCurve curve, Vector2 offset) {
            if (curve.length == 0) {
                return new Vector2[] { offset, offset };
            }
            else {
                var ret = new List<Vector2>();
                float maxTime = GetMaxTime(curve);
                for (var t = 0f; t < maxTime; t += sampleInterval) {
                    ret.Add(new Vector2(t, curve.Evaluate(t)) + offset);
                }
                ret.Add(new Vector2(maxTime, curve.Evaluate(maxTime)) + offset);
                return ret.ToArray();
            }
        }
    }
}