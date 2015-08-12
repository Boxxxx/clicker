using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Box {

	public class ReusePool : MonoBehaviour {

		public GameObject targetPrefab;
		[Tooltip("Use self as root if remaining null.")]
		public GameObject poolRoot;

		public int PoolSize { get { return poolSize; } set { SetPoolSize(value); } }

		List<ReusePool> list = new List<ReusePool>();
		int poolSize;

		public void SetPoolSize(int size) {
			if (size <= poolSize) {
				
				poolSize = size;
			}
			poolSize = size;

		}

		// Use this for initialization
		void Start() {
			if (poolRoot == null) {
				poolRoot = gameObject;
			}
		}

		// Update is called once per frame
		void Update() {

		}
	}

}