using UnityEngine;
using System.Collections;

namespace Clicker.DB {

	public class PlayerDataHelper {

		public static bool CanUpgradeWeapon() {
			var pData = PlayerData.Instance.GetCharacterData();
			if (pData.atkLevel >= ConstDB.Instance.GetAtkMaxLevel()) {
				return false;
			}
			return pData.gold >= ConstDB.Instance.GetAtkToNextLevelGold(pData.atkLevel);
		}

		public static bool CanUpgradeArmor() {
			var pData = PlayerData.Instance.GetCharacterData();
			if (pData.defLevel >= ConstDB.Instance.GetDefMaxLevel()) {
				return false;
			}
			return pData.gold >= ConstDB.Instance.GetDefToNextLevelGold(pData.defLevel);
		}

		public static void UpgradeWeapon() {
			var pData = PlayerData.Instance.GetCharacterData();
			pData.gold -= ConstDB.Instance.GetAtkToNextLevelGold(pData.atkLevel);
			pData.SetAtkLevel(pData.atkLevel + 1);
		}

		public static void UpgradeArmor() {
			var pData = PlayerData.Instance.GetCharacterData();
			pData.gold -= ConstDB.Instance.GetDefToNextLevelGold(pData.defLevel);
			pData.SetDefLevel(pData.defLevel + 1);
		}

	}

}