using System;

namespace CompLib.Internal
{
    static class InternalMath
    {
        // 原始根
        public static int PrimitiveRoot(int m)
        {
            switch (m)
            {
                case 2:
                    return 1;
                case 167772161:
                case 469762049:
                case 998244353:
                    return 3;
                case 754974721:
                    return 11;
            }
            throw new NotImplementedException();
        }

        public static Tuple<long, long> InvGCD(long a, long b)
        {
            a = SafeMod(a, b);
            if (a == 0) return new Tuple<long, long>(0, b);
            long s = b, t = a;
            long m0 = 0, m1 = 1;
            while (t != 0)
            {
                long u = s / t;
                s -= t * u;
                m0 -= m1 * u;

                var tmp = s;
                s = t;
                t = tmp;
                tmp = m0;
                m0 = m1;
                m1 = tmp;
            }
            if (m0 < 0) m0 += b / s;
            return new Tuple<long, long>(s, m0);
        }

        public static long SafeMod(long x, long m)
        {
            x %= m;
            if (x < 0) x += m;
            return x;
        }
    }
}
