using UnityEngine;
using System.Collections;

namespace Clicker {

	public class PlayerStatusUi : MonoBehaviour {

		public UILabel labelHp;
		public UILabel labelAtk;
		public UILabel labelGold;
		public UISprite spriteItem;

		public void Refresh() {
			CharacterDataInst info = DB.PlayerData.Instance.GetCharacterData();

            labelHp.text = string.Format("{0}/{1}", info.hp, info.maxHp);
			labelAtk.text = info.atk.ToString();
			labelGold.text = info.gold.ToString();
			switch (info.itemType) {
				case ItemType.None:
					spriteItem.spriteName = "";
					break;
				case ItemType.Potion:
					spriteItem.spriteName = "potion";
					break;
				case ItemType.DivineReaper:
					spriteItem.spriteName = "divine_reaper";
					break;
				case ItemType.Cash:
					spriteItem.spriteName = "cash";
					break;
			}
		}

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}
	}

}