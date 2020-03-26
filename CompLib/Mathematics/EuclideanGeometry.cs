﻿using System;

namespace CompLib.Mathematics
{
    using Num = Decimal;

    public class EuclideanGeometry
    {
        private const Num Eps = (Num) 1e-10;

        static Num Add(Num l, Num r)
        {
            if (Math.Abs(l + r) < Eps * (Math.Abs(l) + Math.Abs(r))) return 0;
            return l + r;
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
                return new P(Add(l.X, r.X), Add(l.Y, r.Y));
            }

            public static P operator -(P l, P r)
            {
                return new P(Add(l.X, -r.X), Add(l.Y, -r.Y));
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
                return Add(X * r.X, Y * r.Y);
            }

            /// <summary>
            /// 外積 l * r = |l|*|r|*sinθ
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            public Num Det(P r)
            {
                return Add(X * r.Y, -Y * r.X);
            }
        }
    }
}