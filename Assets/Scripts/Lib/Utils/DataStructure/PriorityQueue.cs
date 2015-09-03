using System;
using System.Collections.Generic;

namespace Utils {
    /// <summary>
    /// Priority queue based on heap
    /// </summary>
    public class PriorityQueue<T> where T : IComparable {
        Heap<T> _heap = new Heap<T>();

        public int Count { get { return _heap.Count; } }
        public bool IsEmpty { get { return Count == 0; } }

        public void Clear() { _heap.Clear(); }
        public T Top() { return _heap.Peek(); }
        public T Pop() {
            return _heap.Pop();
        }
        public void Push(T data) {
            _heap.Push(data);
        }
    }
}
