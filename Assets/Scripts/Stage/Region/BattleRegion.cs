using UnityEngine;
using System.Collections;
using Clicker.DB;

namespace Clicker {

	public class BattleRegion : Region {

		MonsterDataInst monsterInfo;
		MonsterAnimation monsterAnime;
		BattleGenerator battleGenerator;

		CharacterAnimation charAnime;
		bool isBattleEntered;
		bool isBattleAnimePlaying;

		public override void Reset(RegionMeta meta, StageController stageController) {
			monsterInfo = meta.monsterInfo;
			monsterAnime = (GameObject.Instantiate(Resources.Load("Monster/monster_" + monsterInfo.raw.id)) as GameObject).GetComponent<MonsterAnimation>();
			monsterAnime.gameObject.transform.parent = this.transform;
			monsterAnime.gameObject.transform.localPosition = new Vector3(1.2f, GameConsts.Inst.monsterYOffset, 0);

			monsterAnime.anime.CrossFade("Idle");

			clickArea.gameObject.SetActive(false);
			text.text = "怪兽区域";
			this.stageController = stageController;
			charAnime = stageController.charAnime;
			isBattleEntered = false;
        }

		public override void Deactive() {
			if (monsterAnime != null) {
				GameObject.Destroy(monsterAnime.gameObject);
				monsterAnime = null;
			}
			base.Deactive();
		}

		public override void RegionUpdate() {
			if (!isBattleEntered) {
				return;
			}

			if (!isBattleAnimePlaying) {
				var record = battleGenerator.GenerateNext();
				if (record.recordType == BattleRecord.RecordType.Win || record.recordType == BattleRecord.RecordType.OurAtk) {
					isBattleAnimePlaying = true;
					var anime = UIAnimation.Sleep(1.0f);
					anime.onStart = () => {
						charAnime.anime.CrossFade("ATK2");
						monsterAnime.anime.CrossFade("Die");
					};
					anime.onFinish = () => {
						monsterInfo.hp -= record.damage;
						if (monsterInfo.hp <= 0) {
							stageController.GoNextRegion();
						}
						isBattleAnimePlaying = false;
						stageController.stageUi.playerStatusUi.Refresh();
					};
					UIAnimator.Begin(gameObject, anime);
				} else {
					isBattleAnimePlaying = true;
					var anime = UIAnimation.Sleep(1.0f);
					anime.onStart = () => {
						charAnime.anime.CrossFade("Damage");
						monsterAnime.anime.CrossFade("ATK");
					};
					anime.onFinish = () => {
						PlayerData.Instance.GetCharacterData().hp -= record.damage;
						if (PlayerData.Instance.GetCharacterData().hp <= 0) {

						}
						isBattleAnimePlaying = false;
						stageController.stageUi.playerStatusUi.Refresh();
					};
					UIAnimator.Begin(gameObject, anime);
				}
			}
		}

		public override void KeyPointAction() {
			battleGenerator = new BattleGenerator(PlayerData.Instance.GetCharacterData(), monsterInfo);
			isBattleEntered = true;
			isBattleAnimePlaying = false;
        }

	}

}