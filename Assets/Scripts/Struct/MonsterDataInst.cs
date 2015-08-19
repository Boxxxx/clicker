using UnityEngine;
using System.Collections;

namespace Clicker {

	using DB;

	public class MonsterDataInst {
		public DBMonster raw;
		public int level;

		public MonsterDataInst(DBMonster dbMonster, int level) {
			this.raw = dbMonster;
			this.level = level;
		}
	}

}