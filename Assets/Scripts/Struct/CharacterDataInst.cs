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
		}

		public void SetAtkLevel(int level) {
			var property = ConstDB.Instance.GetAtkProperty(level);
			atkLevel = level;
			atk = property.value;
		}

		public void SetDefLevel(int level) {
			var property = ConstDB.Instance.GetDefProperty(level);
			defLevel = level;
			maxHp = property.value;
			hp = maxHp;
		}
	}

	public enum ItemType {
		None,
		Potion,
		DivineReaper,
		Cash
	}
}