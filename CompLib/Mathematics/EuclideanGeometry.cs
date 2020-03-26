using System;

namespace CompLib.Mathematics
{
    using Num = Decimal;

    public class EuclideanGeometry
    {
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