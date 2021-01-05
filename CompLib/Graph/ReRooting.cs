namespace CompLib.Graph
{
    using System;
    using System.Collections.Generic;
    class ReRooting<T>
    {
        private readonly int _n;
        private readonly List<E>[] _edge;
        private int _ptr = 0;

        private Func<T, T, T> _f;
        private Func<T, int, int, T> _g;
        private Func<T, int, T> _h;
        private T _id;

        // 0が根。部分木iの値
        private T[] _dp;

        private T[] _result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">頂点の個数</param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="id"></param>
        public ReRooting(int n, Func<T, T, T> f, Func<T, int, int, T> g, Func<T, int, T> h, T id)
        {
            _n = n;

            _edge = new List<E>[_n];
            for (int i = 0; i < _n; i++)
            {
                _edge[i] = new List<E>();
            }

            _f = f;
            _g = g;
            _h = h;
            _id = id;
        }

        public int AddEdge(int u, int v)
        {
            _edge[u].Add(new E(v, _ptr));
            _edge[v].Add(new E(u, _ptr));
            return _ptr++;
        }

        public T[] Solve()
        {
            _dp = new T[_n];
            Go(0, -1);

            _result = new T[_n];
            Go2(0, -1, -1, _id);

            return _result;
        }

        private void Go(int cur, int par)
        {
            _dp[cur] = _id;
            foreach (var e in _edge[cur])
            {
                if (e.To == par) continue;
                Go(e.To, cur);
                _dp[cur] = _f(_dp[cur], _g(_dp[e.To], e.To, e.Num));
            }
            _dp[cur] = _h(_dp[cur], cur);
        }

        // 現在地、親、親へ向かう辺番号、親側の部分木の値
        private void Go2(int cur, int par, int edge, T p)
        {
            T l = par == -1 ? _id : _g(p, par, edge);
            int size = par == -1 ? _edge[cur].Count : _edge[cur].Count - 1;
            if (size > 0)
            {

                T[] r = new T[size];
                r[size - 1] = _id;
                int rPtr = size - 2;
                for (int i = _edge[cur].Count - 1; i >= 0 && rPtr >= 0; i--)
                {
                    var e = _edge[cur][i];
                    if (e.To == par) continue;


                    r[rPtr] = _f(r[rPtr + 1], _g(_dp[e.To], e.To, e.Num));
                    rPtr--;
                }


                int lPtr = 0;
                foreach (var e in _edge[cur])
                {
                    if (e.To == par) continue;
                    Go2(e.To, cur, e.Num, _f(l, r[lPtr]));
                    l = _f(l, _g(_dp[e.To], e.To, e.Num));
                    lPtr++;
                }
            }

            _result[cur] = par == -1 ? _dp[cur] : _h(l, cur);

        }

        private struct E
        {
            public readonly int Num;
            public readonly int To;
            public E(int to, int i)
            {
                To = to;
                Num = i;
            }
        }
    }
}