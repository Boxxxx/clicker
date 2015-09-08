using UnityEngine;
using System.Collections;
using System;

namespace Clicker {

	public class DivineRelicRegion : OnceClickRegion {

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanBuyDivineReaper()) {
				return;
			}
			DB.PlayerDataHelper.BuyDivineReaper();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}

}