using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    public static class Util {
        public static KeyValuePair<K, V> MakePair<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}