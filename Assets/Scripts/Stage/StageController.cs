using UnityEngine;
using System.Collections;
using Box;
using Clicker.DB;

namespace Clicker {

	public class StageController : MonoBehaviour {

		enum State { Battle, Running }

		public CharacterAnimation charAnime;
		public StageUi stageUi;

		State state = State.Running;
		ReusePool regionPool;

		Region lastRegion;
		Region currentRegion;
		Region nextRegion;

		float nextPosition;
		float distanceSum;
		int regionCount;
		int regionLayer;

		BattleGenerator battleGenerator;
		bool isInBattleAnime;

		void DbInit() {
			ConstDB.Instance.LoadDatabase();
			PlayerData.Instance.LoadData();
		}

		void Awake() {
			DbInit();
			PlayerData.Instance.GetCharacterData().gold += 10000;
		}

		void Start() {

			regionPool = GetComponent<ReusePool>();
			regionPool.SetPoolSize(3);
			charAnime.anime.CrossFade("Run");

			distanceSum = 0;
			regionCount = 0;
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = new Vector3(0, 0, 0);

			regionLayer = 1 << LayerMask.NameToLayer("Region");

			GoNextRegion();
		}

		Region CreateNextRegion() {
			regionCount++;
			if (regionCount % 2 == 1) {
				return CreateBattleRegion();
			} else {
				return CreateBlackSmithRegion();
			}
		}

		Region CreateBattleRegion() {
			var region = regionPool.Allocate().GetComponent<Region>();
			region.transform.parent = this.transform;
			RegionMeta meta = new RegionMeta();
			meta.type = RegionType.Monster;
			meta.monsterInfo = new MonsterDataInst(DB.ConstDB.Instance.GetMonsterById("1"), 1);
			region.Reset(meta);

			return region;
		}

		Region CreateBlackSmithRegion() {
			var region = regionPool.Allocate().GetComponent<Region>();
			region.transform.parent = this.transform;
			RegionMeta meta = new RegionMeta();
			meta.type = RegionType.BlackSmith;
			region.Reset(meta);

			return region;
		}

		void Update() {
			if (Input.GetMouseButtonDown(0)) {
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 100, regionLayer)) {
					TriggerCurrentRegionEvent();
				}
			}

			if (state == State.Battle) {
				if (!isInBattleAnime) {
					var record = battleGenerator.GenerateNext();
					if (record.recordType == BattleRecord.RecordType.Win || record.recordType == BattleRecord.RecordType.OurAtk) {
						isInBattleAnime = true;
						var anime = UIAnimation.Sleep(1.0f);
						anime.onStart = () => {
							charAnime.anime.CrossFade("ATK2");
							currentRegion.monsterAnime.anime.CrossFade("Die");
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

		#region Game Logic

		void TryUpgradeWeapon() {
			if (!PlayerDataHelper.CanUpgradeWeapon()) {
				return;
			}
			PlayerDataHelper.UpgradeWeapon();
			stageUi.playerStatusUi.Refresh();
		}

		#endregion

		void TriggerCurrentRegionEvent() {
			if (currentRegion.type == RegionType.BlackSmith) {
				if (currentRegion.isTriggered == false) {
					currentRegion.isTriggered = true;
					TryUpgradeWeapon();
					currentRegion.clickArea.gameObject.SetActive(false);
				}
			}
		}

		void GoNextRegion() {
			if (lastRegion != null) {
				lastRegion.Deactive();
			}

			lastRegion = currentRegion;
			if (lastRegion != null) {
				lastRegion.clickArea.gameObject.SetActive(false);
			}
			currentRegion = nextRegion;
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = currentRegion.transform.localPosition + new Vector3(currentRegion.length, 0, 0);

			nextPosition = distanceSum + currentRegion.length;
			distanceSum += currentRegion.length;

			var tween = UIAnimation.TweenPosition(charAnime.gameObject,
				currentRegion.length / GameConsts.Inst.characterMoveSpeed,
				charAnime.transform.localPosition,
				charAnime.transform.localPosition + new Vector3(currentRegion.length, 0, 0));
			charAnime.anime.CrossFade("Run");
			UIAnimator.Begin(gameObject, tween, RegionAction);

			state = State.Running;
		}

		void RegionAction() {
			if (currentRegion.type == RegionType.Monster) {
				EnterBattle();
			} else {
				GoNextRegion();
			}
		}

		void EnterBattle() {
			state = State.Battle;
			battleGenerator = new BattleGenerator(PlayerData.Instance.GetCharacterData(), currentRegion.monsterInfo);
			isInBattleAnime = false;


			
		}
	}

}