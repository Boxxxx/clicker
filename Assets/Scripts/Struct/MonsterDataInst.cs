using UnityEngine;
using System.Collections;

namespace Clicker {

	using DB;

	public class MonsterDataInst {
		public DBMonster raw;
		public int level;
		public int hp;

		public MonsterDataInst(MonsterDataInst other) {
			raw = other.raw;
			level = other.level;
			hp = other.hp;
		}

		public MonsterDataInst(DBMonster dbMonster, int level) {
			this.raw = dbMonster;
			this.level = level;

			hp = MaxHp;
		}

        public MonsterDataInst(MonsterMeta monsterMeta) 
            : this(ConstDB.Instance.GetMonsterById(monsterMeta.monsterId), monsterMeta.level) { }

		public int MaxHp { get { return ConstDB.GetPropertyValue(raw.hp, level); } }
		public int Atk { get { return ConstDB.GetPropertyValue(raw.atk, level); } }
		public int GoldDrop { get { return ConstDB.GetPropertyValue(raw.goldDrop, level); } }
	}

}