using UnityEngine;
using System.Collections;
using Box;

namespace Clicker {

	public class RegionCreater : MonoBehaviour {

		public ReusePool battlePool;
		public ReusePool blackSmithPool;
		public ReusePool armorSmithPool;
		public ReusePool tarvenPool;
		public ReusePool potionShopPool;
		public ReusePool stockMarketPool;
		public ReusePool divineRelicPool;

		RegionMeta meta;
		StageController stageController;

		void Start() {

		}

		public void SetAllPoolSize() {
			battlePool.SetPoolSize(3);
			blackSmithPool.SetPoolSize(3);
			armorSmithPool.SetPoolSize(3);
			tarvenPool.SetPoolSize(3);
			potionShopPool.SetPoolSize(3);
			stockMarketPool.SetPoolSize(3);
			divineRelicPool.SetPoolSize(3);
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
				case RegionType.Tarven:
					ret = CreateTarvenRegion();
                    break;
				case RegionType.PotionShop:
					ret = CreatePotionShopRegion();
					break;
				case RegionType.StockMarket:
					ret = CreateStockMarketRegion();
					break;
				case RegionType.DivineRelic:
					ret = CreateDivineRelicRegion();
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

		TarvenRegion CreateTarvenRegion() {
			TarvenRegion ret = tarvenPool.Allocate<TarvenRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

		PotionShopRegion CreatePotionShopRegion() {
			PotionShopRegion ret = potionShopPool.Allocate<PotionShopRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

		StockMarketRegion CreateStockMarketRegion() {
			StockMarketRegion ret = stockMarketPool.Allocate<StockMarketRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

		DivineRelicRegion CreateDivineRelicRegion() {
			DivineRelicRegion ret = divineRelicPool.Allocate<DivineRelicRegion>();
			ret.Reset(meta, stageController);
			return ret;
		}

	}
}