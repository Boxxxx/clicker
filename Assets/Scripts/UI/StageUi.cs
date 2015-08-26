using UnityEngine;
using System.Collections;

namespace Clicker {

	public class StageUi : MonoBehaviour {

		public PlayerStatusUi playerStatusUi;
		public UILabel loseLabel;

		// Use this for initialization
		void Start() {
			playerStatusUi.Refresh();
		}

		// Update is called once per frame
		void Update() {

		}
	}

}