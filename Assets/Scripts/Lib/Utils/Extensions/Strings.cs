using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    public static class Strings {
        public static bool EndsWith(string baseStr, string suffix) {
            if (suffix.Length > baseStr.Length) {
                return false;
            }
            int start = baseStr.Length - suffix.Length;
            for (int i = 0; i < suffix.Length; i++) {
                if (baseStr[start + i] != suffix[i]) {
                    return false;
                }
            }
            return true;
        }

        public static bool StartsWith(string baseStr, string prefix) {
            if (prefix.Length > baseStr.Length) {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++) {
                if (baseStr[i] != prefix[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}
