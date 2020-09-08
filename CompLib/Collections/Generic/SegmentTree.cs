﻿namespace CompLib.Collections.Generic
{
    using System;
    using System.Diagnostics;

    public class SegmentTree<T>
    {
        private readonly int N;
        private T[] _array;

        private T _identity;
        private Func<T, T, T> _operation;

        public SegmentTree(int n, Func<T, T, T> operation, T identity)
        {
            N = 1;
            while (N < n)
            {
                N *= 2;
            }

            _identity = identity;
            _operation = operation;
            _array = new T[N * 2];
            for (int i = 1; i < N * 2; i++)
            {
                _array[i] = _identity;
            }
        }

        public SegmentTree(T[] a, Func<T, T, T> operation, T identity)
        {
            N = 1;
            while (N < a.Length)
            {
                N *= 2;
            }

            _identity = identity;
            _operation = operation;
            _array = new T[N * 2];
            for (int i = 0; i < a.Length; i++)
            {
                _array[i + N] = a[i];
            }
            for (int i = a.Length; i < N; i++)
            {
                _array[i + N] = identity;
            }

            for (int i = N - 1; i >= 1; i--)
            {
                _array[i] = operation(_array[i * 2], _array[i * 2 + 1]);
            }
        }

        /// <summary>
        /// A[i]をnに更新 O(log N)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        public void Update(int i, T n)
        {
            i += N;
            _array[i] = n;
            while (i > 1)
            {
                i /= 2;
                _array[i] = _operation(_array[i * 2], _array[i * 2 + 1]);
            }
        }

        private T Query(int left, int right, int k, int l, int r)
        {
            if (r <= left || right <= l)
            {
                return _identity;
            }

            if (left <= l && r <= right)
            {
                return _array[k];
            }

            return _operation(Query(left, right, k * 2, l, (l + r) / 2),
                Query(left, right, k * 2 + 1, (l + r) / 2, r));
        }

        /// <summary>
        /// A[left] op A[left+1] ... op A[right-1]を求める
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public T Query(int left, int right)
        {
            return Query(left, right, 1, 0, N);
        }

        /// <summary>
        /// op(a[0],a[1],...,a[n-1])を返します
        /// </summary>
        /// <returns></returns>
        public T All()
        {
            return _array[1];
        }

        /// <summary>
        /// f(op(a[l],a[l+1],...a[r-1])) = trueとなる最大のrを返します
        /// </summary>
        /// <param name="l"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        int MaxRight(int l, Func<T, bool> f)
        {
            Debug.Assert(0 <= l && l <= N);
#if DEBUG
            Debug.Assert(f(_identity));
#endif
            if (l == N) return N;
            l += N;
            T sm = _identity;
            do
            {
                while (l % 2 == 0) l >>= 1;
                if (!f(_operation(sm, _array[l])))
                {
                    while (l < N)
                    {
                        l <<= 1;
                        if (f(_operation(sm, _array[l])))
                        {
                            sm = _operation(sm, _array[l]);
                            l++;
                        }
                    }
                    return l - N;
                }
                sm = _operation(sm, _array[l]);
                l++;
            } while ((l & -l) != l);
            return N;
        }
        /// <summary>
        /// f(op(a[l],a[l+1],...a[r-1])) = trueとなる最小のlを返します
        /// </summary>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        int MinLeft(int r, Func<T, bool> f)
        {
            Debug.Assert(0 <= r && r <= N);
#if DEBUG
            Debug.Assert(f(_identity));
#endif
            if (r == 0) return 0;
            T sm = _identity;

            do
            {
                r--;
                while (r > 1 && (r % 2 != 0)) r >>= 1;
                if (!f(_operation(_array[r], sm)))
                {
                    while (r < N)
                    {
                        r = (2 * r + 1);
                        if (f(_operation(_array[r], sm)))
                        {
                            sm = _operation(_array[r], sm);
                            r--;
                        }
                    }
                    return r + 1 - N;
                }
                sm = _operation(_array[r], sm);
            } while ((r & -r) != r);
            return 0;
        }

        public T this[int i]
        {
            set { Update(i, value); }
            get { return _array[i + N]; }
        }
    }
}