using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    public static class Dictionarys {
        public static IDictionary<TKey, TValue> Assign<TKey, TValue>(this IDictionary<TKey, TValue> source, params object[] args)
            where TKey : class
            where TValue : class {
            if ((args.Length & 1) != 0) {
                throw new Exception("Util Error: Requires an even number of arguments!");
            }
            int paramNum = args.Length >> 1;
            for (int i = 0; i < paramNum; i++) {
                source[args[i << 1] as TKey] = args[(i << 1) + 1] as TValue;
            }
            return source;
        }
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> source) {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var pair in source) {
                dict[pair.Key] = pair.Value;
            }
            return dict;
        }

        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(params object[] args)
            where TKey : class
            where TValue : class {

            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            return dict.Assign(args) as Dictionary<TKey, TValue>;
        }
        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) {
            return new Dictionary<TKey, TValue>(dictionary);
        }
        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
            return new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source) {
            var ret = new Dictionary<TKey, TValue>();
            foreach (var pair in source) {
                ret[pair.Key] = pair.Value;
            }
            return ret;
        }
        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TValue>(comparer);
            foreach (var pair in source) {
                ret[pair.Key] = pair.Value;
            }
            return ret;
        }
        public static Dictionary<TKey, TSource> NewDictionary<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            var ret = new Dictionary<TKey, TSource>();
            foreach (var element in source) {
                ret[keySelector(element)] = element;
            }
            return ret;
        }
        public static Dictionary<TKey, TSource> NewDictionary<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TSource>(comparer);
            foreach (var element in source) {
                ret[keySelector(element)] = element;
            }
            return ret;
        }
        public static Dictionary<TKey, TElement> NewDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {
            var ret = new Dictionary<TKey, TElement>();
            foreach (var element in source) {
                ret[keySelector(element)] = elementSelector(element);
            }
            return ret;
        }
        public static Dictionary<TKey, TElement> NewDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TElement>(comparer);
            foreach (var element in source) {
                ret[keySelector(element)] = elementSelector(element);
            }
            return ret;
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            var ret = new Dictionary<TKey, TSource>();
            foreach (var element in source) {
                ret[keySelector(element)] = element;
            }
            return ret;
        }
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TSource>(comparer);
            foreach (var element in source) {
                ret[keySelector(element)] = element;
            }
            return ret;
        }
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {
            var ret = new Dictionary<TKey, TElement>();
            foreach (var element in source) {
                ret[keySelector(element)] = elementSelector(element);
            }
            return ret;
        }
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TElement>(comparer);
            foreach (var element in source) {
                ret[keySelector(element)] = elementSelector(element);
            }
            return ret;
        }
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source) {
            var ret = new Dictionary<TKey, TValue>();
            foreach (var pair in source) {
                ret[pair.Key] = pair.Value;
            }
            return ret;
        }
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer) {
            var ret = new Dictionary<TKey, TValue>(comparer);
            foreach (var pair in source) {
                ret[pair.Key] = pair.Value;
            }
            return ret;
        }

        public static Dictionary<TKey, TResult> TransformValues<TKey, TSource, TResult>(Dictionary<TKey, TSource> source, Func<TSource, TResult> convertor) {
            var ret = new Dictionary<TKey, TResult>();
            foreach (var pair in source) {
                ret[pair.Key] = convertor(pair.Value);
            }
            return ret;
        }
        public static Dictionary<TKey, TResult> TransformEntities<TKey, TSource, TResult>(Dictionary<TKey, TSource> source, Func<TKey, TSource, TResult> convertor) {
            var ret = new Dictionary<TKey, TResult>();
            foreach (var pair in source) {
                ret[pair.Key] = convertor(pair.Key, pair.Value);
            }
            return ret;
        }
    }
}