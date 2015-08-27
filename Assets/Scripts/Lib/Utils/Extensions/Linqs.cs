using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    public static class Linqs {
        public static TValue[] ToArray<TValue>(this IEnumerable<TValue> source) {
            return source.ToList().ToArray();
        }
        public static List<TValue> ToList<TValue>(this IEnumerable<TValue> source) {
            List<TValue> list = new List<TValue>();
            foreach (var i in source) {
                list.Add(i);
            }
            return list;
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source) {
            foreach (var element in source) {
                return element;
            }
            throw new IndexOutOfRangeException();
        }
        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (predicate(element)) {
                    return element;
                }
            }
            throw new IndexOutOfRangeException();
        }
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source) {
            foreach (var element in source) {
                return element;
            }
            return default(TSource);
        }
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (predicate(element)) {
                    return element;
                }
            }
            return default(TSource);
        }
        public static TSource Last<TSource>(this IList<TSource> list) {
            return list[list.Count - 1];
        }
        public static TSource Last<TSource>(this IEnumerable<TSource> source) {
            TSource last = default(TSource);
            bool flag = false;
            foreach (var element in source) {
                flag = true;
                last = element;
            }
            if (!flag) {
                throw new IndexOutOfRangeException();
            }
            return last;
        }
        public static TSource Last<TSource>(this IList<TSource> source, Func<TSource, bool> predicate) {
            for (var i = source.Count - 1; i >= 0; i--) {
                if (predicate(source[i])) {
                    return source[i];
                }
            }
            throw new IndexOutOfRangeException();
        }
        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            TSource last = default(TSource);
            bool flag = false;
            foreach (var element in source) {
                if (predicate(element)) {
                    flag = true;
                    last = element;
                }
            }
            if (!flag) {
                throw new IndexOutOfRangeException();
            }
            return last;
        }
        public static TSource LastOrDefault<TSource>(this IList<TSource> source) {
            TSource last = default(TSource);
            if (source.Count > 0) {
                last = source[source.Count - 1];
            }
            return last;
        }
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source) {
            TSource last = default(TSource);
            foreach (var element in source) {
                last = element;
            }
            return last;
        }
        public static TSource LastOrDefault<TSource>(this IList<TSource> source, Func<TSource, bool> predicate) {
            for (var i = source.Count - 1; i >= 0; i--) {
                if (predicate(source[i])) {
                    return source[i];
                }
            }
            return default(TSource);
        }
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            TSource last = default(TSource);
            foreach (var element in source) {
                if (predicate(element)) {
                    last = element;
                }
            }
            return last;
        }
        public static TSource Single<TSource>(this IEnumerable<TSource> source) {
            if (source.Count() == 1) {
                return source.First();
            }
            throw new InvalidOperationException();
        }
        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            return source.Filter(predicate).Single();
        }
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source) {
            var count = source.Count();
            if (count == 1) {
                return source.First();
            }
            else if (count == 0) {
                return default(TSource);
            }
            throw new InvalidOperationException();
        }
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            return source.Filter(predicate).SingleOrDefault();
        }
        public static List<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count) {
            var ret = new List<TSource>();
            int curCount = 0;
            foreach (var element in source) {
                curCount++;
                if (curCount > count) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static List<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            var ret = new List<TSource>();
            bool flag = false;
            foreach (var element in source) {
                if (!flag && !predicate(element)) {
                    flag = true;
                }
                if (flag) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static List<TSource> Take<TSource>(this IEnumerable<TSource> source, int count) {
            var ret = new List<TSource>();
            if (count == 0) {
                return ret;
            }

            int curCount = 0;
            foreach (var element in source) {
                ret.Add(element);
                if ((++curCount) >= count) {
                    break;
                }
            }
            return ret;
        }
        public static List<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            var ret = new List<TSource>();
            foreach (var element in source) {
                if (predicate(element)) {
                    ret.Add(element);
                }
                else {
                    break;
                }
            }
            return ret;
        }
        
        public static void AddRange<TValue>(this ICollection<TValue> source, IEnumerable<TValue> addition) {
            foreach (var element in addition) {
                source.Add(element);
            }
        }
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source) {
            return source.Count() == 0;
        }
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (predicate(element)) {
                    return true;
                }
            }
            return false;
        }
        public static bool IsNullOrEmpty<T>(IEnumerable<T> source) {
            return source == null || source.Count() == 0;
        }
        public static List<TResult> Repeat<TResult>(TResult element, int count) {
            var ret = new List<TResult>();
            for (var i = 0; i < count; i++) {
                ret.Add(element);
            }
            return ret;
        }
        public static List<TSource> Reverse<TSource>(this IEnumerable<TSource> source) {
            var ret = source.ToList();
            ret.Reverse();
            return ret;
        }
        public static List<TSource> Flatten<TSource>(this IEnumerable<IEnumerable<TSource>> source) {
            var ret = new List<TSource>();
            foreach (var list in source) {
                ret.AddRange(list);
            }
            return ret;
        }
        public static IList<TSource> Resize<TSource>(this IList<TSource> source, int size, TSource fill = default(TSource)) {
            size = Maths.Max(size, 0);
            if (source.Count > size) {
                while (source.Count > size) {
                    // Removes last element
                    source.RemoveAt(source.Count - 1);
                }
            }
            else if (source.Count < size) {
                for (int i = source.Count; i < size; i++) {
                    source.Add(fill);
                }
            }
            return source;
        }
        public static IList<TSource> Expand<TSource>(this IList<TSource> source, int size, TSource fill = default(TSource)) {
            return Resize(source, Math.Max(size, source.Count), fill);
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (!predicate(element)) {
                    return false;
                }
            }
            return true;
        }
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (predicate(element)) {
                    return true;
                }
            }
            return false;
        }
        public static List<TResult> Cast<TResult>(this IEnumerable source) {
            List<TResult> ret = new List<TResult>();
            foreach (var element in source) {
                ret.Add((TResult)element);
            }
            return ret;
        }
        public static List<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) {
            var ret = first.ToList();
            ret.AddRange(second);
            return ret;
        }
        public static List<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> mapFunc) {
            List<TResult> list = new List<TResult>();
            foreach (TSource obj in source) {
                list.Add(mapFunc(obj));
            }
            return list;
        }
        public static TResult Reduce<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult, TResult> reduceFunc) {
            var accumulator = default(TResult);
            foreach (var element in source) {
                accumulator = reduceFunc(element, accumulator);
            }
            return accumulator;
        }
        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value) where TSource : IComparable {
            foreach (var element in source) {
                if (element.Equals(value)) {
                    return true;
                }
            }
            return false;
        }
        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer) {
            return source.Any(element => comparer.Equals(element, value));
        }
        public static int Count<TSource>(this ICollection<TSource> source) {
            return source.Count;
        }
        public static int Count<TSource>(this IEnumerable<TSource> source) {
            if (source is ICollection) {
                return (source as ICollection).Count;
            }
            int count = 0;
            foreach (var element in source) {
                count++;
            }
            return count;
        }
        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector) {
            int count = 0;
            foreach (var element in source) {
                if (selector(element)) {
                    count++;
                }
            }
            return count;
        }
        public static List<TSource> Filter<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector) {
            var list = new List<TSource>();
            foreach (var element in source) {
                if (selector(element)) {
                    list.Add(element);
                }
            }
            return list;
        }
        public static List<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) {
            HashSet<TSource> hashSet = new HashSet<TSource>(second);
            List<TSource> ret = new List<TSource>();
            foreach (var element in first) {
                if (!hashSet.Contains(element)) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static List<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) {
            List<TSource> ret = new List<TSource>();
            foreach (var element in first) {
                if (!second.Contains(element, comparer)) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static TSource Max<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> converter) where TValue : IComparable {
            TSource select = source.First();
            TValue maxValue = converter(select);
            foreach (var element in source) {
                var value = converter(element);
                if (value.CompareTo(maxValue) > 0) {
                    select = element;
                    maxValue = value;
                }
            }
            return select;
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source) {
            if (source.IsEmpty()) {
                return new TSource[] { default(TSource) };
            }
            else {
                return source;
            }
        }
        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue) {
            if (source.IsEmpty()) {
                return new TSource[] { defaultValue };
            }
            else {
                return source;
            }
        }
        public static List<TSource> Distinct<TSource>(this IEnumerable<TSource> source) {
            List<TSource> ret = new List<TSource>();
            HashSet<TSource> hashSet = new HashSet<TSource>();
            foreach (var element in source) {
                if (!hashSet.Contains(element)) {
                    ret.Add(element);
                    hashSet.Add(element);
                }
            }
            return ret;
        }
        public static List<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer) {
            List<TSource> ret = new List<TSource>();
            foreach (var element in source) {
                if (!ret.Contains(element, comparer)) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index) {
            int currentIndex = 0;
            foreach (var element in source) {
                currentIndex++;
                if ((++currentIndex) > index) {
                    return element;
                }
            }
            throw new IndexOutOfRangeException();
        }
        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index) {
            int currentIndex = 0;
            foreach (var element in source) {
                currentIndex++;
                if ((++currentIndex) > index) {
                    return element;
                }
            }
            return default(TSource);
        }

        public static Dictionary<TKey, List<TValue>> Group<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) {
            var ret = new Dictionary<TKey, List<TValue>>();
            foreach (var pair in source) {
                ret.EnsureValue(pair.Key).Add(pair.Value);
            }
            return ret;
        }
        public static Dictionary<TKey, List<TValue>> Group<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, List<TValue>>(comparer);
            foreach (var pair in source) {
                ret.EnsureValue(pair.Key).Add(pair.Value);
            }
            return ret;
        }
        public static Dictionary<TKey, List<TValue>> GroupBy<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) {
            var ret = new Dictionary<TKey, List<TValue>>();
            foreach (var element in source) {
                ret.EnsureValue(keySelector(element)).Add(element);
            }
            return ret;
        }
        public static Dictionary<TKey, List<TValue>> GroupBy<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer) {

            var ret = new Dictionary<TKey, List<TValue>>();
            foreach (var element in source) {
                ret.EnsureValue(keySelector(element), comparer);
            }
            return ret;
        }
        public static List<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) where TKey:IComparable {

            var ret = new List<TResult>();
            foreach (var lhs in outer) {
                foreach (var rhs in inner) {
                    if (outerKeySelector(lhs).Equals(innerKeySelector(rhs))) {
                        ret.Add(resultSelector(lhs, rhs));
                    }
                }
            }
            return ret;
        }
        public static List<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer) {

            var ret = new List<TResult>();
            foreach (var lhs in outer) {
                foreach (var rhs in inner) {
                    if (comparer.Equals(outerKeySelector(lhs), innerKeySelector(rhs))) {
                        ret.Add(resultSelector(lhs, rhs));
                    }
                }
            }
            return ret;
        }
        public static Dictionary<TKey, List<TResult>> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) where TKey : IComparable {
            var results = Join(outer, inner, outerKeySelector, innerKeySelector, (lhs, rhs) => {
                return MakePair(outerKeySelector(lhs), resultSelector(lhs, rhs));
            });
            return results.Group<TKey, TResult>();
        }
        public static Dictionary<TKey, List<TResult>> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer) {
            var results = Join(outer, inner, outerKeySelector, innerKeySelector, (lhs, rhs) => {
                return MakePair(outerKeySelector(lhs), resultSelector(lhs, rhs));
            }, comparer);
            return results.Group();
        }
        public static List<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) where TSource : IComparable {
            var ret = new List<TSource>();
            foreach (var element in first) {
                if (second.Contains(element)) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static List<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) {
            var ret = new List<TSource>();
            foreach (var element in first) {
                if (second.Contains(element, comparer)) {
                    ret.Add(element);
                }
            }
            return ret;
        }
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) {
            var hashSet = new HashSet<TSource>(first);
            hashSet.AddRange(second);
            return hashSet.ToArray();
        }
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) {
            var hashSet = new HashSet<TSource>(first, comparer);
            hashSet.AddRange(second);
            return hashSet.ToArray();
        }
        public static List<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Int32, TResult> selector) {
            var ret = new List<TResult>();
            int index = 0;
            foreach (var element in source) {
                ret.Add(selector(element, index++));
            }
            return ret;
        }
        public static List<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            var ret = new List<TResult>();
            foreach (var element in source) {
                ret.Add(selector(element));
            }
            return ret;
        }
        public static List<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
            return source.Select(selector).Flatten();
        }
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) where TSource : IComparable {
            var enumerator1 = first.GetEnumerator();
            var enumerator2 = second.GetEnumerator();
            do {
                var flag1 = enumerator1.MoveNext();
                var flag2 = enumerator2.MoveNext();
                if (flag1 != flag2) {
                    return false;
                }
                else if (!flag1) {
                    return true;
                }
                else if (enumerator1.Current == null && enumerator2.Current != null) {
                    return false;
                }
                else if (enumerator1.Current != null && !enumerator1.Current.Equals(enumerator2.Current)) {
                    return false;
                }
            } while (true);
        }
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) {
            var enumerator1 = first.GetEnumerator();
            var enumerator2 = second.GetEnumerator();
            do {
                var flag1 = enumerator1.MoveNext();
                var flag2 = enumerator2.MoveNext();
                if (flag1 != flag2) {
                    return false;
                }
                else if (!flag1) {
                    return true;
                }
                else if (!comparer.Equals(enumerator1.Current, enumerator2.Current)) {
                    return false;
                }
            } while (true);
        }
        
        public static List<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable {
            var list = source.ToList();
            list.Sort((Comparison<TSource>)((lhs, rhs) => {
                return keySelector(lhs).CompareTo(keySelector(rhs));
            }));
            return list;
        }
        public static List<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) {
            var list = source.ToList();
            list.Sort((Comparison<TSource>)((lhs, rhs) => {
                return comparer.Compare(keySelector(lhs), keySelector(rhs));
            }));
            return list;
        }
        public static List<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable {
            var list = source.ToList();
            list = OrderBy(list, keySelector);
            list.Reverse();
            return list;
        }
        public static List<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) {
            var list = source.ToList();
            list = OrderBy(list, keySelector, comparer);
            list.Reverse();
            return list;
        }

        public static TValue EnsureValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue) {
            if (source.ContainsKey(key)) {
                return source[key];
            }
            else {
                source[key] = defaultValue;
                return defaultValue;
            }
        }
        public static TValue EnsureValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue, IEqualityComparer<TKey> comparer) {
            foreach (var pair in source) {
                if (comparer.Equals(pair.Key, key)) {
                    return pair.Value;
                }
            }
            return source.EnsureValue(key, defaultValue);
        }
        public static TValue EnsureValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key) where TValue : new() {
            return source.EnsureValue(key, new TValue());
        }
        public static TValue EnsureValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, IEqualityComparer<TKey> comparer) where TValue : new() {
            foreach (var pair in source) {
                if (comparer.Equals(pair.Key, key)) {
                    return pair.Value;
                }
            }
            return source.EnsureValue(key);
        }

        public static TKey[] Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) {
            List<TKey> ret = new List<TKey>();
            foreach (var keyValue in source) {
                ret.Add(keyValue.Key);
            }
            return ret.ToArray();
        }
        public static TValue[] Values<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source) {
            List<TValue> ret = new List<TValue>();
            foreach (var keyValue in source) {
                ret.Add(keyValue.Value);
            }
            return ret.ToArray();
        }

        public static KeyValuePair<TKey, TValue> MakePair<TKey, TValue>(TKey key, TValue value) {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> func) {
            foreach (var element in source) {
                func(element);
            }
        }

        public static TSource[] Slice<TSource>(IEnumerable<TSource> source, int from) {
            int count = source.Count();
            from = (from % count + count) % count;
            return source.Skip(from).ToArray();
        }
        public static TSource[] Slice<TSource>(IEnumerable<TSource> source, int from, int to) {
            int count = source.Count();
            from = (from % count + count) % count;
            to = (to % count + count) % count;
            if (from <= to) {
                return source.Skip(from).Take(to - from).ToArray();
            } 
            else {
                return source.Skip(from).Concat(source.Take(to)).ToArray();
            }
        }
        public static int[] Range(int from, int to, int delta = 1) {
            List<int> ret = new List<int>();
            for (int i = from; i != to; i += delta) {
                ret.Add(i);
            }
            return ret.ToArray();
        }
    }
}
