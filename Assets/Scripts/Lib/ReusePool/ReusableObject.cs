using UnityEngine;
using System.Collections;

namespace Box {

	public class ReusableObject : MonoBehaviour {

		[HideInInspector]
		public ReusePool root;
		[HideInInspector]
		public bool isUsing = false;

		public virtual void Init() {

		}

		public virtual void Active() {
			isUsing = true;
			gameObject.SetActive(true);
		}

		public virtual void Deactive() {
			isUsing = false;
			gameObject.SetActive(false);
		}

	}

}