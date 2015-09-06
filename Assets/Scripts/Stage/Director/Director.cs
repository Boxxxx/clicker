using UnityEngine;
using System;
using System.Collections.Generic;

namespace Clicker {
    public class Director : MonoBehaviour {
        private static Director m_instance;
        public static Director Instance {
            get {
                return m_instance;
            }
        }
        void Awake() {
            if (m_instance != null) {
                Debug.LogWarning("You should not have two directors in a scene!");
                Destroy(this);
                return;
            }
            m_instance = this;
        }

        public int numOfRegionsPerDay = 4;
        public RegionSelectPolicy regionSelectPolicy = null;

        private int m_date = 0;
        private int m_offset = 0;
        private List<RegionMeta> m_regionList = new List<RegionMeta>();
        private Dictionary<RegionType, int> m_regionCountMap = new Dictionary<RegionType, int>();
        private Dictionary<RegionType, int> m_regionLastIndex = new Dictionary<RegionType, int>();
        private Dictionary<RegionType, int> m_regionLastDate = new Dictionary<RegionType, int>();

        public void Reset() {
            m_date = 0;
            m_offset = 0;
            m_regionList.Clear();
            m_regionCountMap.Clear();
            m_regionLastIndex.Clear();
            m_regionLastDate.Clear();
        }

        public void PrepareRegions(int num) {
            GenerateNextRegions(num);
        }

        public void EnsureRegionNum(int num) {
            if (num <= m_regionList.Count) {
                return;
            }
            GenerateNextRegions(num - m_regionList.Count);
        }

        public RegionMeta[] Forecast(int num) {
            EnsureRegionNum(m_offset + num);
            var retList = new List<RegionMeta>();
            for (int i = 0; i < num; i++) {
                retList.Add(m_regionList[m_offset + i]);
            }
            return retList.ToArray();
        }

        public RegionMeta NextRegion() {
            EnsureRegionNum(m_offset + 1);
            var regionData = m_regionList[m_offset++];
            m_date = regionData.date;
            return regionData;
        }

        public RegionMeta[] NextRegions(int num) {
            var retList = new List<RegionMeta>();
            for (var i = 0; i < num; i++) {
                retList.Add(NextRegion());
            }
            return retList.ToArray();
        }

        private RegionMeta GenerateNextRegion(int index, int currentDate) {
            return regionSelectPolicy.Select(index, currentDate, m_regionList, m_regionCountMap, m_regionLastIndex, m_regionLastDate);
        }

        private RegionMeta[] GenerateNextRegions(int num) {
            var retList = new List<RegionMeta>();
            int startIndex = m_regionList.Count;
            for (int i = 0; i < num; i++) {
                int index = startIndex + i;
                int currentDate = IndexToDate(index);
                RegionMeta currentRegion = GenerateNextRegion(index, currentDate);
                RegionType regionType = currentRegion.type;
                if (!m_regionCountMap.ContainsKey(regionType)) {
                    m_regionCountMap[regionType] = 0;
                }
                m_regionCountMap[regionType]++;
                m_regionLastIndex[regionType] = index;
                m_regionLastDate[regionType] = currentDate;
                m_regionList.Add(currentRegion);
                retList.Add(currentRegion);
            }
            return retList.ToArray();
        }

        private int IndexToDate(int index) {
            return index / numOfRegionsPerDay;
        }

        private Pair<int, int> DateToIndexRange(int date) {
            return Pair.Of(date * numOfRegionsPerDay, date * numOfRegionsPerDay + numOfRegionsPerDay - 1);
        }
    }
}
