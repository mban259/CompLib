namespace CompLib.Graph.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class DynamicConnectivity<T>
    {
        public readonly int N;
        // level 辺を見た回数


        // Fのlevelがi以上の辺の森
        private List<IETT> F;
        private EulerTourTree<T> F0;

        // levelがiのFに使われてない辺
        private List<HashSet<int>[]> E;

        public int Dep;

        private readonly T[] Ar;
        private readonly CommutativeMonoid<T> Monoid;

        public DynamicConnectivity(int n, T[] ar, Func<T, T, T> op, T id)
        {
            N = n;

            Ar = new T[N];
            for (int i = 0; i < N; i++) Ar[i] = ar[i];
            Monoid = new CommutativeMonoid<T>(op, id);
            F0 = new EulerTourTree<T>(N, Ar, Monoid);
            F = new List<IETT>() { F0 };
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
        /// 頂点vの値をvalueに変える
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        public void Update(int v, T value)
        {
            Ar[v] = value;
            for (int i = 0; i < Dep; i++)
            {
                F0.Update(v, value);
            }
        }

        /// <summary>
        /// vがいる連結成分の総和
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public T Sum(int v)
        {
            return F0.Sum(v);
        }

        /// <summary>
        /// 連結成分の個数
        /// </summary>
        /// <value></value>
        public int Count { get; private set; }

        public T this[int i]
        {
            get
            {
                return Ar[i];
            }
            set
            {
                Update(i, value);
            }
        }
    }

    #region ETT

    public interface IETT
    {
        bool Same(int s, int t);
        bool Connect(int s, int t);
        bool Disconnect(int s, int t);
        int Size(int v);
        int[] Group(int v);
        int[][] Groups();
        void EdgeConnectedUpdate(int v, bool b);
        void EdgeUpdate(int v, Action<int, int> f);
        bool Reconnect(int v, Func<int, bool> f);
    }
    public abstract class AbsETT<T> : IETT where T : AbsNode<T>
    {
        protected readonly int N;
        protected Dictionary<PairInt, T> Ptr;
        protected T[] V;

        public AbsETT(int n)
        {
            N = n;
            Ptr = new Dictionary<PairInt, T>(new PairIntEC());
            V = new T[N];
        }

        public bool Same(int s, int t)
        {
            var nodeS = GetNode(s, s);
            var nodeT = GetNode(t, t);
            if (nodeS != null) Splay(nodeS);
            if (nodeT != null) Splay(nodeT);
            return Same(nodeS, nodeT);
        }

        public bool Connect(int v, int u)
        {
            if (Same(v, u)) return false;
            Merge(Reroot(GetNode(v, v)), GetNode(v, u), Reroot(GetNode(u, u)), GetNode(u, v));
            return true;
        }

        public bool Disconnect(int v, int u)
        {
            T o;
            if (!Ptr.TryGetValue(new PairInt(v, u), out o)) return false;
            var triple = Split(GetNode(v, u), GetNode(u, v));
            Merge(triple.First, triple.Third);
            Ptr.Remove(new PairInt(v, u));
            Ptr.Remove(new PairInt(u, v));
            return true;
        }

        public int Size(int v)
        {
            return Size(V[v]);
        }

        public int[] Group(int v)
        {
            var node = Reroot(GetNode(v, v));
            return TreeToArray(node);
        }

        public void EdgeConnectedUpdate(int v, bool b)
        {
            var s = GetNode(v, v);
            Splay(s);
            s.EdgeConnected = b;
            s.Update();
        }

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

        private void Go(T t, Action<int, int> f)
        {
            if (t.F < t.S && t.Exact)
            {
                Splay(t);
                t.Exact = false;
                t.Update();
                f(t.F, t.S);
                return;
            }

            if (t.Left != null && t.Left.ChildExact) Go(t.Left, f);
            else Go(t.Right, f);
        }

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

        public int[][] Groups()
        {
            List<int[]> result = new List<int[]>();

            foreach (var pair in Ptr)
            {
                var v = pair.Value;
                // root
                if (v.Parent == null)
                {
                    result.Add(TreeToArray(v));
                }
            }

            foreach (var v in V)
            {
                if (v.Parent == null)
                {
                    result.Add(TreeToArray(v));
                }
            }

            return result.ToArray();
        }

        private bool Go2(T t, Func<int, bool> f)
        {
            Debug.Assert(t != null);
            if (t.EdgeConnected)
            {
                Splay(t);
                return f(t.S);
            }

            if (t.Left != null && t.Left.ChildEdgeConnected) return Go2(t.Left, f);
            else return Go2(t.Right, f);
        }

        private T Reroot(T v)
        {
            var pair = Split(v);
            return Merge(pair.Second, pair.First);
        }


        private int Size(T v)
        {
            return v.Size;
        }

        protected abstract T GetNode(int f, int s);

        private int[] TreeToArray(T s)
        {
            Splay(s);
            int[] ar = new int[s.Size];
            int k = 0;
            Go(s, ar, ref k);
            return ar;
        }

        private void Go(T s, int[] ar, ref int k)
        {
            if (s.Size == 0) return;
            if (s.Left != null) Go(s.Left, ar, ref k);
            if (s.F == s.S) ar[k++] = s.F;
            if (s.Right != null) Go(s.Right, ar, ref k);
        }

        #region SplayTree
        protected T Merge(T l, T r)
        {
            if (l == null) return r;
            if (r == null) return l;
            while (l.Right != null) l = l.Right;
            Splay(l);
            l.Right = r;
            if (r != null) r.Parent = l;
            l.Update();
            return l;
        }

        protected T Merge(T s, params T[] t)
        {
            foreach (var v in t)
            {
                s = Merge(s, v);
            }

            return s;
        }

        protected PairNode<T> Split(T s)
        {
            Splay(s);
            var t = s.Left;
            if (t != null) t.Parent = null;
            s.Left = null;
            s.Update();
            return new PairNode<T>(t, s);
        }

        protected PairNode<T> Split2(T s)
        {
            Splay(s);
            var t = s.Left;
            var u = s.Right;
            if (t != null) t.Parent = null;
            s.Left = null;
            if (u != null) u.Parent = null;
            s.Right = null;
            s.Update();
            return new PairNode<T>(t, u);
        }

        protected TripleNode<T> Split(T s, T t)
        {
            var u = Split2(s);
            if (Same(u.First, t))
            {
                var v = Split2(t);
                return new TripleNode<T>(v.First, v.Second, u.Second);
            }
            else
            {
                var v = Split2(t);
                return new TripleNode<T>(u.First, v.First, v.Second);
            }
        }

        protected bool Same(T s, T t)
        {
            if (s != null) Splay(s);
            if (t != null) Splay(t);
            return Root(s) == Root(t);
        }

        protected T Root(T s)
        {
            if (s == null) return s;
            while (s.Parent != null) s = s.Parent;
            return s;
        }

        protected void Splay(T s)
        {
            while (s.Parent != null)
            {
                var t = s.Parent;
                if (t.Parent == null)
                {
                    Rotate(s, t.Left == s);
                }
                else
                {
                    var u = t.Parent;
                    bool b = u.Left == t;
                    if ((b ? t.Left : t.Right) == s)
                    {
                        Rotate(t, b);
                        Rotate(s, b);
                    }
                    else
                    {
                        Rotate(s, !b);
                        Rotate(s, b);
                    }
                }
            }
        }

        protected void Rotate(T s, bool b)
        {
            var x = s.Parent;
            var y = x.Parent;

            if (b)
            {
                if ((x.Left = s.Right) != null) s.Right.Parent = x;
                s.Right = x;
            }
            else
            {
                if ((x.Right = s.Left) != null) s.Left.Parent = x;
                s.Left = x;
            }
            x.Parent = s;
            x.Update();
            s.Update();
            if ((s.Parent = y) != null)
            {
                if (y.Left == x) y.Left = s;
                if (y.Right == x) y.Right = s;
                y.Update();
            }
        }

        #endregion
    }

    public class EulerTourTree : AbsETT<SplayTreeNode>
    {
        public EulerTourTree(int n) : base(n)
        {
            for (int i = 0; i < N; i++) V[i] = new SplayTreeNode(i, i);
        }

        protected override SplayTreeNode GetNode(int f, int s)
        {
            if (f == s) return V[f];
            SplayTreeNode o;
            var pair = new PairInt(f, s);
            if (!Ptr.TryGetValue(pair, out o))
            {
                o = new SplayTreeNode(f, s);
                Ptr[pair] = o;
            }
            return o;
        }
    }

    public class EulerTourTree<T> : AbsETT<SplayTreeNode<T>>
    {
        private CommutativeMonoid<T> Monoid;
        public EulerTourTree(int n, T[] ar, CommutativeMonoid<T> monoid) : base(n)
        {
            for (int i = 0; i < N; i++) V[i] = new SplayTreeNode<T>(i, i, ar[i], monoid);
            Monoid = monoid;
        }

        protected override SplayTreeNode<T> GetNode(int f, int s)
        {
            if (f == s) return V[f];
            SplayTreeNode<T> o;
            var pair = new PairInt(f, s);
            if (!Ptr.TryGetValue(pair, out o))
            {
                o = new SplayTreeNode<T>(f, s, Monoid.Identity, Monoid);
                Ptr[pair] = o;
            }
            return o;
        }

        public void Update(int v, T value)
        {
            var node = GetNode(v, v);
            Splay(node);
            node.Value = value;
            node.Update();
        }

        public T Sum(int v)
        {
            var node = GetNode(v, v);
            Splay(node);
            return node.Sum;
        }
    }

    #endregion


    #region Pair Triple
    public struct TripleNode<T> where T : AbsNode<T>
    {
        public readonly T First, Second, Third;

        public TripleNode(T f, T s, T t)
        {
            First = f;
            Second = s;
            Third = t;
        }
    }

    public struct PairNode<T> where T : AbsNode<T>
    {
        public readonly T First, Second;

        public PairNode(T f, T s)
        {
            First = f;
            Second = s;
        }
    }

    public struct PairInt
    {
        public int First;
        public int Second;
        public PairInt(int f, int s)
        {
            First = f;
            Second = s;
        }
    }

    public class PairIntEC : EqualityComparer<PairInt>
    {
        public override bool Equals(PairInt x, PairInt y)
        {
            return x.First == y.First && x.Second == y.Second;
        }

        public override int GetHashCode(PairInt obj)
        {
            return obj.First ^ (obj.Second + -1640531527 + (obj.First << 6) + (obj.Second >> 2));
        }
    }
    #endregion

    #region Node
    public abstract class AbsNode<T> where T : AbsNode<T>
    {
        public int Size;
        public T Left, Right;
        public T Parent;
        public bool EdgeConnected;
        public bool ChildEdgeConnected;
        public bool Exact;
        public bool ChildExact;
        public int F, S;

        public AbsNode(int f, int s)
        {
            F = f;
            S = s;
            Size = f == s ? 1 : 0;
            EdgeConnected = false;
            ChildEdgeConnected = false;
            Exact = F < S;
            ChildExact = Exact;
        }

        public virtual void Update()
        {
            ChildEdgeConnected = EdgeConnected;
            ChildExact = Exact;
            Size = (F == S ? 1 : 0);

            if (Left != null)
            {
                ChildEdgeConnected |= Left.ChildEdgeConnected;
                ChildExact |= Left.ChildExact;
                Size += Left.Size;
            }

            if (Right != null)
            {
                ChildEdgeConnected |= Right.ChildEdgeConnected;
                ChildExact |= Right.ChildExact;
                Size += Right.Size;
            }
        }
    }

    public class SplayTreeNode : AbsNode<SplayTreeNode>
    {
        public SplayTreeNode(int f, int s) : base(f, s) { }
    }

    public class SplayTreeNode<T> : AbsNode<SplayTreeNode<T>>
    {
        public T Value;
        public T Sum;

        public CommutativeMonoid<T> Monoid;
        public SplayTreeNode(int f, int s, T v, CommutativeMonoid<T> monoid) : base(f, s)
        {
            Value = v;
            Sum = v;
            Monoid = monoid;
        }

        public override void Update()
        {
            base.Update();
            Sum = Value;
            if (Left != null)
            {
                Sum = Monoid.Operator(Left.Sum, Sum);
            }

            if (Right != null)
            {
                Sum = Monoid.Operator(Sum, Right.Sum);
            }
        }
    }

    #endregion

    // 可換モノイド
    public class CommutativeMonoid<T>
    {
        public readonly T Identity;
        public readonly Func<T, T, T> Operator;
        public CommutativeMonoid(Func<T, T, T> op, T id)
        {
            Operator = op;
            Identity = id;
        }
    }
}
