namespace CompLib.Collections.Generic
{
    using System.Diagnostics;
    using System.Linq;

    public class Deque<T>
    {
        private int _offset, _mask;
        private T[] _buffer;
        public Deque() : this(16) { }

        public Deque(int capacity)
        {
            Debug.Assert(capacity > 0);
            Debug.Assert((capacity & (capacity - 1)) == 0);
            _mask = capacity - 1;
            _offset = 0;
            _buffer = new T[capacity];
            Count = 0;
        }

        /// <summary>
        /// 末尾に要素追加 O(1)
        /// </summary>
        /// <param name="item"></param>
        public void PushBack(T item)
        {
            if (Count == _buffer.Length) Extend();
            _buffer[(_offset + Count++) & _mask] = item;
        }

        /// <summary>
        /// 末尾の要素を削除し、返す O(1)
        /// </summary>
        /// <returns></returns>
        public T PopBack()
        {
            Debug.Assert(Count > 0);
            return _buffer[(_offset + --Count) & _mask];
        }

        /// <summary>
        /// 先頭に要素追加 O(1)
        /// </summary>
        /// <param name="item"></param>
        public void PushFront(T item)
        {
            if (Count == _buffer.Length) Extend();
            Count++;
            _buffer[(--_offset) & _mask] = item;
        }

        /// <summary>
        /// 先頭の要素を削除し、返す O(1)
        /// </summary>
        /// <returns></returns>
        public T PopFront()
        {
            Debug.Assert(Count > 0);
            Count--;
            return _buffer[(_offset++) & _mask];
        }

        private void Extend()
        {
            int capacity = _buffer.Length;
            Debug.Assert(Count == capacity);
            T[] tmpBuf = new T[2 * capacity];
            for (int i = 0; i < capacity; i++)
            {
                tmpBuf[i] = _buffer[(_offset + i) & _mask];
            }
            _mask = 2 * capacity - 1;
            _buffer = tmpBuf;
            _offset = 0;
        }

        public int Count { get; private set; }

        public T First
        {
            get
            {
                Debug.Assert(Count > 0);
                return _buffer[_offset & _mask];
            }
        }
        public T Last
        {
            get
            {
                Debug.Assert(Count > 0);
                return _buffer[(_offset + Count - 1) & _mask];
            }
        }

        public T this[int i]
        {
            get
            {
                Debug.Assert(0 <= i && i < Count);
                return _buffer[(_offset + i) & _mask];
            }
            set
            {
                Debug.Assert(0 <= i && i < Count);
                _buffer[(_offset + i) & _mask] = value;
            }
        }

        public void Clear() { Count = 0; }

        public void Insert(int index, T item)
        {
            Debug.Assert(0 <= index && index <= Count);
            if (Count == _buffer.Length) Extend();
            if (Count - index - 1 <= index)
            {
                for (int i = Count - 1; i >= index; i--)
                {
                    _buffer[(_offset + i + 1) & _mask] = _buffer[(_offset + i) & _mask];
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    _buffer[(_offset + i - 1) & _mask] = _buffer[(_offset + i) & _mask];
                }
                _offset--;
            }
            _buffer[(_offset + index) & _mask] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            Debug.Assert(0 <= index && index < Count);

            if (Count - index - 1 <= index)
            {
                for (int i = index + 1; i < Count; i++)
                {
                    _buffer[(_offset + i - 1) & _mask] = _buffer[(_offset + i) & _mask];
                }
            }
            else
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    _buffer[(_offset + i + 1) & _mask] = _buffer[(_offset + i) & _mask];
                }
                _offset++;
            }
            Count--;
        }

        public T[] ToArray()
        {
            T[] array = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = _buffer[(_offset + i) & _mask];
            }
            return array;
        }
    }
}