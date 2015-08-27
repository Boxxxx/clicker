using System.Collections.Generic;
using FullInspector;

namespace Clicker {
    public abstract class RegionSelectPolicy : BaseBehavior {
        public abstract RegionType Select(
            int index,
            int date,
            List<Pair<int, RegionType>> regionHistory, 
            Dictionary<RegionType, int> regionCountMap, 
            Dictionary<RegionType, int> regionLastIndex, 
            Dictionary<RegionType, int> regionLastDate);
    }
}
