﻿using UnityEngine;
using System.Collections;

namespace Clicker {

	public class BlackSmithRegion : OnceClickRegion {

		public override void Reset(RegionMeta meta, StageController stageController) {
			base.Reset(meta, stageController);
			text.text = "武器升级";
		}

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanUpgradeWeapon()) {
				return;
			}
			DB.PlayerDataHelper.UpgradeWeapon();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}
}