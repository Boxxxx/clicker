using UnityEngine;
using System.Collections;

namespace Clicker.DB {

	public class PlayerDataHelper {

		public static bool CanUpgradeWeapon() {
			var pData = PlayerData.Instance.GetCharacterData();
			if (pData.atkLevel + 1 >= ConstDB.Instance.GetDefMaxLevel()) {
				return false;
			}
			return pData.gold >= ConstDB.Instance.GetAtkToNextLevelGold(pData.atkLevel);
		}

		public static bool CanUpgradeArmor() {
			var pData = PlayerData.Instance.GetCharacterData();
			if (pData.defLevel + 1 >= ConstDB.Instance.GetDefMaxLevel()) {
				return false;
			}
			return pData.gold >= ConstDB.Instance.GetDefToNextLevelGold(pData.defLevel);
		}

	}

}