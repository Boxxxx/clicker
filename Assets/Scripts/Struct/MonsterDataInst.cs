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

		public int MaxHp { get { return raw.hp; } }
		public int Atk { get { return raw.atk; } }
	}

}