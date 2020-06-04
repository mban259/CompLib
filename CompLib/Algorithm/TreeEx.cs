using System.Collections.Generic;

namespace CompLib.Algorithm
{
    /// <summary>
    /// Treeに機能追加
    /// </summary>
    public class TreeEx : Tree
    {
        private readonly List<int>[] _ancestor;

        public TreeEx(List<int>[] tree, int root = 0) : base(tree, root)
        {
            _ancestor = new List<int>[_size];
            for (int i = 0; i < _size; i++)
            {
                _ancestor[i] = new List<int>();
            }

            var q = new Queue<int>();
            q.Enqueue(_root);
            while (q.Count > 0)
            {
                var d = q.Dequeue();
                if (Depth(d) > 0)
                {
                    _ancestor[d].Add(Parent(d));
                    for (int i = 0; i < _ancestor[_ancestor[d][i]].Count; i++)
                    {
                        _ancestor[d].Add(_ancestor[_ancestor[d][i]][i]);
                    }
                }

                foreach (int i in _child[d])
                {
                    q.Enqueue(i);
                }
            }
        }

        /// <summary>
        /// 頂点nのi世代上
        /// </summary>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int Ancestor(int n, int i)
        {
            if (i == 0)
            {
                return n;
            }

            for (int t = 0;; t++)
            {
                if (((1 << t) & i) > 0)
                {
                    return Ancestor(_ancestor[n][t], i - (1 << t));
                }
            }
        }

        public int LowestCommonAncestor(int a, int b)
        {
            if (Depth(b) < Depth(a))
            {
                return LowestCommonAncestor(b, a);
            }

            // depth(a) <= depth(b)
            b = Ancestor(b, Depth(b) - Depth(a));
            if (a == b) return a;

            int ok = -1;
            int ng = _ancestor[a].Count;
            while (ng - ok > 1)
            {
                int med = (ok + ng) / 2;
                if (_ancestor[a][med] == _ancestor[b][med])
                {
                    ng = med;
                }
                else
                {
                    ok = med;
                }
            }

            if (ok == -1)
            {
                return Parent(a);
            }

            return LowestCommonAncestor(_ancestor[a][ok], _ancestor[b][ok]);
        }

        public int Dist(int a, int b)
        {
            return Depth(a) + Depth(b) - 2 * Depth(LowestCommonAncestor(a, b));
        }
    }
}