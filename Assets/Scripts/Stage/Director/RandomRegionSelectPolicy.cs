using Clicker.DB;
using System;
using System.Collections.Generic;
using Utils;

namespace Clicker {
    public class RandomRegionSelectPolicy : RegionSelectPolicy {
        [Serializable]
        public class RegionSelectInfo {
            public float weight = 0;
            public float[] args;
        }
        public Dictionary<RegionType, RegionSelectInfo> regionArgs;
        public delegate float CalculateWeightDelegate(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast);
        public delegate void PostProcessDelegate(RegionSelectInfo info, RegionMeta regionMeta);

        private int CurrentDate { get; set; }
        private List<RegionMeta> RegionHistory { get; set; }
        private Dictionary<RegionType, int> RegionCountMap { get; set; }
        private Dictionary<RegionType, int> RegionLastIndex { get; set; }
        private Dictionary<RegionType, int> RegionLastDate { get; set; }

        //!TODO(yfiengh): remove this in release version.
        private RegionSelectInfo _defaultRegionSelectInfo = new RegionSelectInfo();
        private Dictionary<RegionType, CalculateWeightDelegate> m_calculators = new Dictionary<RegionType, CalculateWeightDelegate>();
        private Dictionary<RegionType, PostProcessDelegate> m_postProcesses = new Dictionary<RegionType, PostProcessDelegate>();

        public RandomRegionSelectPolicy() {
            RegisterMethods();
        }

        public override RegionMeta Select(
            int index,
            int date,
            List<RegionMeta> regionHistory,
            Dictionary<RegionType, int> regionCountMap,
            Dictionary<RegionType, int> regionLastIndex,
            Dictionary<RegionType, int> regionLastDate) {

            CurrentDate = date;
            RegionHistory = regionHistory;
            RegionCountMap = regionCountMap;
            RegionLastIndex = regionLastIndex;
            RegionLastDate = regionLastDate;

            var mustSelect = new List<RegionType>();
            var weightList = new List<Pair<RegionType, float>>();
            foreach (var pair in m_calculators) {
                var regionType = pair.Key;
                var calculator = pair.Value;

                int passIndexSinceLast = regionLastIndex.ContainsKey(regionType) ? index - regionLastIndex[regionType] : index + 1;
                int passTimeSinceLast = regionLastDate.ContainsKey(regionType) ? date - regionLastDate[regionType] : date + 1;
                var regionSelectInfo = regionArgs.ContainsKey(regionType) ? regionArgs[regionType] : _defaultRegionSelectInfo;

                float weight = calculator(regionSelectInfo, passIndexSinceLast, passTimeSinceLast);
                if (weight == float.MaxValue) {
                    mustSelect.Add(regionType);
                }
                else {
                    weightList.Add(Pair.Of(regionType, weight));
                }
            }

            RegionType selectedType;
            if (mustSelect.Count > 0) {
                selectedType = Randoms.Default.Range(mustSelect);
            }
            else {
                selectedType = Randoms.Default.RangeWithWeight(weightList, weightList.Map(pair => pair.Second)).First;
            }
            var regionMeta = new RegionMeta() {
                date = date,
                type = selectedType
            };
            if (m_postProcesses.ContainsKey(selectedType)) {
                m_postProcesses[selectedType](
                    regionArgs.ContainsKey(selectedType) ? regionArgs[selectedType] : null,
                    regionMeta);
            }
            return regionMeta;
        }

        private void RegisterMethods() {
            // Register calculator methods
            m_calculators.Add(RegionType.Battle, CalculateWeightForBattle);
            m_calculators.Add(RegionType.BlackSmith, CalculateWeightForBlackSmith);
            m_calculators.Add(RegionType.ArmorSmith, CalculateWeightForArmorSmith);
            m_calculators.Add(RegionType.Tarven, CalculateWeightForTarven);
            m_calculators.Add(RegionType.PotionShop, CalculateWeightForPotionShop);
            m_calculators.Add(RegionType.StockMarket, CalculateWeightForStockMarket);
            m_calculators.Add(RegionType.DivineRelic, CalculateWeightForDivineRelic);

            // Register post process methods
            m_postProcesses.Add(RegionType.Battle, PostProcessForBattle);
        }

        #region Methods for each RegionType
        private float CalculateWeightForBattle(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            int cntOfSame = GetCountOfSameRegionAtTail(RegionType.Battle);
            // no continuous 4 battle
            if (cntOfSame >= 3) {
                return 0;
            }
            return info.weight;
        }

        private float CalculateWeightForBlackSmith(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            float weight = info.weight;

            if (passIndexSinceLast > info.args[0]) {
                weight *= passIndexSinceLast - info.args[0] + 1;
            }

            int cntOfSmith = GetCountOfSmithAtTail();
            // no continuous 3 black smith
            if (cntOfSmith >= 2) {
                return 0;
            }
            else if (cntOfSmith == 1) {
                weight *= 0.25f;
            }

            return weight;
        }

        private float CalculateWeightForArmorSmith(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            float weight = info.weight;

            if (passIndexSinceLast > info.args[0]) {
                weight *= passIndexSinceLast - info.args[0] + 1;
            }

            int cntOfSmith = GetCountOfSmithAtTail();
            // no continuous 3 armor smith
            if (cntOfSmith >= 2) {
                return 0;
            }
            else if (cntOfSmith == 1) {
                weight *= 0.25f;
            }

            return weight;
        }

        private float CalculateWeightForTarven(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            if (passIndexSinceLast >= info.args[0]) {
                return float.MaxValue;
            }
            return info.weight;
        }

        private float CalculateWeightForPotionShop(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            return info.weight;
        }

        private float CalculateWeightForStockMarket(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            return info.weight;
        }

        private float CalculateWeightForDivineRelic(RegionSelectInfo info, int passIndexSinceLast, int passTimeSinceLast) {
            return info.weight;
        }

        private void PostProcessForBattle(RegionSelectInfo info, RegionMeta regionMeta) {
            var monsterId = Randoms.Default.Range(ConstDB.Instance.GetAllMonsterIds());
            regionMeta.monsterMeta = new MonsterMeta(monsterId, 1);
        }
        #endregion

        #region Utility
        private int GetCountOfSameRegionAtTail(RegionType regionType) {
            int cnt = 0;
            for (int i = RegionHistory.Count - 1; i >= 0; i--) {
                if (RegionHistory[i].type == regionType) {
                    cnt++;
                }
                else {
                    break;
                }
            }
            return cnt;
        }

        private int GetCountOfSmithAtTail() {
            int cnt = 0;
            for (int i = RegionHistory.Count - 1; i >= 0; i--) {
                if (RegionHistory[i].type == RegionType.BlackSmith || RegionHistory[i].type == RegionType.ArmorSmith) {
                    cnt++;
                }
                else {
                    break;
                }
            }
            return cnt;
        }
        #endregion
    }
}