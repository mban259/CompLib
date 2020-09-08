namespace CompLib.Collections.Generic
{
    using System;
    using System.Diagnostics;

    public class SegmentTree<T>
    {
        // 見かけ上の大きさ、実際の大きさ
        private readonly int _n, _size;
        private T[] _array;

        private T _identity;
        private Func<T, T, T> _operation;

        public SegmentTree(int n, Func<T, T, T> operation, T identity)
        {
            _n = n;
            _size = 1;
            while (_size < _n)
            {
                _size *= 2;
            }

            _identity = identity;
            _operation = operation;
            _array = new T[_size * 2];
            for (int i = 1; i < _size * 2; i++)
            {
                _array[i] = _identity;
            }
        }

        public SegmentTree(T[] a, Func<T, T, T> operation, T identity)
        {
            _n = a.Length;
            while (_size < _n)
            {
                _size *= 2;
            }

            _identity = identity;
            _operation = operation;
            _array = new T[_size * 2];
            for (int i = 0; i < a.Length; i++)
            {
                _array[i + _size] = a[i];
            }
            for (int i = a.Length; i < _size; i++)
            {
                _array[i + _size] = identity;
            }

            for (int i = _size - 1; i >= 1; i--)
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
            Debug.Assert(0 <= i && i < _n);
            i += _size;
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
            Debug.Assert(0 <= left && left <= right && right <= _n);
            return Query(left, right, 1, 0, _size);
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
            Debug.Assert(0 <= l && l <= _n);
#if DEBUG
            Debug.Assert(f(_identity));
#endif
            if (l == _n) return _n;
            l += _size;
            T sm = _identity;
            do
            {
                while (l % 2 == 0) l >>= 1;
                if (!f(_operation(sm, _array[l])))
                {
                    while (l < _size)
                    {
                        l <<= 1;
                        if (f(_operation(sm, _array[l])))
                        {
                            sm = _operation(sm, _array[l]);
                            l++;
                        }
                    }
                    return l - _size;
                }
                sm = _operation(sm, _array[l]);
                l++;
            } while ((l & -l) != l);
            return _n;
        }
        /// <summary>
        /// f(op(a[l],a[l+1],...a[r-1])) = trueとなる最小のlを返します
        /// </summary>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        int MinLeft(int r, Func<T, bool> f)
        {
            Debug.Assert(0 <= r && r <= _n);
#if DEBUG
            Debug.Assert(f(_identity));
#endif
            if (r == 0) return 0;
            r += _size;
            T sm = _identity;

            do
            {
                r--;
                while (r > 1 && (r % 2 != 0)) r >>= 1;
                if (!f(_operation(_array[r], sm)))
                {
                    while (r < _size)
                    {
                        r = (2 * r + 1);
                        if (f(_operation(_array[r], sm)))
                        {
                            sm = _operation(_array[r], sm);
                            r--;
                        }
                    }
                    return r + 1 - _size;
                }
                sm = _operation(_array[r], sm);
            } while ((r & -r) != r);
            return 0;
        }

        public T this[int i]
        {
            set { Update(i, value); }
            get
            {
                Debug.Assert(0 <= i && i < _n);
                return _array[i + _n];
            }
        }
    }
}