using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Clicker {

	public class WorldUi : MonoBehaviour {

		public UiLifeBar lifeBarPrefab;

		List<TransformPair> pairs = new List<TransformPair>();

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		public UiLifeBar CreateLifeBar(Transform source) {
			UiLifeBar go = GameObject.Instantiate<UiLifeBar>(lifeBarPrefab);
			AddWidgetLink(source, go.transform);
			return go;
		}

		public void AddWidgetLink(Transform source, Transform targetWidget) {
			pairs.Add(new TransformPair(source, targetWidget));
			targetWidget.parent = transform;
			targetWidget.localScale = Vector3.one;
			pairs[pairs.Count - 1].Adjust();
		}

		public void RemoveWidgetLink(Transform source) {
			foreach (var p in pairs) {
				if (p.source == source) {
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