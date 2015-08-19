using UnityEngine;
using System.Collections;

namespace Clicker.DB {

	public class PlayerData {

		private static PlayerData instance;
		public static PlayerData Instance {
			get {
				if (instance == null) {
					instance = new PlayerData();
				}
				return instance;
			}
		}

		private CharacterDataInst characterData = new CharacterDataInst();

		private PlayerData() {

		}

		public void LoadData() {
			characterData = new CharacterDataInst();
			characterData.SetAtkLevel(0);
			characterData.SetDefLevel(0);
			characterData.itemType = ItemType.None;
		}

		public CharacterDataInst GetCharacterData() {
			return characterData;
		}
	}

}