using UnityEngine;
using System.Collections;

namespace Clicker {

	public class BlackSmithRegion : Region {

		bool isTriggered;
		int regionLayer;

		void Awake() {
			regionLayer = 1 << LayerMask.NameToLayer("Region");
		}

		public override void Reset(RegionMeta meta, StageController stageController) {
			clickArea.gameObject.SetActive(true);
			clickArea.transform.localPosition = new Vector3(GameConsts.ScreenWidth, 0, 0);
			clickArea.size = new Vector3(GameConsts.ScreenWidth, GameConsts.ScreenHeight, 0);
			text.text = "武器升级";
			isTriggered = false;

			this.stageController = stageController;
		}

		public override void RegionUpdate() {
			if (Input.GetMouseButtonDown(0)) {
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 100, regionLayer)) {
					TryUpgradeWeapon();
                }
			}
		}

		void TryUpgradeWeapon() {
			if (!DB.PlayerDataHelper.CanUpgradeWeapon()) {
				return;
			}
			DB.PlayerDataHelper.UpgradeWeapon();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}
}