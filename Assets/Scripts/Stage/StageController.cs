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

		void Start() {
			DB.Database.Instance.LoadDatabase();
			regionPool = GetComponent<ReusePool>();
			regionPool.SetPoolSize(3);
			knightAnime.anime.CrossFade("Run");

			distanceSum = 0;
			nextRegion = CreateRegion();
			nextRegion.transform.localPosition = new Vector3(0, 0, 0);

			GoNextRegion();
		}

		Region CreateRegion() {
			var region = regionPool.Allocate().GetComponent<Region>();
			region.transform.parent = this.transform;
			RegionMeta meta = new RegionMeta();
			meta.type = RegionType.Monster;
			meta.length = ScreenWidth;
			meta.monsterInfo = new MonsterInfo(DB.Database.Instance.GetMonsterById("1"), 1);
			region.Reset(meta);

			return region;
		}

		void Update() {
			if (state == State.Running) {

			} else if (state == State.Battle) {
				
			}
		}

		void GoNextRegion() {
			if (lastRegion != null) {
				lastRegion.Deactive();
			}

			lastRegion = currentRegion;
			currentRegion = nextRegion;
			nextRegion = CreateRegion();
			nextRegion.transform.localPosition = currentRegion.transform.localPosition + new Vector3(currentRegion.length, 0, 0);

			nextPosition = distanceSum + currentRegion.length;
			distanceSum += currentRegion.length;

			var tween = UIAnimation.TweenPosition(knightAnime.gameObject,
				currentRegion.length / regionMoveSpeed,
				knightAnime.transform.localPosition,
				knightAnime.transform.localPosition + new Vector3(currentRegion.length, 0, 0));
			UIAnimator.Begin(gameObject, tween, EnterBattle);
			
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

		float ScreenWidth { get { return 1.0f * 2 * Screen.width / Screen.height; } }
	}

}