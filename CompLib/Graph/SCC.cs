using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CompLib.Graph
{
    class SCC
    {
        private readonly int _n;
        private readonly List<(int f, int t)> _edges;

        // start[i+1] - start[i] iから生える辺の本数
        // eList[start[i]] ~ eList[start[i+1]-1]がiから生える辺の行き先 
        private int[] _start;
        private int[] _eList;


        // 行きがけ順現在地, 強連結成分の個数
        private int _nowOrd, _groupNum;
        List<int> _visited;

        // iから行ける頂点のordの最小値 ,行きがけ順, iが含まれるトポロジカル順序
        int[] _low, _ord, _ids;

        public SCC(int n)
        {
            _n = n;
            _edges = new List<(int f, int t)>();
        }

        /// <summary>
        /// fromからtoへ有向辺を追加します
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddEdge(int from, int to)
        {
            Debug.Assert(0 <= from && from < _n);
            Debug.Assert(0 <= to && to < _n);
            _edges.Add((from, to));
        }

        /// <summary>
        /// 強連結成分分解して、トポロジカル順序で返します
        /// </summary>
        /// <returns></returns>
        public List<int>[] Execute()
        {
            Build();
            CalcId();

            // ACLだとvec.reserveしてるけどいる?
            List<int>[] groups = new List<int>[_groupNum];
            for (int i = 0; i < _groupNum; i++)
            {
                groups[i] = new List<int>();
            }

            for (int i = 0; i < _n; i++)
            {
                groups[_ids[i]].Add(i);
            }

            return groups;
        }

        void CalcId()
        {
            _nowOrd = 0;
            _groupNum = 0;
            _visited = new List<int>(_n);
            _low = new int[_n];
            _ord = new int[_n];

            // CodeforcesだとArrayにFill()が無い
            for (int i = 0; i < _n; i++)
            {
                _ord[i] = -1;
            }
            _ids = new int[_n];

            for (int i = 0; i < _n; i++)
            {
                if (_ord[i] == -1) Go(i);
            }

            for (int i = 0; i < _n; i++)
            {
                _ids[i] = _groupNum - 1 - _ids[i];
            }
        }

        void Go(int v)
        {
            _low[v] = _ord[v] = _nowOrd++;
            _visited.Add(v);
            for (int i = _start[v]; i < _start[v + 1]; i++)
            {
                int to = _eList[i];
                if (_ord[to] == -1)
                {
                    Go(to);
                    _low[v] = Math.Min(_low[v], _low[to]);
                }
                else
                {
                    _low[v] = Math.Min(_low[v], _ord[to]);
                }
            }

            if (_low[v] == _ord[v])
            {
                while (true)
                {
                    int u = _visited[_visited.Count - 1];
                    _visited.RemoveAt(_visited.Count - 1);
                    _ord[u] = _n;
                    _ids[u] = _groupNum;
                    if (u == v) break;
                }
                _groupNum++;
            }
        }

        private void Build()
        {
            _start = new int[_n + 1];
            _eList = new int[_edges.Count];

            foreach (var e in _edges)
            {
                _start[e.f + 1]++;
            }

            for (int i = 1; i <= _n; i++)
            {
                _start[i] += _start[i - 1];
            }

            var counter = new int[_n + 1];
            Array.Copy(_start, counter, _n + 1);

            foreach (var e in _edges)
            {
                _eList[counter[e.f]++] = e.t;
            }
        }
    }
}
