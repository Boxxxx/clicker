using UnityEngine;
using System.Collections;
using Clicker.DB;

namespace Clicker {

	public class CharacterDataInst {
		public int maxHp = 0;
		public int hp = 0;
		public int atk = 0;

		public int defLevel = 0;
		public int atkLevel = 0;

		public int gold = 0;
		public ItemType itemType = ItemType.None;

		public float currentLifeTime = 0;
		public bool isDivineReaperUsed = false;
		public bool isStockUsed = false;

		public float RemainingLifePercent {
			get {
				if (currentLifeTime >= ConstDB.Instance.GetCharLifeTime()) {
					return 0.0f;
				}
				return 1.0f - currentLifeTime / ConstDB.Instance.GetCharLifeTime();
			}
		}

		public CharacterDataInst() {

		}

		public CharacterDataInst(CharacterDataInst other) {
			maxHp = other.maxHp;
			hp = other.hp;
			atk = other.atk;
			defLevel = other.defLevel;
			atkLevel = other.atkLevel;
			gold = other.gold;
			itemType = other.itemType;
			currentLifeTime = other.currentLifeTime;
			isDivineReaperUsed = other.isDivineReaperUsed;
			isStockUsed = other.isStockUsed;
		}

		public void SetAtkLevel(int level) {
			atkLevel = level;
			atk = ConstDB.GetPropertyValue(ConstDB.Instance.GetCharacter().atkLevels, level);
		}

		public void SetDefLevel(int level) {
			defLevel = level;
			maxHp = ConstDB.GetPropertyValue(ConstDB.Instance.GetCharacter().defLevels, level);
			hp = maxHp;
		}
	}

	public enum ItemType {
		None,
		Potion,
		DivineReaper,
		Stock
	}
}