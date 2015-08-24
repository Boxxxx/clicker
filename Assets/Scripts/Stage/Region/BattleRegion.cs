using UnityEngine;
using System.Collections;

namespace Clicker {

	public class BattleRegion : Region {

		MonsterDataInst monsterInfo;
		MonsterAnimation monsterAnime;
		BattleGenerator battleGenerator;

		public override void Reset(RegionMeta meta) {
			monsterInfo = meta.monsterInfo;
			monsterAnime = (GameObject.Instantiate(Resources.Load("Monster/monster_" + monsterInfo.raw.id)) as GameObject).GetComponent<MonsterAnimation>();
			monsterAnime.gameObject.transform.parent = this.transform;
			monsterAnime.gameObject.transform.localPosition = new Vector3(1.2f, GameConsts.Inst.monsterYOffset, 0);

			monsterAnime.anime.CrossFade("Idle");

			clickArea.gameObject.SetActive(false);
			text.text = "怪兽区域";
		}

		public override void Deactive() {
			if (monsterAnime != null) {
				GameObject.Destroy(monsterAnime);
				monsterAnime = null;
			}
			base.Deactive();
		}

		public override void RegionUpdate() {
			if (!isInBattleAnime) {
				var record = battleGenerator.GenerateNext();
				if (record.recordType == BattleRecord.RecordType.Win || record.recordType == BattleRecord.RecordType.OurAtk) {
					isInBattleAnime = true;
					var anime = UIAnimation.Sleep(1.0f);
					anime.onStart = () => {
						charAnime.anime.CrossFade("ATK2");
						(currentRegion as BattleRegion).monsterAnime.anime.CrossFade("Die");
					};
					anime.onFinish = () => {
						currentRegion.monsterInfo.hp -= record.damage;
						if (currentRegion.monsterInfo.hp <= 0) {
							GoNextRegion();
						}
						isInBattleAnime = false;
						stageUi.playerStatusUi.Refresh();
					};
					UIAnimator.Begin(gameObject, anime);
				} else {
					isInBattleAnime = true;
					var anime = UIAnimation.Sleep(1.0f);
					anime.onStart = () => {
						charAnime.anime.CrossFade("Damage");
						currentRegion.monsterAnime.anime.CrossFade("ATK");
					};
					anime.onFinish = () => {
						PlayerData.Instance.GetCharacterData().hp -= record.damage;
						if (PlayerData.Instance.GetCharacterData().hp <= 0) {

						}
						isInBattleAnime = false;
						stageUi.playerStatusUi.Refresh();
					};
					UIAnimator.Begin(gameObject, anime);
				}
			}
		}

	}

}