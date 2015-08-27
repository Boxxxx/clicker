using System;
using System.Collections;

namespace Utils {
    public class Asserts {
        public static void Equals<T>(T a, T b, string errMsg = "Assert failed") {
            if (!a.Equals(b)) {
                throw new Exception(errMsg);
            }
        }
        public static void Assert(bool flag, string errMsg = "Assert failed!") {
            if (!flag) {
                throw new Exception(errMsg);
            }
        }
    }
}