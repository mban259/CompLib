using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompLib.Mathematics
{
    using Num = Double;

    public class EuclideanGeometry
    {
        private const Num Eps = (Num) 1e-10;

        public static Num Add(Num l, Num r)
        {
            if (Math.Abs(l + r) < Eps * (Math.Abs(l) + Math.Abs(r))) return 0;
            return l + r;
        }

        public static bool Equal(Num l, Num r)
        {
            return Math.Abs(l - r) < Eps * (Math.Abs(l) + Math.Abs(r));
        }

        /// <summary>
        /// 直線l上にpがあるか?
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool OnLine(Line l, P p)
        {
            return (l.A - p).Det(l.B - p) == 0;
        }

        /// <summary>
        /// 線分l上に点pがあるか?
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool OnSegment(Segment l, P p)
        {
            // pを原点に移動
            // pが直線l上にある かつ 内積が負
            return OnLine(l.ToLine(), p) && (l.A - p).Dot(l.B - p) <= 0;
        }

        /// <summary>
        /// 直線 l,rの交点 l,rが並行なら false
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool Intersection(Line l, Line r, out P p)
        {
            var t = (r.B - r.A).Det(l.B - l.A);
            // t == 0 l,rが並行
            if (t < Eps)
            {
                p = new P();
                return false;
            }

            p = l.A + (l.B - l.A) * ((r.B - r.A).Det(r.A - l.A) / t);
            return true;
        }

        // 凸包
        public static int[] ConvexHull(P[] ps, List<P> result = null)
        {
            int n = ps.Length;
            var tup = new Tuple<P, int>[n];
            for (int i = 0; i < n; i++)
            {
                tup[i] = new Tuple<P, int>(ps[i], i);
            }

            Array.Sort(tup,
                (l, r) => Equal(l.Item1.X, r.Item1.X)
                    ? l.Item1.Y.CompareTo(r.Item1.Y)
                    : l.Item1.X.CompareTo(r.Item1.X));

            var qs = new Tuple<P, int>[2 * n];
            int k = 0;

            // 下側
            for (int i = 0; i < n; i++)
            {
                while (k > 1 && (qs[k - 1].Item1 - qs[k - 2].Item1).Det(tup[i].Item1 - qs[k - 1].Item1) <= 0) k--;
                qs[k++] = tup[i];
            }

            // 上側
            for (int i = n - 2, t = k; i >= 0; i--)
            {
                while (k > t && (qs[k - 1].Item1 - qs[k - 2].Item1).Det(tup[i].Item1 - qs[k - 1].Item1) <= 0) k--;
                qs[k++] = tup[i];
            }

            int[] res = new int[k - 1];
            for (int i = 0; i < k - 1; i++)
            {
                res[i] = qs[i].Item2;
            }

            if (result != null)
            {
                for (int i = 0; i < k - 1; i++)
                {
                    result.Add(qs[i].Item1);
                }
            }

            return res;
        }

        /// <summary>
        /// a,bの中点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static P Mid(P a, P b) => new P(Add(a.X, b.X) / 2, Add(a.Y, b.Y) / 2);

        /// <summary>
        /// 線分 lの中点
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static P Mid(Segment l) => Mid(l.A, l.B);

        /// <summary>
        /// 線分a,bの垂直二等分線
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Line VerticalBisector(P a, P b)
        {
            var mid = Mid(a, b);

            var p = new P(Add(mid.X, Add(b.Y, -a.Y)), Add(mid.Y, Add(a.X, -b.X)));
            return new Line(mid, p);
        }

        /// <summary>
        /// 線分lの垂直二等分線
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Line VerticalBisector(Segment l) => VerticalBisector(l.A, l.B);

        /// <summary>
        /// 3点を通る円
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool MakeCircle(P a, P b, P c, Circle o)
        {
            if (OnLine(new Line(a, b), c))
            {
                o = new Circle();
                return false;
            }

            P p;
            Intersection(VerticalBisector(a, b), VerticalBisector(b, c), out p);

            return true;
        }

        /// <summary>
        /// 2円の交点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static P[] Intersection(Circle a, Circle b)
        {
            var tmp = a.O - b.O;
            if (Add(a.R, b.R) * Add(a.R, b.R) > Add(tmp.X * tmp.X, tmp.Y * tmp.Y)) return new P[0];
            if (Add(tmp.X * tmp.X, tmp.Y * tmp.Y) < Add(a.R, -b.R) * Add(a.R, -b.R)) return new P[0];
            
            Num aa = 
        }
    }

    /// <summary>
    /// 円 中心 O 半径 R
    /// </summary>
    public struct Circle
    {
        public readonly P O;
        public readonly Num R;

        public Circle(P o, Num r)
        {
            O = o;
            R = r;
        }
    }

    /// <summary>
    /// 直線 a-b
    /// </summary>
    public struct Line
    {
        public readonly P A, B;

        public Line(P a, P b)
        {
            A = a;
            B = b;
        }

        public Line(Num aX, Num aY, Num bX, Num bY)
        {
            A = new P(aX, aY);
            B = new P(bX, bY);
        }

        public Line(Segment s)
        {
            A = s.A;
            B = s.B;
        }

        public P Vector() => A - B;
    }

    /// <summary>
    /// 線分 a-b
    /// </summary>
    public struct Segment
    {
        public readonly P A, B;

        public Segment(P a, P b)
        {
            A = a;
            B = b;
        }

        public Segment(Num aX, Num aY, Num bX, Num bY)
        {
            A = new P(aX, aY);
            B = new P(bX, bY);
        }

        public Line ToLine()
        {
            return new Line(A, B);
        }
    }

    // 2次元ベクトル
    public struct P
    {
        public readonly Num X, Y;

        public P(Num x, Num y)
        {
            X = x;
            Y = y;
        }

        public static P operator +(P l, P r)
        {
            return new P(EuclideanGeometry.Add(l.X, r.X), EuclideanGeometry.Add(l.Y, r.Y));
        }

        public static P operator -(P l, P r)
        {
            return new P(EuclideanGeometry.Add(l.X, -r.X), EuclideanGeometry.Add(l.Y, -r.Y));
        }

        public static P operator *(P l, Num r)
        {
            return new P(l.X * r, l.Y * r);
        }

        /// <summary>
        /// 内積 l * r = |l|*|r|*cosθ
        /// </summary>
        /// <returns></returns>
        public Num Dot(P r)
        {
            return EuclideanGeometry.Add(X * r.X, Y * r.Y);
        }

        /// <summary>
        /// 外積 l * r = |l|*|r|*sinθ
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Num Det(P r)
        {
            return EuclideanGeometry.Add(X * r.Y, -Y * r.X);
        }

        public Num Abs() => Math.Sqrt(EuclideanGeometry.Add(X * X, Y * Y));
    }
}