using System.Collections.Generic;
using Utils;

namespace Clicker {
    public class RandomRegionSelectPolicy : RegionSelectPolicy {
        public Dictionary<RegionType, float[]> regionArgs;
        public delegate float CalculateWeightDelegate(float[] args, int passIndexSinceLast, int passTimeSinceLast);

        private int CurrentDate { get; set; }
        private List<Pair<int, RegionType>> RegionHistory { get; set; }
        private Dictionary<RegionType, int> RegionCountMap { get; set; }
        private Dictionary<RegionType, int> RegionLastIndex { get; set; }
        private Dictionary<RegionType, int> RegionLastDate { get; set; }

        private Dictionary<RegionType, CalculateWeightDelegate> m_calculators = new Dictionary<RegionType, CalculateWeightDelegate>();

        public RandomRegionSelectPolicy() {
            m_calculators.Add(RegionType.Battle, CalculateWeightForBattle);
            m_calculators.Add(RegionType.BlackSmith, CalculateWeightForBlackSmith);
        }

        public override RegionType Select(
            int index,
            int date,
            List<Pair<int, RegionType>> regionHistory,
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

                int passIndexSinceLast = regionLastIndex.ContainsKey(regionType) ? index - regionLastIndex[regionType] : -1;
                int passTimeSinceLast = regionLastDate.ContainsKey(regionType) ? index - regionLastDate[regionType] : -1;
                var args = regionArgs.ContainsKey(regionType) ? regionArgs[regionType] : null;

                float weight = calculator(args, passIndexSinceLast, passTimeSinceLast);
                if (weight == float.MaxValue) {
                    mustSelect.Add(regionType);
                }
                else {
                    weightList.Add(Pair.Of(regionType, weight));
                }
            }

            if (mustSelect.Count > 0) {
                return Randoms.Default.Range(mustSelect);
            }
            else {
                return Randoms.Default.RangeWithWeight(weightList, weightList.Map(pair => pair.Second)).First;
            }
        }

        #region Weight calculate functions for each RegionType
        private float CalculateWeightForBattle(float[] args, int passIndexSinceLast, int passTimeSinceLast) {
            return float.MaxValue;
        }

        private float CalculateWeightForBlackSmith(float[] args, int passIndexSinceLast, int passTimeSinceLast) {
            return float.MaxValue;
        }
        #endregion
    }
}