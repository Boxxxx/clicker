﻿using UnityEngine;
using System.Collections;
using Clicker.DB;

namespace Clicker {

	public class BattleRegion : Region {

		public DamageText damageTextPrefab;

		MonsterDataInst monsterInfo;
		MonsterAnimation monsterAnime;
		UiLifeBar monsterLifeBar;
		BattleGenerator battleGenerator;

		CharacterAnimation charAnime;
		bool isBattleEntered;
		bool isBattleAnimePlaying;

		public override void Reset(RegionMeta meta, StageController stageController) {
			monsterInfo = new MonsterDataInst(meta.monsterMeta);

			monsterAnime = (GameObject.Instantiate(Resources.Load("Monster/monster_" + monsterInfo.raw.id)) as GameObject).GetComponent<MonsterAnimation>();
			monsterAnime.gameObject.transform.parent = this.transform;
			monsterAnime.gameObject.transform.localPosition = new Vector3(1.2f, GameConsts.Inst.monsterYOffset, 0);
			monsterAnime.anime.CrossFade("Idle");

			monsterLifeBar = stageController.stageUi.worldUi.CreateLifeBar(monsterAnime.lifeBarPos);
			monsterLifeBar.SetHp(monsterInfo.hp, monsterInfo.MaxHp);

			clickArea.gameObject.SetActive(false);
			text.text = "怪兽区域";
			this.stageController = stageController;
			charAnime = stageController.charAnime;
			isBattleEntered = false;
        }

		public override void Deactive() {
			if (monsterLifeBar != null) {
				stageController.stageUi.worldUi.RemoveWidgetLink(monsterAnime.lifeBarPos);
				GameObject.Destroy(monsterLifeBar.gameObject);
				monsterLifeBar = null;
			}
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
					var anime = UIAnimation.Sleep(1.5f);
					anime.onStart = () => {
						charAnime.anime.CrossFade("ATK2");
						monsterAnime.anime.CrossFade("Die");
					};
					anime.timeEvents.Add(new UIAnimation.TimeEvent(false, 0.7f, ()=> {
						monsterInfo.hp -= record.damage;
						ShowDamageText(monsterAnime.transform.localPosition + transform.localPosition,
							"-" + record.damage.ToString());
						monsterLifeBar.SetHp(monsterInfo.hp, monsterInfo.MaxHp);
					}));
					anime.onFinish = () => {
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
					anime.timeEvents.Add(new UIAnimation.TimeEvent(false, 0.3f, () => {
						PlayerData.Instance.GetCharacterData().hp -= record.damage;
						ShowDamageText(stageController.charAnime.transform.localPosition +
							stageController.charAnime.anime.transform.localPosition, "-" + record.damage.ToString());
					}));
					anime.onFinish = () => {
						
						if (PlayerData.Instance.GetCharacterData().hp <= 0) {
							stageController.GameLose();
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

		void ShowDamageText(Vector3 pos, string str) {
			var text = GameObject.Instantiate<DamageText>(damageTextPrefab);
			text.transform.parent = stageController.transform;
			text.transform.localPosition = pos;
			text.GetComponent<TextMesh>().text = str;
			text.BeginFloat();
		}

	}

}