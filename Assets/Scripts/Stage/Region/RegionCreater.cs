using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class RegionCreater : MonoBehaviour {

		public ReusePool battlePool;
		public ReusePool blackSmithPool;
		public ReusePool armorSmithPool;

		RegionMeta meta;
		StageController stageController;

		void Start() {

		}

		public void SetAllPoolSize() {
			battlePool.SetPoolSize(3);
			blackSmithPool.SetPoolSize(3);
			armorSmithPool.SetPoolSize(3);
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
				case RegionType.ArmorSmith:
					ret = CreateArmorSmith();
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

		ArmorSmithRegion CreateArmorSmith() {
			ArmorSmithRegion ret = armorSmithPool.Allocate<ArmorSmithRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

	}
}