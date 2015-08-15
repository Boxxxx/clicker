﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Box {

	public class ReusePool : MonoBehaviour {

		public ReusableObject targetPrefab;
		[Tooltip("Use self as root if remaining null.")]
		public GameObject poolRoot;

		public int PoolSize { get { return poolSize; } set { SetPoolSize(value); } }

		List<ReusableObject> list = new List<ReusableObject>();
		int poolSize = 0;

		public void SetPoolSize(int size) {
			if (size < poolSize) {
				for (int i = size; i < poolSize; i++) {
					list[i].Deactive();
				}
			} else if (size < list.Count) {

			} else {
				int addCount = size - list.Count;
				for (int i = 0; i < addCount; i++) {
					list.Add(GameObject.Instantiate(targetPrefab) as ReusableObject);
					list[list.Count - 1].root = this;
					list[list.Count - 1].isUsing = false;
					list[list.Count - 1].Init();
				}
			}
			poolSize = size;
		}

		// Use this for initialization
		void Start() {
			if (poolRoot == null) {
				poolRoot = gameObject;
			}
		}

		public ReusableObject Allocate() {
			for (int i = 0; i < poolSize; i++) {
				if (list[i].isUsing == false) {
					list[i].Active();
					return list[i];
				}
			}
			return null;
		}
	}

}