namespace CompLib.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    // IEnumeratorいるかな?
    public class Map<TKey, TValue>
    {
        // keyで見ると2分探索木
        // rankで見るとheap
        // Treap (tree + heap)

        private Node _root;

        private Comparison<TKey> _comparison;

        private Random _random;

        /// <summary>
        /// 連想配列
        /// </summary>
        /// <param name="comparison">keyの比較 狭義弱順序</param>
        public Map(Comparison<TKey> comparison)
        {
            _random = new Random();
            _comparison = comparison;
        }

        /// <summary>
        /// 連想配列
        /// </summary>
        /// <param name="comparison">keyの比較 狭義弱順序</param>
        public Map(IComparer<TKey> comparer) : this(comparer.Compare)
        {
        }

        
        /// <summary>
        /// 連想配列
        /// </summary>
        public Map() : this(Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// キーがx以上の要素を探索
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int LowerBound(TKey x)
        {
            Node p = _root;
            int k = 0;
            while (true)
            {
                if ((p?.Count ?? 0) == 0) return k;
                if (_comparison(x, p.Key) <= 0)
                {
                    p = p.Left;
                }
                else
                {
                    k += p.LeftCount() + 1;
                    p = p.Right;
                }
            }
        }

        /// <summary>
        /// キーがx強の要素を探索
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int UpperBound(TKey x)
        {
            Node p = _root;
            int k = 0;
            while (true)
            {
                if ((p?.Count ?? 0) == 0) return k;
                if (_comparison(x, p.Key) < 0)
                {
                    p = p.Left;
                }
                else
                {
                    k += p.LeftCount() + 1;
                    p = p.Right;
                }
            }
        }

        private Node Find(Node t, int i)
        {
            int left = t.LeftCount();
            if (i < left)
            {
                return Find(t.Left, i);
            }
            else if (i == left)
            {
                return t;
            }
            else // i > left
            {
                return Find(t.Right, i - left - 1);
            }
        }

        /// <summary>
        /// i番目の要素を取得
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public KeyValuePair<TKey, TValue> Get(int i)
        {
            var node = Find(_root, i);
            return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        }

        /// <summary>
        /// tをx未満のleft,x以上のrightに分割
        /// </summary>
        /// <param name="t"></param>
        /// <param name="x"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void Split(Node t, TKey x, out Node left, out Node right)
        {
            if (t == null)
            {
                left = null;
                right = null;
            }
            else if (_comparison(x, t.Key) < 0)
            {
                Split(t.Left, x, out left, out t.Left);
                right = t;
            }
            else
            {
                Split(t.Right, x, out t.Right, out right);
                left = t;
            }

            left?.Update();
            right?.Update();
        }


        // tにitemを追加
        private void Insert(ref Node t, Node item)
        {
            if (t == null)
            {
                t = item;
            }
            else if (item.Rank > t.Rank)
            {
                Split(t, item.Key, out item.Left, out item.Right);
                t = item;
            }
            else
            {
                if (_comparison(item.Key, t.Key) < 0)
                {
                    Insert(ref t.Left, item);
                }
                else
                {
                    Insert(ref t.Right, item);
                }
            }

            t.Update();
        }

        /// <summary>
        /// キー keyにvalueを追加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                return false;
            }

            var node = new Node(key, value, _random.Next());
            Insert(ref _root, node);
            return true;
        }

        /// <summary>
        /// キー keyの要素をvalueに更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(TKey key, TValue value)
        {
            Node n;
            if (ContainsKey(key, out n))
            {
                n.Value = value;
            }
            else
            {
                var node = new Node(key, value, _random.Next());
                Insert(ref _root, node);
            }
        }

        // l,rをマージしてtに入れる l.key < r.key
        private void Merge(out Node t, Node l, Node r)
        {
            if (l == null) t = r;
            else if (r == null) t = l;
            else if (l.Rank > r.Rank)
            {
                Merge(out l.Right, l.Right, r);
                t = l;
            }
            else
            {
                Merge(out r.Left, l, r.Left);
                t = r;
            }

            t?.Update();
        }

        private bool Remove(ref Node t, TKey key)
        {
            if (t == null)
            {
                return false;
            }

            if(_comparison(key,t.Key) == 0)
            {
                Merge(out t, t.Left, t.Right);
                return true;
            }
            else
            {
                bool f = _comparison(key, t.Key) < 0 ? Remove(ref t.Left, key) : Remove(ref t.Right, key);
                t.Update();
                return f;
            }
        }

        /// <summary>
        /// キーがkeyの要素を削除して削除できたならtrue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return Remove(ref _root, key);
        }

        // 根がtの部分木の[left,right)を削除
        private void RemoveRange(ref Node t, int left, int right)
        {
            if (t == null || right - left == 0) return;
            if (left <= t.LeftCount() && t.LeftCount() < right)
            {
                if (left == 0 && t.Count <= right)
                {
                    t = null;
                    return;
                }

                // t.Left.Countが変わるのでRightが先
                RemoveRange(ref t.Right, 0, right - t.LeftCount() - 1);
                RemoveRange(ref t.Left, left, t.LeftCount());
                Merge(out t, t.Left, t.Right);
            }
            else if (right <= t.LeftCount())
            {
                RemoveRange(ref t.Left, left, right);
            }
            else
            {
                RemoveRange(ref t.Right, left - t.LeftCount() - 1, right - t.LeftCount() - 1);
            }

            t?.Update();
        }

        /// <summary>
        /// [begin, end)を削除
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void RemoveRange(int begin, int end)
        {
            RemoveRange(ref _root, Math.Max(0, begin), Math.Min(Count, end));
        }

        /// <summary>
        /// i番目を削除
        /// </summary>
        /// <param name="i"></param>
        public void RemoveAt(int i)
        {
            RemoveRange(i, i + 1);
        }

        // キーがkeyのノードがあるか
        private bool ContainsKey(TKey key, out Node n)
        {
            n = _root;
            while (true)
            {
                if (n == null)
                {
                    return false;
                }

                if (_comparison(key, n.Key) < 0)
                {
                    n = n.Left;
                }
                else
                {
                    if(_comparison(key,n.Key) == 0)
                    {
                        return true;
                    }

                    n = n.Right;
                }
            }
        }

        /// <summary>
        /// キーがkeyの要素があるか
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            Node _;
            return ContainsKey(key, out _);
        }

        public TValue this[TKey key]
        {
            set { Update(key, value); }
            get
            {
                Node v;
                return ContainsKey(key, out v) ? v.Value : default(TValue);
            }
        }

        /// <summary>
        /// 要素の個数
        /// </summary>
        public int Count
        {
            get { return _root?.Count ?? 0; }
        }

        private class Node
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public int Rank { get; set; }
            public Node Left;
            public Node Right;

            public int Count { get; private set; }

            // leftの部分木の大きさ
            public int LeftCount()
            {
                return Left?.Count ?? 0;
            }

            public void Update()
            {
                Count = LeftCount() + 1 + (Right?.Count ?? 0);
            }

            public Node(TKey key, TValue value, int rank)
            {
                Key = key;
                Value = value;
                Rank = rank;
                Left = null;
                Right = null;
            }
        }
    }
}