using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    public static class Lists {
        public static List<TElement> NewArrayList<TElement>(IEnumerable<TElement> source = null) {
            if (source == null) {
                return new List<TElement>();
            }
            else {
                return new List<TElement>(source);
            }
        }
        public static List<TElement> NewArrayList<TElement>(int size, TElement fill = default(TElement)) {
            List<TElement> list = new List<TElement>();
            for (int i = 0; i < size; i++) {
                list.Add(fill);
            }
            return list;
        }

        public static List<TElement> Filter<TElement>(List<TElement> list, Func<TElement, bool> selector) {
            return Linqs.Filter(list, selector);
        }

        public static List<TResult> Transform<TSource, TResult>(List<TSource> list, Func<TSource, TResult> convertor) {
            return Linqs.Map(list, convertor);
        }

        /// <summary>
        /// Gets all permutaions of list, they will be in ascending order.
        /// </summary>
        public static TElement[][] Permutation<TElement>(this List<TElement> list, Comparison<TElement> comparision) {
            var ret = new List<TElement[]>();
            var permutation = list.Clone();
            ret.Add(permutation.ToArray());
            while (list.NextPermutation(comparision)) {
                ret.Add(permutation.ToArray());
            }
            return ret.ToArray();
        }
        public static TElement[][] Permutation<TElement>(this List<TElement> list, IComparer<TElement> comparer) {
            return Permutation(list, (Comparison<TElement>)((lhs, rhs) => {
                return comparer.Compare(lhs, rhs);
            }));
        }
        public static TElement[][] Permutation<TElement>(this List<TElement> list) where TElement : IComparable {
            return Permutation(list, (Comparison<TElement>)((lhs, rhs) => {
                return lhs.CompareTo(rhs);
            }));
        }

        /// <summary>
        /// Transforms the list into next permutation, if there is no such permutation, return false, and sort the list in ascending order.
        /// </summary>
        public static bool NextPermutation<TElement>(this List<TElement> list, Comparison<TElement> comparision) {
            if (list.Count <= 1) {
                return false;
            }
            for (int i = list.Count - 2; i >= 0; i--) {
                if (comparision(list[i], list[i + 1]) < 0) {
                    int selectedIndex = i + 1;
                    TElement selection = list[i + 1];
                    for (int j = i + 2; j < list.Count; j++) {
                        if (comparision(list[i], list[j]) < 0 && comparision(list[j], selection) < 0) {
                            selectedIndex = j;
                            selection = list[j];
                        }
                    }
                    // Swaps i and selectedIndex
                    list[selectedIndex] = list[i];
                    list[i] = selection;
                    // And sort the rest elements
                    Sort(list, i + 1, list.Count, comparision);
                    return true;
                }
            }
            list.Sort(comparision);
            return false;
        }
        public static bool NextPermutation<TElement>(this List<TElement> list, IComparer<TElement> comparer) {
            return NextPermutation(list, (Comparison<TElement>)((lhs, rhs) => {
                return comparer.Compare(lhs, rhs);
            }));
        }
        public static bool NextPermutation<TElement>(this List<TElement> list) where TElement : IComparable {
            return NextPermutation(list, (Comparison<TElement>)((lhs, rhs) => {
                return lhs.CompareTo(rhs);
            }));
        }

        public static void Copy<TElement>(this List<TElement> source, IEnumerable<TElement> copyFrom, int startIndex = 0, int size = 0) {
            if (size == 0) {
                size = copyFrom.Count();
            }
            int cnt = 0;
            foreach (var element in copyFrom) {
                source[startIndex + cnt] = element;
                if ((++cnt) >= size) {
                    break;
                }
            }
        }

        public static void Sort<TElement>(this List<TElement> source, int from, int to, Comparison<TElement> comparision) {
            if (source.IsEmpty()) {
                return;
            }
            var sortedSlice = NewArrayList(Linqs.Slice(source, from, to));
            sortedSlice.Sort(comparision);
            var startIndex = (from % source.Count + source.Count) % source.Count;
            Copy(source, sortedSlice, startIndex);
        }
        public static void Sort<TElement>(this List<TElement> source, int from, int to, IComparer<TElement> comparer) {
            Sort(source, from, to, (Comparison<TElement>)((lhs, rhs) => {
                return comparer.Compare(lhs, rhs);
            }));
        }
        public static void Sort<TElement>(this List<TElement> source, int from, int to) where TElement : IComparable {
            Sort(source, from, to, (Comparison<TElement>)((lhs, rhs) => {
                return lhs.CompareTo(rhs);
            }));
        }

        public static List<TElement> Clone<TElement>(this IEnumerable<TElement> source) {
            return new List<TElement>(source);
        }
    }
}