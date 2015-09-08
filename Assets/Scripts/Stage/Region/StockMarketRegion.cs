using UnityEngine;
using System.Collections;
using System;

namespace Clicker {

	public class StockMarketRegion : OnceClickRegion {

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanBuyStock()) {
				return;
			}
			DB.PlayerDataHelper.BuyStock();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}

}