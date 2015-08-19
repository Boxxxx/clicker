using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class StageController : MonoBehaviour {

		enum State { Battle, Running }

		public CharacterAnimation charAnime;

		State state = State.Running;
		ReusePool regionPool;

		Region lastRegion;
		Region currentRegion;
		Region nextRegion;

		float nextPosition;
		float distanceSum;
		int regionCount;
		int regionLayer;

		void DbInit() {
			DB.ConstDB.Instance.LoadDatabase();
			DB.PlayerData.Instance.LoadData();
		}

		void Awake() {
			DbInit();
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
		}

		void TriggerCurrentRegionEvent() {
			if (currentRegion.type == RegionType.BlackSmith) {
				if (currentRegion.isTriggered == false) {
					currentRegion.isTriggered = true;

					Debug.Log("Weapon level up!");

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
			UIAnimator.Begin(gameObject, tween, RegionAction);
			
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
			var builder = new UIAnimationBuilder();
			builder += new UIAnimation(1.0f, (p) => { });
			builder.Last.onStart = () => {
				charAnime.anime.CrossFade("ATK2");
				currentRegion.monsterAnime.anime.CrossFade("Die");
			};
			builder.Last.onFinish = () => {
				charAnime.anime.CrossFade("Run");
				GoNextRegion();
			};
			builder.MakeChain();

			UIAnimator.Begin(this.gameObject, builder.First);
		}
	}

}