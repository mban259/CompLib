namespace CompLib.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    public class Set<T>
    {
        private readonly Comparison<T> _comparison;
        private readonly bool _multiset;

        private Node _root;
        private readonly Random _rnd;

        public Set(Comparison<T> comparison, bool multiset = false)
        {
            _comparison = comparison;
            _multiset = multiset;
            _rnd = new Random();
        }

        public Set(IComparer<T> comparer, bool multiset = false) : this(comparer.Compare, multiset)
        {
        }

        public Set(bool multiset = false) : this(Comparer<T>.Default, multiset)
        {
        }

        /// <summary>
        /// item以上の値を探索
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int LowerBound(T item)
        {
            Node p = _root;
            int k = 0;
            while (true)
            {
                if ((p?.Count ?? 0) == 0) return k;
                if (_comparison(item, p.Value) <= 0)
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
        /// item強の値を探索
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpperBound(T item)
        {
            Node p = _root;
            int k = 0;
            while (true)
            {
                if ((p?.Count ?? 0) == 0) return k;
                if (_comparison(item, p.Value) < 0)
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

        // tが根の部分木のi番目
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
            else // if (i > left)
            {
                return Find(t.Right, i - left - 1);
            }
        }

        /// <summary>
        /// i番目の要素を取得
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T Get(int i)
        {
            var node = Find(_root, i);
            return node.Value;
        }

        // tをx未満left x以上rightに分割
        private bool Split(Node t, T x, ref Node left, ref Node right)
        {
            if (t == null)
            {
                left = null;
                right = null;
                return true;
            }

            int comp = _comparison(x, t.Value);
            if (comp < 0)
            {
                if (Split(t.Left, x, ref left, ref t.Left))
                {
                    right = t;
                    left?.Update();
                    right.Update();
                    return true;
                }
            }
            else if (_multiset || comp > 0)
            {
                if (Split(t.Right, x, ref t.Right, ref right))
                {
                    left = t;
                    left.Update();
                    right?.Update();
                    return true;
                }
            }

            return false;
        }

        private bool Insert(ref Node t, Node item)
        {
            bool result;
            if (t == null)
            {
                t = item;
                result = true;
            }
            else if (item.Rank > t.Rank)
            {
                result = Split(t, item.Value, ref item.Left, ref item.Right);
                if (result)
                {
                    t = item;
                }
            }
            else
            {
                int comp = _comparison(item.Value, t.Value);
                if (comp < 0)
                {
                    result = Insert(ref t.Left, item);
                }
                else
                {
                    if (!_multiset && comp == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = Insert(ref t.Right, item);
                    }
                }
            }

            t.Update();
            return result;
        }

        /// <summary>
        /// itemを追加
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            var node = new Node(item, _rnd.Next());
            return Insert(ref _root, node);
        }

        // l,rをマージしてtに入れる 
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

        private bool Remove(ref Node t, T item)
        {
            if (t == null)
            {
                return false;
            }

            int comp = _comparison(item, t.Value);
            if (comp == 0)
            {
                Merge(out t, t.Left, t.Right);
                return true;
            }
            else
            {
                bool f = comp < 0 ? Remove(ref t.Left, item) : Remove(ref t.Right, item);
                t.Update();
                return f;
            }
        }

        public bool Remove(T item)
        {
            return Remove(ref _root, item);
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
        /// [left,right)を削除
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void RemoveRange(int left, int right)
        {
            RemoveRange(ref _root, left, right);
        }

        /// <summary>
        /// i番目を削除
        /// </summary>
        /// <param name="i"></param>
        public void RemoveAt(int i)
        {
            RemoveRange(i, i + 1);
        }

        // nにitemがあるか
        private bool Contains(out Node n, T item)
        {
            n = _root;
            while (true)
            {
                if (n == null)
                {
                    return false;
                }

                int comp = _comparison(item, n.Value);
                if (comp < 0)
                {
                    n = n.Left;
                }
                else
                {
                    if (comp == 0)
                    {
                        return true;
                    }

                    n = n.Right;
                }
            }
        }

        public bool Contains(T item)
        {
            Node n;
            return Contains(out n, item);
        }

        public int Count(T item)
        {
            return UpperBound(item) - LowerBound(item);
        }

        public int Count()
        {
            return _root?.Count ?? 0;
        }

        public T[] ToArray()
        {
            var res = new T[Count()];
            int k = 0;
            Walk(_root, res, ref k);
            return res;
        }

        private void Walk(Node p, T[] ar, ref int k)
        {
            if (p == null) return;
            Walk(p.Left, ar, ref k);
            ar[k++] = p.Value;
            Walk(p.Right, ar, ref k);
        }

        public T this[int i]
        {
            get { return Get(i); }
        }

        private class Node
        {
            public readonly int Rank;
            public readonly T Value;
            public int Count { get; private set; }
            public Node Left, Right;

            public int LeftCount()
            {
                return Left?.Count ?? 0;
            }

            public void Update()
            {
                Count = LeftCount() + 1 + (Right?.Count ?? 0);
            }

            public Node(T item, int rank)
            {
                Rank = rank;
                Value = item;
                Update();
            }
        }
    }
}