namespace CompLib.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class DynamicConnectivity
    {
        public readonly int N;
        // level 辺を見た回数


        // Fのlevelがi以上の辺の森
        private List<EulerTourTree> F;

        // levelがiのFに使われてない辺
        private List<HashSet<int>[]> E;

        public int Dep;

        public DynamicConnectivity(int n)
        {
            N = n;


            F = new List<EulerTourTree>() { new EulerTourTree(N) };

            E = new List<HashSet<int>[]>();
            E.Add(new HashSet<int>[N]);
            for (int i = 0; i < N; i++)
            {
                E[0][i] = new HashSet<int>();
            }

            Dep = 1;
            Count = N;
        }

        /// <summary>
        /// 辺(s,t)を追加
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public bool Connect(int s, int t)
        {
            if (s == t) return false;
            if (F[0].Connect(s, t))
            {
                Count--;
                return true;
            }
            else
            {
                if (E[0][s].Add(t) && E[0][s].Count == 1) F[0].EdgeConnectedUpdate(s, true);
                if (E[0][t].Add(s) && E[0][t].Count == 1) F[0].EdgeConnectedUpdate(t, true);
                return false;
            }
        }

        /// <summary>
        /// s,tが同じグループに属するか?
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Same(int s, int t)
        {
            return F[0].Same(s, t);
        }

        /// <summary>
        /// 辺(s,t)を削除
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Disconnect(int s, int t)
        {
            if (s == t) return false;
            for (int i = 0; i < Dep; i++)
            {
                if (E[i][s].Remove(t) && E[i][s].Count == 0) F[i].EdgeConnectedUpdate(s, false);
                if (E[i][t].Remove(s) && E[i][t].Count == 0) F[i].EdgeConnectedUpdate(t, false);
            }

            for (int i = Dep - 1; i >= 0; i--)
            {
                if (F[i].Disconnect(s, t))
                {
                    if (i == Dep - 1)
                    {
                        Dep++;
                        var ett = new EulerTourTree(N);
                        var adjList = new HashSet<int>[N];
                        for (int j = 0; j < N; j++) adjList[j] = new HashSet<int>();
                        F.Add(ett);
                        E.Add(adjList);
                    }

                    bool result = !Reconnect(s, t, i);
                    if (result) Count++;
                    return result;
                }
            }

            return false;
        }

        // F_kにおいて、(s,t)の代替となる辺をE_kから探してF_kに置き換える
        private bool Reconnect(int s, int t, int k)
        {
            for (int i = 0; i < k; i++) F[i].Disconnect(s, t);
            for (int i = k; i >= 0; i--)
            {
                if (F[i].Size(s) > F[i].Size(t))
                {
                    int tmp = s;
                    s = t;
                    t = tmp;
                }

                // sがいる木
                // levelが丁度iの辺のlevelを1つ上げる
                F[i].EdgeUpdate(s, (x, y) => F[i + 1].Connect(x, y));
                // 頂点xから生える辺たち
                // 代替にならない辺 見たのでlevel上げる
                // なる辺 Fに追加
                Func<int, bool> f = x =>
                {
                    var ls = new List<int>();
                    int yy = -1;
                    foreach (int y in E[i][x])
                    {
                        if (!F[i].Same(x, y))
                        {
                            yy = y;
                            break;
                        }

                        ls.Add(y);
                    }

                    foreach (int y in ls)
                    {
                        if (E[i][x].Remove(y) && E[i][x].Count == 0) F[i].EdgeConnectedUpdate(x, false);
                        if (E[i][y].Remove(x) && E[i][y].Count == 0) F[i].EdgeConnectedUpdate(y, false);
                        if (E[i + 1][x].Add(y) && E[i + 1][x].Count == 1) F[i + 1].EdgeConnectedUpdate(x, true);
                        if (E[i + 1][y].Add(x) && E[i + 1][y].Count == 1) F[i + 1].EdgeConnectedUpdate(y, true);
                    }

                    if (yy != -1)
                    {
                        if (E[i][x].Remove(yy) && E[i][x].Count == 0) F[i].EdgeConnectedUpdate(x, false);
                        if (E[i][yy].Remove(x) && E[i][yy].Count == 0) F[i].EdgeConnectedUpdate(yy, false);
                        for (int j = 0; j <= i; j++) F[j].Connect(x, yy);
                        return true;
                    }

                    return false;
                };
                if (F[i].Reconnect(s, f)) return true;
            }

            return false;
        }

        /// <summary>
        /// 頂点vがある連結成分のサイズ
        /// /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int Size(int v)
        {
            return F[0].Size(v);
        }

        /// <summary>
        /// vがいる連結成分の頂点
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int[] Group(int v)
        {
            return F[0].Group(v);
        }

        public int[][] Groups()
        {
            return F[0].Groups();
        }

        /// <summary>
        /// 連結成分の個数
        /// </summary>
        /// <value></value>
        public int Count { get; private set; }
    }


    public class EulerTourTree
    {
        public readonly int N;
        private Dictionary<int, SplayTreeNode>[] Ptr;

        public EulerTourTree(int n)
        {
            N = n;
            Ptr = new Dictionary<int, SplayTreeNode>[N];
            for (int i = 0; i < N; i++)
            {
                Ptr[i] = new Dictionary<int, SplayTreeNode>() { { i, new SplayTreeNode(i, i) } };
            }
        }

        // s,tが同じ木にあるか?
        public bool Same(int s, int t)
        {
            var nodeS = GetNode(s, s);
            var nodeT = GetNode(t, t);
            if (nodeS != null) Splay(nodeS);
            if (nodeT != null) Splay(nodeT);
            return Same(nodeS, nodeT);
        }


        // s,tをつなげる
        public bool Connect(int v, int u)
        {
            if (Same(v, u)) return false;
            Merge(Reroot(GetNode(v, v)), GetNode(v, u), Reroot(GetNode(u, u)), GetNode(u, v));
            return true;
        }

        // s,tを切り離す
        public bool Disconnect(int v, int u)
        {
            SplayTreeNode o;
            if (!Ptr[v].TryGetValue(u, out o)) return false;
            var triple = Split(GetNode(v, u), GetNode(u, v));
            Merge(triple.First, triple.Third);
            Ptr[u].Remove(v);
            Ptr[v].Remove(u);
            return true;
        }

        public int Size(int v)
        {
            SplayTreeNode o;
            if (!Ptr[v].TryGetValue(v, out o)) return 0;
            return Size(o);
        }

        private int Size(SplayTreeNode v)
        {
            return v.Size;
        }

        public int[] Group(int v)
        {
            var node = Reroot(GetNode(v, v));
            return NodeToArray(node);
        }

        public int[][] Groups()
        {
            List<int[]> result = new List<int[]>();
            foreach (Dictionary<int, SplayTreeNode> map in Ptr)
            {
                foreach (var pair in map)
                {
                    var v = pair.Value;
                    // root
                    if (v.Parent == null)
                    {
                        result.Add(NodeToArray(v));
                    }
                }
            }

            return result.ToArray();
        }

        public void EdgeConnectedUpdate(int v, bool b)
        {
            var s = GetNode(v, v);
            Splay(s);
            s.EdgeConnected = b;
            s.Update();
        }

        // 頂点vがいる木を全探索
        // levelが丁度iの辺のレベルを上げる
        public void EdgeUpdate(int v, Action<int, int> f)
        {
            var s = GetNode(v, v);
            Splay(s);
            while (s != null && s.ChildExact)
            {
                Go(s, f);
                Splay(s);
            }
        }

        private void Go(SplayTreeNode t, Action<int, int> f)
        {
            if (t.F < t.S && t.Exact)
            {
                Splay(t);
                t.Exact = false;
                t.Update();
                f(t.F, t.S);
                return;
            }

            if (t.Children[0] != null && t.Children[0].ChildExact) Go(t.Children[0], f);
            else Go(t.Children[1], f);
        }


        // vがいる部分木
        // E_iから辺を探す
        // あればつなげてtrueを返す
        public bool Reconnect(int v, Func<int, bool> f)
        {
            var s = GetNode(v, v);
            Splay(s);
            while (s.ChildEdgeConnected)
            {
                if (Go2(s, f)) return true;
                Splay(s);
            }

            return false;
        }

        private bool Go2(SplayTreeNode t, Func<int, bool> f)
        {
            Debug.Assert(t != null);
            if (t.EdgeConnected)
            {
                Splay(t);
                return f(t.S);
            }

            if (t.Children[0] != null && t.Children[0].ChildEdgeConnected) return Go2(t.Children[0], f);
            else return Go2(t.Children[1], f);
        }

        // オイラーツアーの始点をvにする
        private SplayTreeNode Reroot(SplayTreeNode v)
        {
            var pair = Split(v);
            return Merge(pair.Second, pair.First);
        }

        // 辺(f,t)のNodeを取得 無いなら作る
        private SplayTreeNode GetNode(int f, int t)
        {
            SplayTreeNode o;
            if (!Ptr[f].TryGetValue(t, out o))
            {
                o = new SplayTreeNode(f, t);
                Ptr[f][t] = o;
            }

            return o;
        }

        #region SplayTree

        private SplayTreeNode Merge(SplayTreeNode s, SplayTreeNode t)
        {
            if (s == null) return t;
            if (t == null) return s;
            // 右端まで行く
            while (s.Children[1] != null) s = s.Children[1];

            // sを根にする
            Splay(s);

            // sの右の子をtにする
            s.Children[1] = t;
            if (t != null) t.Parent = s;
            s.Update();
            return s;
        }

        // sにtたちをマージ
        private SplayTreeNode Merge(SplayTreeNode s, params SplayTreeNode[] t)
        {
            foreach (var v in t)
            {
                s = Merge(s, v);
            }

            return s;
        }

        // sがいる木をsより前,s以降(s含む)に分割
        private PairNode Split(SplayTreeNode s)
        {
            Splay(s);
            // node以前
            var t = s.Children[0];
            if (t != null) t.Parent = null;
            s.Children[0] = null;
            s.Update();
            return new PairNode(t, s);
        }

        // sがいる木をsより前、sより後ろに分割 sは含まない
        private PairNode Split2(SplayTreeNode s)
        {
            Splay(s);
            var t = s.Children[0];
            var u = s.Children[1];
            if (t != null) t.Parent = null;
            s.Children[0] = null;
            if (u != null) u.Parent = null;
            s.Children[1] = null;
            s.Update();
            return new PairNode(t, u);
        }

        // s,tがいる木をs,tを境目に3つに分割 s,tは含まない
        private TripleNode Split(SplayTreeNode s, SplayTreeNode t)
        {
            var u = Split2(s);
            if (Same(u.First, t))
            {
                // tは前にある
                var v = Split2(t);
                return new TripleNode(v.First, v.Second, u.Second);
            }
            else
            {
                // 後ろ
                var v = Split2(t);
                return new TripleNode(u.First, v.First, v.Second);
            }
        }

        // s,tが同じ木にいるか?
        private bool Same(SplayTreeNode s, SplayTreeNode t)
        {
            if (s != null) Splay(s);
            if (t != null) Splay(t);
            return Root(s) == Root(t);
        }

        // sがいる木の根
        private SplayTreeNode Root(SplayTreeNode s)
        {
            if (s == null) return s;
            while (s.Parent != null) s = s.Parent;
            return s;
        }

        // sを根にする
        private void Splay(SplayTreeNode s)
        {
            while (s.Parent != null)
            {
                var t = s.Parent;
                if (t.Parent == null)
                {
                    // 親が根なら

                    // sが左?右回転:左回転
                    Rotate(s, t.Children[0] == s ? 1 : 0);
                }
                else
                {
                    // 2つ上
                    var u = t.Parent;
                    int b = u.Children[0] == t ? 1 : 0;

                    // ジグザグ
                    if (t.Children[1 - b] == s)
                    {
                        Rotate(t, b);
                        Rotate(s, b);
                    }
                    else
                    {
                        // まっすぐ
                        Rotate(s, 1 - b);
                        Rotate(s, b);
                    }
                }
            }
        }

        // sを回転
        private void Rotate(SplayTreeNode s, int b)
        {
            // 1つ上
            var x = s.Parent;
            // 2つ上
            var y = x.Parent;
            if ((x.Children[1 - b] = s.Children[b]) != null) s.Children[b].Parent = x;
            s.Children[b] = x;
            x.Parent = s;
            x.Update();
            s.Update();
            if ((s.Parent = y) != null)
            {
                if (y.Children[0] == x) y.Children[0] = s;
                if (y.Children[1] == x) y.Children[1] = s;
                y.Update();
            }
        }

        // sがいる木のF==Sを満すnodeを返す
        private int[] NodeToArray(SplayTreeNode s)
        {
            Splay(s);
            int[] ar = new int[s.Size];
            int k = 0;
            Go(s, ar, ref k);
            return ar;
        }

        private void Go(SplayTreeNode s, int[] ar, ref int k)
        {
            if (s.Size == 0) return;
            if (s.Children[0] != null) Go(s.Children[0], ar, ref k);
            if (s.F == s.S) ar[k++] = s.F;
            if (s.Children[1] != null) Go(s.Children[1], ar, ref k);
        }

        public class SplayTreeNode
        {
            public int Size;
            public SplayTreeNode[] Children;
            public SplayTreeNode Parent;

            // Eにある辺と繋がっている頂点か?
            public bool EdgeConnected;

            // 部分木にEdgeConnected = trueがあるか?
            public bool ChildEdgeConnected;

            // 一度も見てない辺か? (F_iにいるlevel=iの辺か?)
            public bool Exact;

            // 部分木にExact=trueがあるか?
            public bool ChildExact;


            public int F, S;

            public SplayTreeNode(int f, int s)
            {
                F = f;
                S = s;
                Size = F == S ? 1 : 0;
                Children = new SplayTreeNode[2] { null, null };
                Parent = null;


                EdgeConnected = false;

                ChildEdgeConnected = false;

                Exact = F < S;


                ChildExact = Exact;
            }

            public void Update()
            {
                ChildEdgeConnected = EdgeConnected;
                ChildExact = Exact;
                Size = (F == S ? 1 : 0);

                if (Children[0] != null)
                {
                    ChildEdgeConnected |= Children[0].ChildEdgeConnected;
                    ChildExact |= Children[0].ChildExact;
                    Size += Children[0].Size;
                }

                if (Children[1] != null)
                {
                    ChildEdgeConnected |= Children[1].ChildEdgeConnected;
                    ChildExact |= Children[1].ChildExact;
                    Size += Children[1].Size;
                }
            }
        }

        #endregion
    }

    public struct TripleNode
    {
        public readonly EulerTourTree.SplayTreeNode First, Second, Third;

        public TripleNode(EulerTourTree.SplayTreeNode f, EulerTourTree.SplayTreeNode s, EulerTourTree.SplayTreeNode t)
        {
            First = f;
            Second = s;
            Third = t;
        }
    }

    public struct PairNode
    {
        public readonly EulerTourTree.SplayTreeNode First, Second;

        public PairNode(EulerTourTree.SplayTreeNode f, EulerTourTree.SplayTreeNode s)
        {
            First = f;
            Second = s;
        }
    }
}
