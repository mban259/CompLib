using CompLib.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CompLib.Mathematics
{
    class TwoSAT
    {
        private readonly int _n;
        private readonly bool[] _answer;
        private readonly SCC scc;

        public TwoSAT(int n)
        {
            _n = n;
            _answer = new bool[_n];
            scc = new SCC(2 * _n);
        }

        /// <summary>
        /// 条件(クロージャ) (i = f) ∨ (j = g)を追加します
        /// </summary>
        /// <param name="i"></param>
        /// <param name="f"></param>
        /// <param name="j"></param>
        /// <param name="g"></param>
        public void AddClause(int i, bool f, int j, bool g)
        {
            Debug.Assert(0 <= i && i < _n);
            Debug.Assert(0 <= j && j < _n);
            scc.AddEdge(2 * i + (f ? 0 : 1), 2 * j + (g ? 1 : 0));
            scc.AddEdge(2 * j + (g ? 0 : 1), 2 * i + (f ? 1 : 0));
        }

        /// <summary>
        /// 条件を満たすリテラルが存在するか?
        /// </summary>
        public bool Satisfiable()
        {
            scc.Execute();
            for (int i = 0; i < _n; i++)
            {
                if (scc.GetId(2 * i) == scc.GetId(2 * i + 1)) return false;
                _answer[i] = scc.GetId(2 * i) < scc.GetId(2 * i + 1);
            }
            return true;
        }
        /// <summary>
        /// 条件を満たすリテラルの一例を返す
        /// </summary>
        /// <remarks>
        /// Satisfiable() してtrueを返したら呼んでください
        /// </remarks>
        /// <returns></returns>
        public bool[] Answer()
        {
            return _answer;
        }
    }
}
