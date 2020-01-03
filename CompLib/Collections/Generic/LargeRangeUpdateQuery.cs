namespace CompLib.Collections.Generic
{
    public class LargeRangeUpdateQuery<T>
    {
        private Set<Segment> _set;

        public LargeRangeUpdateQuery()
        {
            _set = new Set<Segment>((a, b) => a.Right.CompareTo(b.Right));
            // 番兵
            _set.Add(new Segment(long.MaxValue, default(T)));
        }

        // [l,r)をitemに更新
        public void Update(long l, long r, T item)
        {
            int left = _set.UpperBound(new Segment(l, default(T)));
            int right = _set.UpperBound(new Segment(r, default(T)));

            var ll = _set[left];
            Segment a = new Segment(l, ll.Value);
            Segment b = new Segment(r, item);

            // [left,right)を消す            
            _set.RemoveRange(left, right);
            _set.Add(b);
            // set[left-1].R == a.Rなら setなので追加されない
            _set.Add(a);
        }

        public T Get(long l)
        {
            int i = GetSegment(l);
            return GetBySegment(i);
        }

        public T GetBySegment(int i)
        {
            return _set[i].Value;
        }

        public int GetSegment(long l)
        {
            return _set.UpperBound(new Segment(l, default(T)));
        }

        public T this[long l]
        {
            get { return Get(l); }
            set { Update(l, l + 1, value); }
        }

        struct Segment
        {
            public readonly long Right;
            public readonly T Value;

            public Segment(long right, T v)
            {
                Right = right;
                Value = v;
            }
        }
    }
}