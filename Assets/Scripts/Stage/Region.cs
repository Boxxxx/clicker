using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class Region : ReusableObject {
		
		public TextMesh text;
		public BoxCollider clickArea;

		[HideInInspector]
		public MonsterInfo monsterInfo;
		[HideInInspector]
		public MonsterAnimation monsterAnime;
		[HideInInspector]
		public float length;
		[HideInInspector]
		public RegionType type = RegionType.BlackSmith;

		public void Reset(RegionMeta meta) {
			switch (meta.type) {
				case RegionType.Monster:
					monsterInfo = meta.monsterInfo;
					monsterAnime = (GameObject.Instantiate(Resources.Load("Monster/monster_" + monsterInfo.raw.id)) as GameObject).GetComponent<MonsterAnimation>();
					monsterAnime.gameObject.transform.parent = this.transform;
					monsterAnime.gameObject.transform.localPosition = new Vector3(1.2f, 0, 0);

					monsterAnime.anime.CrossFade("Idle");
					length = GameConsts.ScreenWidth;

					clickArea.gameObject.SetActive(false);
					text.text = "怪兽区域";
					break;
				case RegionType.BlackSmith:
					monsterAnime = null;

					length = GameConsts.ScreenWidth * 2;
					clickArea.gameObject.SetActive(true);
					clickArea.transform.localPosition = new Vector3(GameConsts.ScreenWidth, 0, 0);
					clickArea.size = new Vector3(GameConsts.ScreenWidth, GameConsts.ScreenHeight, 0);
					text.text = "武器升级";
					break;
			}
			type = meta.type;
		}

		public override void Deactive() {
			if (monsterAnime != null) {
				GameObject.Destroy(monsterAnime.gameObject);
			}
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