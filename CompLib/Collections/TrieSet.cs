namespace CompLib.Collections
{
    using Num = System.Int32;
    using System.Collections.Generic;
    using System;

    class TrieSet
    {
        private int Depth = 30;
        private List<int> _left, _right, _cnt;
        public bool IsMultiSet = false;

        private int[] _stack;

        public TrieSet()
        {
            _left = new List<int>() { -1 };
            _right = new List<int>() { -1 };
            _cnt = new List<int>() { 0 };
            _stack = new int[Depth];
        }


        public bool Add(Num val)
        {
            int cur = 0;
            for (int i = Depth - 1; i >= 0; i--)
            {
                List<int> next;
                if ((val & ((Num)1 << i)) == 0)
                {
                    next = _left;
                }
                else
                {
                    next = _right;
                }

                if (next[cur] == -1)
                {
                    next[cur] = next.Count;
                    _left.Add(-1);
                    _right.Add(-1);
                    _cnt.Add(0);
                }

                _stack[i] = cur;
                cur = next[cur];
            }

            if (!IsMultiSet && _cnt[cur] > 0) return false;
            _cnt[cur] += 1;

            for (int i = 0; i < Depth; i++)
            {
                cur = _stack[i];
                _cnt[cur] = (_left[cur] == -1 ? 0 : _cnt[_left[cur]]) + (_right[cur] == -1 ? 0 : _cnt[_right[cur]]);
            }

            return true;
        }

        public Num Get(int idx, int xor)
        {
            if (idx >= Count) throw new IndexOutOfRangeException();
            Num val = 0;
            int cur = 0;
            for (int i = Depth - 1; i >= 0; i--)
            {
                List<int> left, right;
                if ((xor & (1L << i)) == 0)
                {
                    (left, right) = (_left, _right);
                }
                else
                {
                    (left, right) = (_right, _left);
                }

                if (left[cur] != -1 && idx < _cnt[left[cur]])
                {
                    cur = left[cur];
                }
                else
                {
                    idx -= (left[cur] == -1 ? 0 : _cnt[left[cur]]);
                    val |= ((Num)1 << i);
                    cur = right[cur];
                }
            }

            return val;
        }

        private int Count
        {
            get => _cnt[0];
        }
    }
}
