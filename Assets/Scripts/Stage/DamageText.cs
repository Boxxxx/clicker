using UnityEngine;
using System.Collections;

namespace Clicker {

	public class DamageText : MonoBehaviour {

		public float floatSpeed = 1.0f;
		public float floatDuration = 1.0f;

		// Use this for initialization
		void Start() {
			BeginFloat();
		}

		void BeginFloat() {
			var anime = UIAnimation.TweenPosition(gameObject,
				floatDuration,
				transform.localPosition,
				transform.localPosition + new Vector3(0, floatSpeed * floatDuration, 0));
			UIAnimator.Begin(gameObject, anime, () => { Destroy(gameObject); });
		}

		// Update is called once per frame
		void Update() {

		}

	}

}