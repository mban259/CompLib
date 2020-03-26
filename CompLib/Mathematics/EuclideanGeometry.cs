using System;

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


        // 2次元ベクトル
        public struct P
        {
            public readonly Num X, Y;

            public P(Num x, Num y)
            {
                X = x;
                Y = y;
            }
        }
    }
}