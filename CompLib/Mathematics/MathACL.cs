using System.Diagnostics;

namespace CompLib.Mathematics
{
    public static partial class MathACL
    {
        /// <summary>
        /// xのn乗をmで割った余り
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static long PowMod(long x, long n, int m)
        {
            Debug.Assert(0 <= n && 1 <= m);
            if (m == 1) return 0;
            x = SafeMod(x, m);
            long r = 1;
            while (n != 0)
            {
                if ((n & 1) > 0)
                {
                    r *= x;
                    r %= m;
                }
                x *= x;
                x %= m;
                n >>= 1;
            }
            return r;
        }

        /// <summary>
        /// xの逆数をmで割った余り
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static long InvMod(long x, long m)
        {
            Debug.Assert(1 <= m);
            var z = InvGCD(x, m);
            Debug.Assert(z.g == 1);
            return z.x;
        }

        /// <summary>
        /// x = r[i] (mod m[i]), ∀i ∈ {0,1,...,n-1} なxを求め、(mod lcm(m))で返します
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static (long rem, long mod) CRT(long[] r, long[] m)
        {
            Debug.Assert(r.Length == m.Length);
            int n = r.Length;
            long r0 = 0;
            long m0 = 1;
            for (int i = 0; i < n; i++)
            {
                Debug.Assert(1 <= m[i]);
                long r1 = SafeMod(r[i], m[i]);
                long m1 = m[i];

                if (m0 < m1)
                {
                    long t = r0;
                    r0 = r1;
                    r1 = t;

                    t = m0;
                    m0 = m1;
                    m1 = t;
                }

                if (m0 % m1 == 0)
                {
                    if (r0 % m1 != r1) return (0, 0);
                    continue;
                }


                (long g, long im) = InvGCD(m0, m1);

                long u1 = m1 / g;

                if ((r1 - r0) % g != 0) return (0, 0);

                long x = (r1 - r0) / g % u1 * im % u1;

                r0 += x * m0;
                m0 *= u1;
                if (r0 < 0) r0 += m0;
            }

            return (r0, m0);
        }

        /// <summary>
        /// Σ floor((a*i + b)/m)を返します
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long FloorSum(long n, long m, long a, long b)
        {
            long ans = 0;
            if (a >= m)
            {
                ans += (n - 1) * n * (a / m) / 2;
                a %= m;
            }
            if (b >= m)
            {
                ans += n * (b / m);
                b %= m;
            }

            long yMax = (a * n + b) / m;
            long xMax = (yMax * m - b);
            if (yMax == 0) return ans;
            ans += (n - (xMax + a - 1) / a) * yMax;
            ans += FloorSum(yMax, a, m, (a - xMax % a) % a);
            return ans;
        }

        private static long SafeMod(long n, long m)
        {
            n %= m;
            if (n < 0) n += m;
            return n;
        }

        // g = gcd (a, b)
        // xa = g (mod b)
        private static (long g, long x) InvGCD(long a, long b)
        {
            a = SafeMod(a, b);
            if (a == 0) return (b, 0);
            long s = b;
            long t = a;
            long m0 = 0;
            long m1 = 1;

            while (t > 0)
            {
                long u = s / t;
                s -= t * u;
                m0 -= m1 * u;

                long tmp = s;
                s = t;
                t = tmp;

                tmp = m0;
                m0 = m1;
                m1 = tmp;
            }

            if (m0 < 0) m0 += b / s;
            return (s, m0);
        }
    }
}
