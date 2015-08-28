using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Clicker {

	public class WorldUi : MonoBehaviour {

		List<TransformPair> pairs = new List<TransformPair>();

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		public void CreateLifeBar() {

		}

		public void AddWidgetLink(Transform source, Transform targetWidget) {
			pairs.Add(new TransformPair(source, targetWidget));
			targetWidget.parent = transform;
			pairs[pairs.Count - 1].Adjust();
		}

		public void RemoveWidgetLink(Transform source, Transform targetWidget) {
			foreach (var p in pairs) {
				if (p.source == source && p.target == targetWidget) {
					pairs.Remove(p);
					break;
				}
			}
		}

		public void UpdateAll() {
			foreach (var p in pairs) {
				p.Adjust();
			}
		}
	}

	class TransformPair {
		public Transform source;
		public Transform target;

		public TransformPair(Transform s, Transform t) {
			source = s;
			target = t;
		}

		public void Adjust() {
			target.localPosition = StageHelper.WorldPointToUI(source.position);
		}
	}

}