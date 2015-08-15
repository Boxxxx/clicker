using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class Region : ReusableObject {

		public RegionType type = RegionType.BlackSmith;
		public MonsterInfo monsterInfo;
		public MonsterAnimation monsterAnime;
		public float length;

		public void Reset(RegionMeta meta) {
			switch (meta.type) {
				case RegionType.Monster:
					monsterInfo = meta.monsterInfo;
					monsterAnime = (GameObject.Instantiate(Resources.Load("Monster/monster_" + monsterInfo.raw.id)) as GameObject).GetComponent<MonsterAnimation>();
					monsterAnime.gameObject.transform.parent = this.transform;
					monsterAnime.gameObject.transform.localPosition = new Vector3(1.2f, 0, 0);

					monsterAnime.anime.CrossFade("Idle");
					length = meta.length;
					break;
				case RegionType.BlackSmith:

					break;
			}
			type = meta.type;
		}

		public override void Deactive() {
			GameObject.Destroy(monsterAnime.gameObject);
			base.Deactive();
		}
	}

	public class RegionMeta {
		public RegionType type;
		/// <summary>
		/// Only used when type = Monster
		/// </summary>
		public MonsterInfo monsterInfo;
		public float length;
	}

	public enum RegionType {
		Monster,
		BlackSmith
	}

}