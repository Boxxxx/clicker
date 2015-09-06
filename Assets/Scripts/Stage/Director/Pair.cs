namespace Clicker {
    public struct Pair<Type1, Type2> {
        public Type1 First { get { return _first; } }
        public Type2 Second { get { return _second; } }

        private Type1 _first;
        private Type2 _second;

        public Pair(Type1 first, Type2 second) {
            _first = first;
            _second = second;
        }
    }

    public static class Pair {
        public static Pair<Type1, Type2> Of<Type1, Type2>(Type1 first, Type2 second) {
            return new Pair<Type1, Type2>(first, second);
        }
    }
}
