using System;
using System.Collections;

namespace Utils {
    public static class Maths {
        public const float Epsilon = 1.4013e-045f;

        public static T Clamp<T>(T source, T min, T max) where T : IComparable {
            return Max(min, Min(max, source));
        }
        public static T Min<T>(T a, T b) where T : IComparable {
            return a.CompareTo(b) < 0 ? a : b;
        }
        public static T Max<T>(T a, T b) where T : IComparable {
            return a.CompareTo(b) > 0 ? a : b;
        }
        public static bool IsZero(float a) {
            return Math.Abs(a) < Epsilon;
        }
        public static bool NotZero(float a) {
            return Math.Abs(a) > Epsilon;
        }
        public static bool Equals(float a, float b) {
            return Math.Abs(a - b) < Epsilon;
        }
        public static bool Greater(float a, float b) {
            return a > b + Epsilon;
        }
        public static bool GreaterOrEqual(float a, float b) {
            return a >= b - Epsilon;
        }
        public static bool Less(float a, float b) {
            return a < b - Epsilon;
        }
        public static bool LessOrEqual(float a, float b) {
            return a <= b + Epsilon;
        }
        public static bool AbsGreater(float a, float b) {
            return Math.Abs(a) > Math.Abs(b) + Epsilon;
        }
        public static bool AbsGreaterOrEqual(float a, float b) {
            return Math.Abs(a) >= Math.Abs(b) - Epsilon;
        }
        public static bool AbsLess(float a, float b) {
            return Math.Abs(a) < Math.Abs(b) - Epsilon;
        }
        public static bool AbsLessOrEqual(float a, float b) {
            return Math.Abs(a) <= Math.Abs(b) + Epsilon;
        }
        public static float Sign(float val) {
            if (IsZero(val)) {
                return 0;
            }
            else {
                return Math.Sign(val);
            }
        }
        public static float Zeroization(float val) {
            if (Math.Abs(val) < Epsilon) {
                return 0;
            }
            return val;
        }
    }
}
