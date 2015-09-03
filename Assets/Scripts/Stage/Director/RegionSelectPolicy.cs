using System.Collections.Generic;
using FullInspector;

namespace Clicker {
    public abstract class RegionSelectPolicy : BaseBehavior {
        public abstract RegionMeta Select(
            int index,
            int date,
            List<RegionMeta> regionHistory, 
            Dictionary<RegionType, int> regionCountMap, 
            Dictionary<RegionType, int> regionLastIndex, 
            Dictionary<RegionType, int> regionLastDate);
    }
}
