using UnityEngine;
using System.Collections;

namespace Clicker {

	public class ArmorSmithRegion : Region {

		bool isTriggered;
		int regionLayer;

		void Awake() {
			regionLayer = 1 << LayerMask.NameToLayer("Region");
		}

		public override void Reset(RegionMeta meta, StageController stageController) {
			clickArea.gameObject.SetActive(true);
			text.text = "防具升级";
			isTriggered = false;

			this.stageController = stageController;
		}

		public override void RegionUpdate() {
			if (isTriggered) {
				return;
			}
			if (Input.GetMouseButtonDown(0)) {
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 100, regionLayer)) {
					TryUpgradeArmor();
					isTriggered = true;
				}
			}
		}

		void TryUpgradeArmor() {
			if (!DB.PlayerDataHelper.CanUpgradeArmor()) {
				return;
			}
			// TODO(sonicmisora): modify upgrade logic
			DB.PlayerDataHelper.UpgradeArmor();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}
}
