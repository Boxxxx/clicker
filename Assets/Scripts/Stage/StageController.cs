using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Box;
using Clicker.DB;

namespace Clicker {

	public class StageController : MonoBehaviour {
		public CharacterAnimation charAnime;
		public StageUi stageUi;
		public RegionCreater regionCreater;

		Region lastRegion;
		Region currentRegion;
		Region nextRegion;

		float nextPosition;
		float distanceSum;
		int regionCount;

		void DbInit() {
			ConstDB.Instance.LoadDatabase();
			PlayerData.Instance.LoadData();
		}

		void Awake() {
			DbInit();
			// TODO(sonicmisora): remove this test line
			PlayerData.Instance.GetCharacterData().gold += 10000;
		}

		void Start() {
			regionCreater.SetAllPoolSize();
			charAnime.anime.CrossFade("Run");

			distanceSum = 0;
			regionCount = 0;
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = new Vector3(0, 0, 0);

			GoNextRegion();
		}

		void Update() {
			if (currentRegion != null) {
				currentRegion.RegionUpdate();
			}
			stageUi.worldUi.UpdateAll();
		}

		// TODO(sonicmisoa): move create region logic into another component
		Region CreateNextRegion() {
			regionCount++;
			if (regionCount % 2 == 1) {
				return CreateBattleRegion();
			} else {
				return CreateBlackSmithRegion();
			}
		}

		Region CreateBattleRegion() {
			RegionMeta meta = new RegionMeta();
			meta.type = RegionType.Battle;
			meta.monsterInfo = new MonsterDataInst(ConstDB.Instance.GetMonsterById("1"), 1);

			var region = regionCreater.Create(meta, this);
			region.transform.parent = transform;

			return region;
		}

		Region CreateBlackSmithRegion() {
			RegionMeta meta = new RegionMeta();
			meta.type = RegionType.BlackSmith;

			var region = regionCreater.Create(meta, this);
			region.transform.parent = transform;

			return region;
		}

		/// <summary>
		/// Make the character go to next region and switch current region to the last one
		/// </summary>
		public void GoNextRegion() {
			if (lastRegion != null) {
				lastRegion.Deactive();
			}

			lastRegion = currentRegion;
			if (lastRegion != null) {
				lastRegion.clickArea.gameObject.SetActive(false);
			}
			currentRegion = nextRegion;
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = currentRegion.transform.localPosition + new Vector3(currentRegion.Length, 0, 0);

			nextPosition = distanceSum + currentRegion.Length;
			distanceSum += currentRegion.Length;

			var tween = UIAnimation.TweenPosition(charAnime.gameObject,
				currentRegion.Length / GameConsts.Inst.characterMoveSpeed,
				charAnime.transform.localPosition,
				currentRegion.transform.localPosition + new Vector3(currentRegion.KeyPointOffset, 0, 0));
			charAnime.anime.CrossFade("Run");
			UIAnimator.Begin(gameObject, tween, RegionAction);
		}

		public void GameLose() {
			stageUi.loseLabel.gameObject.SetActive(true);
		}

		void RegionAction() {
			currentRegion.KeyPointAction();
		}
	}

}