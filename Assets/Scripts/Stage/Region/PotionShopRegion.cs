using UnityEngine;
using System.Collections;

namespace Clicker {

	public class PotionShopRegion : OnceClickRegion {

		protected override void OnClick() {
			if (!DB.PlayerDataHelper.CanBuyPotion()) {
				return;
			}
			DB.PlayerDataHelper.BuyPotion();
			stageController.stageUi.playerStatusUi.Refresh();
		}

	}

}