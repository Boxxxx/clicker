using UnityEngine;
using System.Collections;

namespace Clicker {

	using DB;

	public class MonsterInfo {
		public DBMonster raw;
		public int level;

		public MonsterInfo(DBMonster dbMonster, int level) {
			this.raw = dbMonster;
			this.level = level;
		}
	}

}