using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rei.Random;

namespace Utils {

    public static class Randoms {
        public static readonly Random Default = RandomFactory.Create(RandomFactory.AlgorithmType.Default);
        public static float NextFloat(this Random random) {
            return (float)random.NextDouble();
        }

        public static double Range(this Random random, double min, double max) {
            return (max - min) * random.NextDouble() + min;
        }
        public static float Range(this Random random, float min, float max) {
            return (max - min) * random.NextFloat() + min;
        }
        /// <summary>
        /// Picks an integer from [min, max) 
        /// </summary>
        public static int Range(this Random random, int min, int max) {
            return random.Next(min, max);
        }
        /// <summary>
        /// Picks an integer from [min, max]
        /// </summary>
        public static int RangeInclude(this Random random, int min, int max) {
            return random.Next(min, max + 1);
        }

        public static T Range<T>(this Random random, IEnumerable<T> list) {
            int index = random.Next(0, list.Count());
            int cnt = 0;
            foreach (var ele in list) {
                if (cnt >= index) {
                    return ele;
                }
                cnt++;
            }
            throw new IndexOutOfRangeException();
        }
        public static T RangeWithDefault<T>(this Random random, IEnumerable<T> list) {
            int index = random.Next(0, list.Count());
            int cnt = 0;
            foreach (var ele in list) {
                if (cnt >= index) {
                    return ele;
                }
                cnt++;
            }
            return default(T);
        }
        public static T RangeWithWeight<T>(this Random random, IEnumerable<T> list, List<float> weights) {
            Asserts.Equals(list.Count(), weights.Count);
            Asserts.Assert(list.Count() > 0);

            float weightSum = 0;
            foreach (float weight in weights) {
                weightSum += weight;
            }
            float sum = 0;
            float value = random.NextFloat() * weightSum;
            int index = 0;
            T lastOne = default(T);
            foreach (var ele in list) {
                sum += weights[index++];
                lastOne = ele;
                if (sum >= value) {
                    return ele;
                }
            }
            return lastOne;
        }

        public static void Shuffle<T>(this Random random, List<T> list) {
            for (int i = 0; i < list.Count; i++) {
                int toIndex = i + random.Next(0, list.Count - i);
                // swap i and toIndex
                T tmp = list[i];
                list[i] = list[toIndex];
                list[toIndex] = tmp;
            }
        } 
    }

}