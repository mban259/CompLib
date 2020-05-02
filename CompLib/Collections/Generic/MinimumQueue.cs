namespace CompLib.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 最小値を持てるキュー
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MinimumQueue<T>
    {
        private readonly Comparison<T> _compare;
        // 前、後ろ
        private readonly MinimumStack<T> _s1, _s2;

        public MinimumQueue(Comparison<T> comparison)
        {
            _compare = comparison;
            _s1 = new MinimumStack<T>(comparison);
            _s2 = new MinimumStack<T>(comparison);
        }

        public MinimumQueue(IComparer<T> comparer) : this(comparer.Compare) { }

        public MinimumQueue() : this(Comparer<T>.Default) { }

        /// <summary>
        /// itemを末尾に追加
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            _s2.Push(item);
        }

        /// <summary>
        /// 先頭の要素を取り出す
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (_s1.Count == 0) Move();
            return _s1.Pop();
        }

        /// <summary>
        /// 先頭の値
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (_s1.Count == 0) Move();
            return _s1.Peek();
        }

        // s2の要素を反転してs1に入れる
        // s1が空の時
        private void Move()
        {
            while (_s2.Count > 0)
            {
                _s1.Push(_s2.Pop());
            }
        }

        /// <summary>
        /// 要素の最小値
        /// </summary>
        /// <returns></returns>
        public T Min()
        {
            if (_s1.Count == 0) return _s2.Min();
            if (_s2.Count == 0) return _s1.Min();
            return _Min(_s1.Min(), _s2.Min());
        }

        private T _Min(T x, T y)
        {
            return _compare(x, y) <= 0 ? x : y;
        }

        public int Count => _s1.Count + _s2.Count;
    }
}