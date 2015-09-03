using System;
using System.Collections.Generic;

namespace Utils {
    /// <summary>
    /// A heap with object but not generics 
    /// </summary>
    public class Heap {
        List<IComparable> _data = new List<IComparable>();
        Dictionary<IComparable, int> _indexes = new Dictionary<IComparable, int>();

        public int Count { get { return _data.Count - 1; } }
        public bool IsEmpty { get { return Count <= 0; } }

        public Heap() {
            // Adds one default item to ensure index.
            _data.Add(default(IComparable));
        }

        public void Clear() {
            _data.Clear();
            // Ensures guard null at 0
            _data.Add(default(IComparable));
        }
        public IComparable Pop() {
            if (IsEmpty) {
                return default(IComparable);
            }

            var ret = _data[1];
            _data[1] = _data[Count];
            _data.RemoveAt(Count);

            if (!IsEmpty) {
                Down(1);
            }

            _indexes.Remove(ret);
            return ret;
        }
        public IComparable Peek() {
            if (IsEmpty) {
                return default(IComparable);
            }

            return _data[1];
        }
        public void Push(IComparable o) {
            _data.Add(o);
            _indexes[o] = Count;
            Up(Count);
        }
        public void Update(IComparable o) {
            var index = _indexes[o];
            if (index > 0) {
                Up(index);
                Down(index);
            }
        }
        public bool Contains(IComparable o) {
            return _indexes.ContainsKey(o);
        }

        void Down(int n) {
            IComparable o = _data[n];
            int p = n, q = p << 1;
            while (q <= Count) {
                if (q + 1 <= Count && _data[q + 1].CompareTo(_data[q]) < 0) {
                    q++;
                }
                if (o.CompareTo(_data[q]) < 0) {
                    break;
                }

                _data[p] = _data[q];
                _indexes[_data[q]] = p;
                p = q;
                q = q << 1;
            }
            _data[p] = o;
            _indexes[o] = p;
        }
        void Up(int n) {
            IComparable o = _data[n];
            int p = n, q = p >> 1;
            while (q > 0 && o.CompareTo(_data[q]) < 0) {
                _data[p] = _data[q];
                _indexes[_data[q]] = p;
                p = q;
                q = q >> 1;
            }
            _data[p] = o;
            _indexes[o] = p;
        }
    }

    /// <summary>
    /// A Heap with generic type
    /// </summary>
    public class Heap<T> where T : IComparable {
        List<T> _data = new List<T>();
        Dictionary<T, int> _indexes = new Dictionary<T, int>();

        public int Count { get { return _data.Count - 1; } }
        public bool IsEmpty { get { return Count <= 0; } }

        public Heap() {
            // Adds one default item to ensure index.
            _data.Add(default(T));
        }

        public void Clear() {
            _data.Clear();
            // Ensures guard null at 0
            _data.Add(default(T));
        }
        public T Pop() {
            if (IsEmpty) {
                return default(T);
            }

            var ret = _data[1];
            _data[1] = _data[Count];
            _data.RemoveAt(Count);

            if (!IsEmpty) {
                Down(1);
            }

            _indexes.Remove(ret);
            return ret;
        }
        public T Peek() {
            if (IsEmpty) {
                return default(T);
            }

            return _data[1];
        }
        public void Push(T o) {
            _data.Add(o);
            _indexes[o] = Count;
            Up(Count);
        }
        public void Update(T o) {
            var index = _indexes[o];
            if (index > 0) {
                Up(index);
                Down(index);
            }
        }
        public bool Contains(T o) {
            return _indexes.ContainsKey(o);
        }

        void Down(int n) {
            T o = _data[n];
            int p = n, q = p << 1;
            while (q <= Count) {
                if (q + 1 <= Count && _data[q + 1].CompareTo(_data[q]) < 0) {
                    q++;
                }
                if (o.CompareTo(_data[q]) < 0) {
                    break;
                }

                _data[p] = _data[q];
                _indexes[_data[q]] = p;
                p = q;
                q = q << 1;
            }
            _data[p] = o;
            _indexes[o] = p;
        }
        void Up(int n) {
            T o = _data[n];
            int p = n, q = p >> 1;
            while (q > 0 && o.CompareTo(_data[q]) < 0) {
                _data[p] = _data[q];
                _indexes[_data[q]] = p;
                p = q;
                q = q >> 1;
            }
            _data[p] = o;
            _indexes[o] = p;
        }
    }
}
