using UnityEngine;
using System.Collections;

namespace Clicker {

	public class ArmorSmithRegion : OnceClickRegion {

		public override void Reset(RegionMeta meta, StageController stageController) {
			base.Reset(meta, stageController);
			text.text = "防具升级";
		}

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanUpgradeArmor()) {
				return;
			}
			DB.PlayerDataHelper.UpgradeArmor();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}
}
