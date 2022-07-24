using System;
using System.Collections.Generic;
using System.Text;

namespace CompLib.Graph
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HLD
    {
        protected readonly int _n;
        protected readonly Tree _tree;

        protected int[] _weight;
        protected int[] _depth;
        protected int[] _parent;

        private int[] _path;
        private int[] _begin;
        private int _ptrPath;
        private int _ptrBegin;

        private (int I, int J)[] _index;

        public int Count { get; private set; }

        public HLD(int n)
        {
            _n = n;
            _tree = new Tree(_n);
        }

        public void AddEdge(int u, int v)
        {
            _tree.AddEdge(u, v);
        }

        public void Build(int root = 0)
        {
            Debug.Assert(0 <= root && root < _n);
            _tree.Build();

            _weight = new int[_n];
            _depth = new int[_n];
            _parent = new int[_n];
            Count = 0;
            Go1(root, -1);

            _path = new int[_n];
            _begin = new int[Count + 1];
            _ptrPath = 0;
            _ptrBegin = 0;
            _begin[_ptrBegin++] = 0;
            Go2(root, -1);
            _begin[_ptrBegin] = _n;

            _index = new (int I, int J)[_n];
            for (int i = 0; i < Count; i++)
            {
                var span = this[i];
                for (int j = 0; j < span.Length; j++)
                {
                    _index[span[j]] = (i, j);
                }
            }
        }

        private void Go1(int cur, int par)
        {
            _weight[cur] = 1;
            if (_tree[cur].Length == 1 && _tree[cur][0] == par) Count++;
            foreach (int to in _tree[cur])
            {
                if (to == par) continue;
                _depth[to] = _depth[cur] + 1;
                _parent[to] = cur;
                Go1(to, cur);
                _weight[cur] += _weight[to];
            }
        }

        private void Go2(int cur, int par)
        {
            _path[_ptrPath++] = cur;

            int w = int.MinValue;
            int heaviest = -1;
            foreach (int to in _tree[cur])
            {
                if (to == par) continue;

                if (w < _weight[to])
                {
                    w = _weight[to];
                    heaviest = to;
                }
            }

            if (heaviest == -1) return;

            Go2(heaviest, cur);

            foreach (int to in _tree[cur])
            {
                if (to == par || to == heaviest) continue;
                _begin[_ptrBegin++] = _ptrPath;
                Go2(to, cur);
            }
        }

        public ReadOnlySpan<int> this[int i] => _path.AsSpan(_begin[i], _begin[i + 1] - _begin[i]);

        public (int I, int J) Search(int v)
        {
            Debug.Assert(0 <= v && v < _n);
            return _index[v];
        }
    }

    public class HLDVertexSegTree<T>
    {
        private readonly int _n;
        private readonly Func<T, T, T> _op;
        private readonly T _id;

        private readonly HLD _hld;



        public HLDVertexSegTree(int n, Func<T, T, T> op, T id)
        {
            _n = n;
            _op = op;
            _id = id;

            _hld = new HLD(_n);
        }

        public void AddEdge(int u, int v)
        {
            _hld.AddEdge(u, v);
        }

        public void Build()
        {
            _hld.Build();
        }

        public void Set(int v, T item)
        {

        }



        public T this[int i]
        {
            get
            {

            }
            set
            {
                Set(i, value);
            }
        }
    }

    public class Tree
    {
        private readonly int _n;
        private readonly int[] _u, _v;
        private int _ptr;

        private int[] _graph;
        private int[] _begin;

        public Tree(int n)
        {
            _n = n;
            _u = new int[_n - 1];
            _v = new int[_n - 1];
            _ptr = 0;
        }

        public void AddEdge(int u, int v)
        {
            Debug.Assert(0 <= u && u < _n);
            Debug.Assert(0 <= v && v < _n);
            Debug.Assert(u != v);
            _u[_ptr] = u;
            _v[_ptr] = v;
            _ptr++;
        }

        public void Build()
        {
            Debug.Assert(_ptr == _n - 1);
            _graph = new int[2 * (_n - 1)];
            _begin = new int[_n + 1];

            int[] degree = new int[_n];
            for (int i = 0; i < _n - 1; i++)
            {
                degree[_u[i]]++;
                degree[_v[i]]++;
            }

            _begin[0] = 0;
            for (int i = 0; i < _n; i++)
            {
                _begin[i + 1] = _begin[i] + degree[i];
            }

            int[] counter = new int[_n];
            for (int i = 0; i < _n - 1; i++)
            {
                _graph[_begin[_u[i]] + counter[_u[i]]++] = _v[i];
                _graph[_begin[_v[i]] + counter[_v[i]]++] = _u[i];
            }
        }

        public ReadOnlySpan<int> this[int v] => _graph.AsSpan(_begin[v], _begin[v + 1] - _begin[v]);
    }
}
