namespace Clicker {
    public struct Pair<Type1, Type2> {
        public Type1 First { get; set; }
        public Type2 Second { get; set; }

        public Pair(Type1 first, Type2 second) {
            First = first;
            Second = second;
        }
    }

    public static class Pair {
        public static Pair<Type1, Type2> Of<Type1, Type2>(Type1 first, Type2 second) {
            return new Pair<Type1, Type2>(first, second);
        }
    }
}
