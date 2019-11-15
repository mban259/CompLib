namespace CompLib.Collections
{
    #region RangeSumQuery

    using Num = System.Int64;

    /// <summary>
    /// 遅延セグメント木 (RangeSumQuery)
    /// 指定した範囲の和を求める、更新 それぞれO(log N)
    /// </summary>
    public class LazySegmentTreeRsq
    {
        private readonly int _size;
        private readonly Num[] _array;
        private readonly Num[] _add;
        private readonly Num[] _update;
        private readonly bool[] _updateFlag;

        /// <summary>
        /// コンストラクタ
        /// 全て0で初期化
        /// </summary>
        /// <param name="size">配列の大きさ = N</param>
        public LazySegmentTreeRsq(int size)
        {
            _size = 1;
            while (_size < size)
                _size *= 2;
            _array = new Num[_size * 2];
            _add = new Num[_size * 2];
            _update = new Num[_size * 2];
            _updateFlag = new bool[_size * 2];
        }

        /// <summary>
        /// コンストラクタ
        /// 配列aをコピー
        /// </summary>
        /// <param name="a">配列</param>
        public LazySegmentTreeRsq(Num[] a) : this(a.Length)
        {
            for (int i = 0; i < a.Length; i++)
                _array[i + _size] = a[i];
            for (int i = _size - 1; i >= 1; i--)
                _array[i] = _array[i * 2] + _array[i * 2 + 1];
        }

        private void Eval(int l, int r, int k)
        {
            if (_updateFlag[k])
            {
                _array[k] = _update[k] * (r - l);
                if (r - l > 1)
                {
                    _update[k * 2] = _update[k];
                    _update[k * 2 + 1] = _update[k];
                    _add[k * 2] = 0;
                    _add[k * 2 + 1] = 0;
                    _updateFlag[k * 2] = true;
                    _updateFlag[k * 2 + 1] = true;
                }

                _updateFlag[k] = false;
            }

            if (_add[k] != 0)
            {
                _array[k] += _add[k] * (r - l);
                if (r - l > 1)
                {
                    _add[k * 2] += _add[k];
                    _add[k * 2 + 1] += _add[k];
                }

                _add[k] = 0;
            }
        }

        private void Add(int left, int right, int k, int l, int r, Num n)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return;
            if (left <= l && r <= right)
            {
                _add[k] += n;
                Eval(l, r, k);
            }
            else
            {
                Add(left, right, k * 2, l, (l + r) / 2, n);
                Add(left, right, k * 2 + 1, (l + r) / 2, r, n);
                _array[k] = _array[k * 2] + _array[k * 2 + 1];
            }
        }

        /// <summary>
        /// [left,right)にnを足す O(log N)
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="right">右端</param>
        /// <param name="n">増やす値</param>
        public void Add(int left, int right, Num n) => Add(left, right, 1, 0, _size, n);

        /// <summary>
        /// i番目にnを足す O(log N)
        /// </summary>
        /// <param name="i">インデックス</param>
        /// <param name="n">増やす値</param>
        public void Add(int i, Num n) => Add(i, i + 1, n);

        private void Update(int left, int right, int k, int l, int r, Num n)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return;
            if (left <= l && r <= right)
            {
                _update[k] = n;
                _updateFlag[k] = true;
                _add[k] = 0;
                Eval(l, r, k);
            }
            else
            {
                Update(left, right, k * 2, l, (l + r) / 2, n);
                Update(left, right, k * 2 + 1, (l + r) / 2, r, n);
                _array[k] = _array[k * 2] + _array[k * 2 + 1];
            }
        }

        /// <summary>
        /// [left, right)をnに置き換える
        /// </summary>
        /// <param name="left">右端</param>
        /// <param name="right">左端</param>
        /// <param name="n">置き換える値</param>
        public void Update(int left, int right, Num n) => Update(left, right, 1, 0, _size, n);

        /// <summary>
        /// i番目をnに置き換える
        /// </summary>
        /// <param name="i">インデックス</param>
        /// <param name="n">置き換える値</param>
        public void Update(int i, Num n) => Update(i, i + 1, n);

        private Num Sum(int left, int right, int k, int l, int r)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return 0;
            if (left <= l && r <= right) return _array[k];
            return Sum(left, right, k * 2, l, (l + r) / 2) + Sum(left, right, k * 2 + 1, (l + r) / 2, r);
        }

        /// <summary>
        /// [left, right)の和を求める O(log N)
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="right">右端</param>
        /// <returns></returns>
        public Num Sum(int left, int right) => Sum(left, right, 1, 0, _size);

        public Num this[int i]
        {
            get { return Sum(i, i + 1); }
            set { Update(i, value); }
        }
    }

    #endregion

    #region RangeMinimumQuery

    /// <summary>
    /// 遅延セグメント木 (RangeMinimumQuery)
    /// 指定した範囲の最小値を求める、値の更新 O(log N)
    /// </summary>
    public class LazySegmentTreeRmq<T>
    {
        private readonly int _size;
        private readonly T[] _array;
        private readonly T[] _tmp;
        private readonly bool[] _flag;

        private readonly System.Comparison<T> _compare;
        private readonly T _inf;

        public LazySegmentTreeRmq(int size, System.Comparison<T> comparison, T inf)
        {
            _inf = inf;
            _compare = comparison;
            _size = 1;
            while (_size < size)
            {
                _size *= 2;
            }

            _array = new T[_size * 2];
            _tmp = new T[_size * 2];
            _flag = new bool[_size * 2];

            for (int i = 1; i < _size * 2; i++)
            {
                _array[i] = _inf;
            }
        }

        public LazySegmentTreeRmq(int size, System.Collections.Generic.IComparer<T> comparer, T inf) : this(size,
            comparer.Compare, inf)
        {
        }

        public LazySegmentTreeRmq(int size, T inf) : this(size, System.Collections.Generic.Comparer<T>.Default, inf)
        {
        }

        public LazySegmentTreeRmq(T[] a, System.Comparison<T> comparison, T inf) : this(a.Length, comparison, inf)
        {
            for (int i = 0; i < a.Length; i++)
                _array[i + _size] = a[i];
            for (int i = _size - 1; i >= 1; i--)
                _array[i] = Min(_array[i * 2], _array[i * 2 + 1]);
        }

        public LazySegmentTreeRmq(T[] a, System.Collections.Generic.IComparer<T> comparer, T inf) : this(a,
            comparer.Compare, inf)
        {
        }

        public LazySegmentTreeRmq(T[] a, T inf) : this(a, System.Collections.Generic.Comparer<T>.Default, inf)
        {
        }

        private void Eval(int l, int r, int k)
        {
            if (!_flag[k]) return;
            if (r - l > 1)
            {
                _tmp[k * 2] = _tmp[k];
                _tmp[k * 2 + 1] = _tmp[k];
                _flag[k * 2] = true;
                _flag[k * 2 + 1] = true;
            }

            _array[k] = _tmp[k];
            _flag[k] = false;
        }

        private void Update(int left, int right, int k, int l, int r, T item)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return;
            if (left <= l && r <= right)
            {
                _tmp[k] = item;
                _flag[k] = true;
                Eval(l, r, k);
            }
            else
            {
                Update(left, right, k * 2, l, (l + r) / 2, item);
                Update(left, right, k * 2 + 1, (l + r) / 2, r, item);
                _array[k] = Min(_array[k * 2], _array[k * 2 + 1]);
            }
        }


        /// <summary>
        /// [left, right)をitemに更新 O(log N)
        /// </summary>
        /// <param name="left">右端</param>
        /// <param name="right">左端</param>
        /// <param name="item">更新する値</param>
        public void Update(int left, int right, T item) => Update(left, right, 1, 0, _size, item);

        /// <summary>
        /// i番目をitemに更新 O(log N)
        /// </summary>
        /// <param name="i">インデックス</param>
        /// <param name="item">更新する値</param>
        public void Update(int i, T item) => Update(i, i + 1, item);

        private T Minimum(int left, int right, int k, int l, int r)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return _inf;
            if (left <= l && r <= right) return _array[k];
            return Min(Minimum(left, right, k * 2, l, (l + r) / 2), Minimum(left, right, k * 2 + 1, (l + r) / 2, r));
        }

        /// <summary>
        /// [left, right)の最小値を求める O(log N)
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="right">右端</param>
        /// <returns></returns>
        public T Minimum(int left, int right) => Minimum(left, right, 1, 0, _size);

        private T Min(T x, T y)
        {
            return _compare(x, y) <= 0 ? x : y;
        }

        public T this[int i]
        {
            get { return Minimum(i, i + 1); }
            set { Update(i, value); }
        }
    }

    
    /// <summary>
    /// 遅延セグメント木 (RangeMinimumQuery)
    /// 指定した範囲の最小値を求める、値の更新 O(log N) デフォルトは0
    /// </summary>
    public class LazySegmentTreeRmq
    {
        private readonly int _size;
        private readonly Num[] _array;
        private readonly Num[] _add;
        private readonly Num[] _update;
        private readonly bool[] _updateFlag;

        private readonly System.Comparison<Num> _compare;
        private readonly Num _inf;

        public LazySegmentTreeRmq(int size, System.Comparison<Num> comparison, Num inf)
        {
            _inf = inf;
            _compare = comparison;
            _size = 1;
            while (_size < size)
            {
                _size *= 2;
            }

            _array = new Num[_size * 2];
            _add = new Num[_size * 2];
            _update = new Num[_size * 2];
            _updateFlag = new bool[_size * 2];

            for (int i = 0; i < size; i++)
            {
                _array[i + _size] = 0;
            }

            for (int i = size; i < _size; i++)
            {
                _array[i + _size] = inf;
            }

            for (int i = _size - 1; i >= 1; i--)
            {
                _array[i] = Min(_array[i * 2], _array[i * 2 + 1]);
            }
        }

        public LazySegmentTreeRmq(int size, System.Collections.Generic.IComparer<Num> comparer, Num inf) : this(size,
            comparer.Compare, inf)
        {
        }

        public LazySegmentTreeRmq(int size) : this(size, System.Collections.Generic.Comparer<Num>.Default, Num.MaxValue)
        {
        }

        public LazySegmentTreeRmq(Num[] a, System.Comparison<Num> comparison, Num inf) : this(a.Length, comparison, inf)
        {
            for (int i = 0; i < a.Length; i++)
                _array[i + _size] = a[i];
            for (int i = _size - 1; i >= 1; i--)
                _array[i] = Min(_array[i * 2], _array[i * 2 + 1]);
        }

        public LazySegmentTreeRmq(Num[] a, System.Collections.Generic.IComparer<Num> comparer, Num inf) : this(a,
            comparer.Compare, inf)
        {
        }

        public LazySegmentTreeRmq(Num[] a) : this(a, System.Collections.Generic.Comparer<Num>.Default, Num.MaxValue)
        {
        }

        private void Eval(int l, int r, int k)
        {
            if (_updateFlag[k])
            {
                _array[k] = _update[k];
                if (r - l > 1)
                {
                    _update[k * 2] = _update[k];
                    _update[k * 2 + 1] = _update[k];
                    _add[k * 2] = 0;
                    _add[k * 2 + 1] = 0;
                    _updateFlag[k * 2] = true;
                    _updateFlag[k * 2 + 1] = true;
                }

                _updateFlag[k] = false;
            }

            if (_add[k] != 0)
            {
                _array[k] += _add[k];
                if (r - l > 1)
                {
                    _add[k * 2] += _add[k];
                    _add[k * 2 + 1] += _add[k];
                }

                _add[k] = 0;
            }
        }

        private void Add(int left, int right, int k, int l, int r, Num n)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return;
            if (left <= l && r <= right)
            {
                _add[k] += n;
                Eval(l, r, k);
            }
            else
            {
                Add(left, right, k * 2, l, (l + r) / 2, n);
                Add(left, right, k * 2 + 1, (l + r) / 2, r, n);
                _array[k] = Min(_array[k * 2], _array[k * 2 + 1]);
            }
        }


        /// <summary>
        /// [left, right)にnを足す O(log N)
        /// </summary>
        /// <param name="left">右端</param>
        /// <param name="right">左端</param>
        /// <param name="n">更新する値</param>
        public void Add(int left, int right, Num n) => Add(left, right, 1, 0, _size, n);

        /// <summary>
        /// i番目にnを足す O(log N)
        /// </summary>
        /// <param name="i">インデックス</param>
        /// <param name="n">値</param>
        public void Add(int i, Num n) => Add(i, i + 1, n);

        private void Update(int left, int right, int k, int l, int r, Num n)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return;
            if (left <= l && r <= right)
            {
                _update[k] = n;
                _updateFlag[k] = true;
                _add[k] = 0;
                Eval(l, r, k);
            }
            else
            {
                Update(left, right, k * 2, l, (l + r) / 2, n);
                Update(left, right, k * 2 + 1, (l + r) / 2, r, n);
                _array[k] = Min(_array[k * 2], _array[k * 2 + 1]);
            }
        }


        /// <summary>
        /// [left, right)をnに更新 O(log N)
        /// </summary>
        /// <param name="left">右端</param>
        /// <param name="right">左端</param>
        /// <param name="n">更新する値</param>
        public void Update(int left, int right, Num n) => Update(left, right, 1, 0, _size, n);

        /// <summary>
        /// i番目をnに更新 O(log N)
        /// </summary>
        /// <param name="i">インデックス</param>
        /// <param name="n">更新する値</param>
        public void Update(int i, Num n) => Update(i, i + 1, n);

        private Num Minimum(int left, int right, int k, int l, int r)
        {
            Eval(l, r, k);
            if (r <= left || right <= l) return _inf;
            if (left <= l && r <= right) return _array[k];
            return Min(Minimum(left, right, k * 2, l, (l + r) / 2), Minimum(left, right, k * 2 + 1, (l + r) / 2, r));
        }

        /// <summary>
        /// [left, right)の最小値を求める O(log N)
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="right">右端</param>
        /// <returns></returns>
        public Num Minimum(int left, int right) => Minimum(left, right, 1, 0, _size);

        private Num Min(Num x, Num y)
        {
            return _compare(x, y) <= 0 ? x : y;
        }

        public Num this[int i]
        {
            get { return Minimum(i, i + 1); }
            set { Update(i, value); }
        }
    }

    #endregion
}