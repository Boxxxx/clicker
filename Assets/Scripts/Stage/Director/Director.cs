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
        public Director() {
            m_instance = this;
        }

        public int numOfRegionsPerDay = 5;
        public RegionSelectPolicy regionSelectPolicy = null;

        private int m_date = 0;
        private int m_offset = 0;
        private List<Pair<int, RegionType>> m_regionList = new List<Pair<int, RegionType>>();
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

        public Pair<int, RegionType>[] Forecast(int num) {
            EnsureRegionNum(m_offset + num);
            var retList = new List<Pair<int, RegionType>>();
            for (int i = 0; i < num; i++) {
                retList.Add(m_regionList[m_offset + i]);
            }
            return retList.ToArray();
        }

        public Pair<int, RegionType> NextRegion() {
            EnsureRegionNum(m_offset + 1);
            var regionData = m_regionList[m_offset++];
            m_date = regionData.First;
            return regionData;
        }

        public Pair<int, RegionType>[] NextRegions(int num) {
            var retList = new List<Pair<int, RegionType>>();
            for (var i = 0; i < num; i++) {
                retList.Add(NextRegion());
            }
            return retList.ToArray();
        }

        private RegionType GenerateNextRegion(int index, int currentDate) {
            return regionSelectPolicy.Select(index, currentDate, m_regionList, m_regionCountMap, m_regionLastIndex, m_regionLastDate);
        }

        private Pair<int, RegionType>[] GenerateNextRegions(int num) {
            var retList = new List<Pair<int, RegionType>>();
            int startIndex = m_regionList.Count;
            for (int i = 0; i < num; i++) {
                int index = startIndex + i;
                int currentDate = IndexToDate(index);
                RegionType currentRegion = GenerateNextRegion(index, currentDate);
                if (!m_regionCountMap.ContainsKey(currentRegion)) {
                    m_regionCountMap[currentRegion] = 0;
                }
                m_regionCountMap[currentRegion]++;
                m_regionLastIndex[currentRegion] = index;
                m_regionLastDate[currentRegion] = currentDate;

                var pair = Pair.Of(currentDate, currentRegion);
                m_regionList.Add(pair);
                retList.Add(pair);
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
