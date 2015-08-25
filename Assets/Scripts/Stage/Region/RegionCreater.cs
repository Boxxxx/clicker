using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class RegionCreater : MonoBehaviour {

		public ReusePool battlePool;
		public ReusePool blackSmithPool;

		RegionMeta meta;
		StageController stageController;

		void Awake() {
			battlePool.SetPoolSize(3);
			blackSmithPool.SetPoolSize(3);
		}

		public Region Create(RegionMeta meta, StageController stageController) {
			Region ret = null;
			this.meta = meta;
			this.stageController = stageController;
			switch (meta.type) {
				case RegionType.Battle:
					ret = CreateBattle();
					break;
				case RegionType.BlackSmith:
					ret = CreateBlackSmith();
					break;
			}
			return ret;
		}

		BattleRegion CreateBattle() {
			BattleRegion ret = battlePool.Allocate<BattleRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

		BlackSmithRegion CreateBlackSmith() {
			BlackSmithRegion ret = blackSmithPool.Allocate<BlackSmithRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

	}
}