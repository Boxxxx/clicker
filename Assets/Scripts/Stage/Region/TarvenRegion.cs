using UnityEngine;
using System.Collections;
using System;

namespace Clicker {

	public class TarvenRegion : OnceClickRegion {

		public override void Reset(RegionMeta meta, StageController stageController) {
			base.Reset(meta, stageController);
			text.text = "回复区域";
		}

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanRestoreLifeSpan()) {
				return;
			}
			DB.PlayerDataHelper.RestoreLifeSpan();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}

}