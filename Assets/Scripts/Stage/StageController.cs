using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class StageController : MonoBehaviour {

		enum State { Battle, Running }

		public KnightAnimation knightAnime;

		public float regionMoveSpeed;

		State state = State.Running;
		ReusePool regionPool;
		KnightInfo knightInfo;

		Region lastRegion;
		Region currentRegion;
		Region nextRegion;

		float nextPosition;
		float distanceSum;
		int regionCount;

		void Start() {
			DB.Database.Instance.LoadDatabase();
			regionPool = GetComponent<ReusePool>();
			regionPool.SetPoolSize(3);
			knightAnime.anime.CrossFade("Run");

			distanceSum = 0;
			regionCount = 0;
			knightInfo = new KnightInfo();
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = new Vector3(0, 0, 0);

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
			meta.monsterInfo = new MonsterInfo(DB.Database.Instance.GetMonsterById("1"), 1);
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
			if (Input.GetMouseButton(1)) {

			}
		}

		void GoNextRegion() {
			if (lastRegion != null) {
				lastRegion.Deactive();
			}

			lastRegion = currentRegion;
			currentRegion = nextRegion;
			nextRegion = CreateNextRegion();
			nextRegion.transform.localPosition = currentRegion.transform.localPosition + new Vector3(currentRegion.length, 0, 0);

			nextPosition = distanceSum + currentRegion.length;
			distanceSum += currentRegion.length;

			var tween = UIAnimation.TweenPosition(knightAnime.gameObject,
				currentRegion.length / regionMoveSpeed,
				knightAnime.transform.localPosition,
				knightAnime.transform.localPosition + new Vector3(currentRegion.length, 0, 0));
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
				knightAnime.anime.CrossFade("ATK2");
				currentRegion.monsterAnime.anime.CrossFade("Die");
			};
			builder.Last.onFinish = () => {
				knightAnime.anime.CrossFade("Run");
				GoNextRegion();
			};
			builder.MakeChain();

			UIAnimator.Begin(this.gameObject, builder.First);
		}
	}

}