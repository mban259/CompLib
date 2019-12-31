namespace CompLib.Mathematics
{
    public partial class EMath
    {
        /// <summary>
        /// ap+bq=gcd(a,b)を求め,gcd(a,b)を返す
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static long ExtGCD(long a, long b, out long p, out long q)
        {
            if (b == 0)
            {
                p = 1;
                q = 0;
                return a;
            }

            long d = ExtGCD(b, a % b, out q, out p);
            q -= a / b * p;
            return d;
        }
    }
}