namespace CompLib.Collections
{
    using Num = System.Int64;

    public class RangeSumQuery
    {
        //制約に合った2の冪
        private const int N = 1 << 21;
        private readonly Num[] _array;

        public RangeSumQuery()
        {
            _array = new Num[N * 2];
        }

        /// <summary>
        /// A[i]をnに更新 O(log N)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        public void Update(int i, Num n)
        {
            i += N;
            _array[i] = n;
            while (i > 1)
            {
                i /= 2;
                _array[i] = _array[i * 2] + _array[i * 2 + 1];
            }
        }

        private Num Sum(int left, int right, int k, int l, int r)
        {
            if (right <= l || r <= left)
            {
                return 0;
            }

            if (left <= l && r <= right)
            {
                return _array[k];
            }

            return Sum(left, right, k * 2, l, (l + r) / 2) + Sum(left, right, k * 2 + 1, (l + r) / 2, r);
        }

        /// <summary>
        /// A[left], A[left+1] ... ,A[right-1]の総和を求める O(log N)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Num Sum(int left, int right)
        {
            return Sum(left, right, 1, 0, N);
        }

        public Num this[int i]
        {
            set { Update(i, value); }
            get { return _array[i + N]; }
        }
    }
}