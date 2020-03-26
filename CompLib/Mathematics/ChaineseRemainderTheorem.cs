namespace CompLib.Mathematics
{
    public static partial class EMath
    {
        private static long Mod(long a, long m)
        {
            return (a % m + m) % m;
        }

        /// <summary>
        /// n%p=a, n%q=b, n%lcm(p*q)を求める ただしa=b (mod gcd(p,q))
        /// </summary>
        /// <param name="a"></param>
        /// <param name="p"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static long ChaineseRemainderTheorem(long a, long p, long b, long q, out long pq)
        {
            long c, d;
            long e = EMath.ExtGCD(p, q, out c, out d);
            pq = p / e * q;
            if (a % e != b % e)
            {
                return -1;
            }

            // pc + qd = e 

            // n = a + p(b-a)c/e

            // p(b-a)c/e (mod lcm(pq))

            long tmp = (b - a) / e * c % (q / e);
            return (a + Mod(p * tmp, pq)) % pq;
        }
    }
}