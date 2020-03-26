﻿using System;
using System.Collections.Generic;

namespace CompLib.Mathematics
{
    using Num = Decimal;

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
        /// 線分l上に点pがあるか?
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool OnSegment(Segment l, P p)
        {
            // pを原点に移動
            // pが直線l上にある かつ 内積が負
            return (l.A - p).Det(l.B - p) == 0 && (l.A - p).Dot(l.B - p) <= 0;
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
    }
}