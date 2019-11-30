namespace CompLib.Mathematics
{
    using System;
    public static class PrimalityTest
    {
        private static Random Random = new Random();

        /// <summary>
        /// Miller-Rabin素数判定法を用いてnが素数か判定 O(k log^2 n)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsPrime(long n, int k = 60)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            if (n < 10000)
            {
                for (int i = 3; i * i <= n; i++)
                {
                    if (n % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }

            if (!StrongFermatTest(2, n))
            {
                return false;
            }

            for (int i = 0; i < k; i++)
            {
                if (!StrongFermatTest(NextLong(2, n), n))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool StrongFermatTest(long a, long n)
        {
            long t = n - 1;
            long t2 = t;
            while (t % 2 == 0)
            {
                t /= 2;
                if (Pow(a, t, n) == t2)
                {
                    return true;
                }
            }
            return Pow(a, t, n) == 1;
        }

        private static long Pow(long x, long y, long mod)
        {
            x %= mod;
            long result = 1;
            while (y > 0)
            {
                if (y % 2 == 1)
                {
                    result = Multiplication(result, x, mod);
                }
                x = Multiplication(x, x, mod);
                y /= 2;
            }
            return result;
        }

        private static long Multiplication(long a, long b, long mod)
        {
            if (mod < int.MaxValue)
            {
                return (a * b) % mod;
            }
            long result = 0;
            while (b > 0)
            {
                if (b % 2 == 1)
                {
                    result += a;
                    result %= mod;
                }
                a *= 2;
                a %= mod;
                b /= 2;
            }
            return result;
        }

        private static long NextLong(long min, long max)
        {
            long d = max - min;
            long result = Random.Next();
            result <<= 31;
            result += Random.Next();
            result %= d;
            return result + min;
        }
    }
}