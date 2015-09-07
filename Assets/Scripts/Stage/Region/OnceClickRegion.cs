using UnityEngine;
using System.Collections;

namespace Clicker {

	public abstract class OnceClickRegion : Region {

		protected bool isTriggered;
		protected int regionLayer {
			get {
				return 1 << LayerMask.NameToLayer("Region");
			}
		}

		public override void Reset(RegionMeta meta, StageController stageController) {
			clickArea.gameObject.SetActive(true);
			isTriggered = false;

			this.stageController = stageController;
		}

		public override void RegionUpdate() {
			if (isTriggered) {
				return;
			}
			if (Input.GetMouseButtonDown(0)) {
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 100, regionLayer)) {
					OnClick();
					isTriggered = true;
				}
			}
		}

		protected abstract void OnClick();
	}

}