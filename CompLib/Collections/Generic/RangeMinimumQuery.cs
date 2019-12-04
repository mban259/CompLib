namespace CompLib.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    public class RangeMinimumQuery<T>
    {
        // 制約に合った2の冪
        private const int N = 1 << 21;
        private readonly T[] _array;
        private readonly Comparison<T> _comparison;
        private readonly T _inf;

        public RangeMinimumQuery(Comparison<T> comparison, T inf)
        {
            _comparison = comparison;
            _array = new T[N * 2];
            _inf = inf;
            for (int i = 1; i < N * 2; i++)
            {
                _array[i] = _inf;
            }
        }

        public RangeMinimumQuery(IComparer<T> comparer, T inf) : this(comparer.Compare, inf)
        {
        }

        public RangeMinimumQuery(T inf) : this(Comparer<T>.Default, inf)
        {
        }

        /// <summary>
        /// A[i]をitemに更新 O(log N)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        public void Update(int i, T item)
        {
            i += N;
            _array[i] = item;
            while (i > 1)
            {
                i /= 2;
                _array[i] = Min(_array[i * 2], _array[i * 2 + 1]);
            }
        }

        private T Minimum(int left, int right, int k, int l, int r)
        {
            if (r <= left || right <= l)
            {
                return _inf;
            }

            if (left <= l && r <= right)
            {
                return _array[k];
            }

            return Min(Minimum(left, right, k * 2, l, (l + r) / 2), Minimum(left, right, k * 2 + 1, (l + r) / 2, r));
        }

        /// <summary>
        /// [left,right)の最小値を求める
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public T Minimum(int left, int right)
        {
            return Minimum(left, right, 1, 0, N);
        }

        public T this[int i]
        {
            set { Update(i, value); }
            get { return _array[i + N]; }
        }

        private T Min(T a, T b)
        {
            return _comparison(a, b) < 0 ? a : b;
        }
    }
}