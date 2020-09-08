using System.Diagnostics;

namespace CompLib.Mathematics
{
    class DynamicModint
    {
        public readonly long Mod;

        /// <summary>
        /// Mod mでの値を計算する
        /// </summary>
        /// <param name="m"></param>
        public DynamicModint(long m)
        {
            Debug.Assert(m > 0);
            Mod = m;
        }

        /// <summary>
        /// a + bをmで割った余り
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public long Add(long a, long b)
        {
            return (SafeMod(a) + SafeMod(b)) % Mod;
        }

        /// <summary>
        /// a*bをmで割った余り
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public long Multiply(long a, long b)
        {
            return (SafeMod(a) * SafeMod(b)) % Mod;
        }

        /// <summary>
        /// a = b (mod m)か?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(long a, long b)
        {
            return SafeMod(a) == SafeMod(b);
        }

        /// <summary>
        /// 1/a (mod m)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public long Inverse(long a)
        {
            return Pow(a, Mod - 2);
        }

        /// <summary>
        /// a^b (mod m)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public long Pow(long a, long b)
        {
            a = SafeMod(a);
            long r = 1;
            while (b > 0)
            {
                if ((b & 1) > 0)
                {
                    r = Multiply(r, a);
                }
                a = Multiply(a, a);
                b /= 2;
            }
            return r;
        }

        /// <summary>
        /// aをmで割った余り
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public long SafeMod(long a)
        {
            a %= Mod;
            if (a < 0) a += Mod;
            return a;
        }
    }
}

