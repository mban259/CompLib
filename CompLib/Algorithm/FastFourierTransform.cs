namespace CompLib.Algorithm
{
    using Mathematics;
    using System;
    using System.Numerics;

    public static class FastFourierTransform
    {
        private const int E = 26;
        public static readonly ModInt O = new ModInt(18769, 136);
        public static readonly ModInt InvO = new ModInt(137728885, 839354248);

        // Z[i] 1の原始2^i乗根
        public static readonly ModInt[] Z;
        public static readonly ModInt[] InvZ;

        static FastFourierTransform()
        {
            Z = new ModInt[E + 1];
            InvZ = new ModInt[E + 1];
            Z[E] = O; // ZE☆
            InvZ[E] = InvO;
            for (int i = E - 1; i >= 0; i--)
            {
                Z[i] = Z[i + 1] * Z[i + 1];
                InvZ[i] = InvZ[i + 1] * InvZ[i + 1];
            }
        }

        /// <summary>
        /// a,bからc[k]=Σ(a[i]*b[k-i])を求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long[] Convolution(long[] a, long[] b)
        {
            int len = 1;
            int l = 0;
            while (len < (a.Length + b.Length))
            {
                l++;
                len *= 2;
            }

            var mA = new ModInt[len];
            for (int i = 0; i < a.Length; i++)
            {
                mA[i] = a[i];
            }

            var mB = new ModInt[len];
            for (int i = 0; i < b.Length; i++)
            {
                mB[i] = b[i];
            }

            ModInt[] hatG = Transform(mA, l, false);
            ModInt[] hatH = Transform(mB, l, false);
            ModInt[] hatF = new ModInt[len];
            for (int i = 0; i < len; i++)
            {
                hatF[i] = hatG[i] * hatH[i];
            }

            ModInt[] nF = Transform(hatF, l, true);

            ModInt invLen = ModInt.Inverse(len);
            long[] f = new long[len];
            for (int i = 0; i < len; i++)
            {
                f[i] = (nF[i] * invLen).Num;
            }

            return f;
        }

        private static ModInt[] Transform(ModInt[] a, int l, bool inverse)
        {
            int len = 1 << l;
            if (len == 1)
            {
                return a;
            }

            ModInt[] a0 = new ModInt[len / 2];
            ModInt[] a1 = new ModInt[len / 2];

            for (int i = 0; i < len / 2; i++)
            {
                a0[i] = a[i * 2];
                a1[i] = a[i * 2 + 1];
            }

            ModInt[] hatF0 = Transform(a0, l - 1, inverse);
            ModInt[] hatF1 = Transform(a1, l - 1, inverse);

            ModInt zetaN = inverse ? InvZ[l] : Z[l];

            ModInt[] result = new ModInt[len];
            ModInt zetaNi = 1;
            for (int i = 0; i < len; i++)
            {
                result[i] = hatF0[i % (len / 2)] + zetaNi * hatF1[i % (len / 2)];
                zetaNi *= zetaN;
            }

            return result;
        }

        /// <summary>
        /// a,bからc[k]=Σ(a[i]*b[k-i])を求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Convolution(double[] a, double[] b)
        {
            int n = 1;
            while (n < (a.Length + b.Length))
            {
                n *= 2;
            }

            var compA = new Complex[n];
            for (int i = 0; i < a.Length; i++)
            {
                compA[i] = new Complex(a[i], 0);
            }

            var compB = new Complex[n];
            for (int i = 0; i < b.Length; i++)
            {
                compB[i] = new Complex(b[i], 0);
            }

            Complex[] hatG = Transform(compA);
            Complex[] hatH = Transform(compB);
            Complex[] hatF = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                hatF[i] = hatG[i] * hatH[i];
            }

            Complex[] nF = Transform(hatF, true);

            double[] f = new double[n];
            for (int i = 0; i < n; i++)
            {
                f[i] = nF[i].Real / n;
            }

            return f;
        }

        public static Complex[] DiscreteFourierTransform(Complex[] a, bool inverse = false)
        {
            int n = 1;
            while (n < a.Length)
            {
                n *= 2;
            }

            Complex[] aa = new Complex[n];
            for (int i = 0; i < a.Length; i++)
            {
                aa[i] = a[i];
            }

            return Transform(aa, inverse);
        }

        // f(x) = Σa_i*x^i の離散フーリエ変換
        // deg(f)は2羃
        private static Complex[] Transform(Complex[] a, bool inverse = false)
        {
            int n = a.Length;
            if (n == 1)
            {
                return a;
            }

            Complex[] a0 = new Complex[n / 2];
            Complex[] a1 = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                a0[i] = a[2 * i];
                a1[i] = a[2 * i + 1];
            }

            Complex[] hatF0 = Transform(a0, inverse);
            Complex[] hatF1 = Transform(a1, inverse);

            // ζ_n
            Complex zetaN = Zeta(inverse ? -n : n);

            var result = new Complex[n];

            // ζ_n^i
            Complex zetaNi = 1;
            for (int i = 0; i < n; i++)
            {
                result[i] = hatF0[i % (n / 2)] + zetaNi * hatF1[i % (n / 2)];
                zetaNi *= zetaN;
            }

            return result;
        }

        private static Complex Zeta(int n)
        {
            double rad = 2 * Math.PI / n;
            double re = Math.Cos(rad);
            double im = Math.Sin(rad);
            return new Complex(re, im);
        }

        public struct ModInt
        {
            private const long Mod1 = (1L << 27) * 15 + 1;
            private const long Mod2 = (1L << 26) * 27 + 1;
            public long Num1 { get; private set; }
            public long Num2 { get; private set; }

            public long Num
            {
                get
                {
                    long pq;
                    return EMath.ChaineseRemainderTheorem(Num1, Mod1, Num2, Mod2, out pq);
                }
            }

            public ModInt(long n)
            {
                Num1 = n % Mod1;
                Num2 = n % Mod2;
            }

            public ModInt(long n, long m)
            {
                Num1 = n % Mod1;
                Num2 = m % Mod2;
            }

            public static implicit operator ModInt(long n)
            {
                return new ModInt(n);
            }

            public static ModInt operator *(ModInt a, ModInt b)
            {
                return new ModInt(a.Num1 * b.Num1, a.Num2 * b.Num2);
            }

            public static ModInt operator +(ModInt a, ModInt b)
            {
                return new ModInt(a.Num1 + b.Num1, a.Num2 + b.Num2);
            }

            public static ModInt Pow(ModInt a, long b)
            {
                ModInt result = 1;
                while (b > 0)
                {
                    if (b % 2 == 1)
                    {
                        result *= a;
                    }

                    a *= a;
                    b /= 2;
                }

                return result;
            }

            public static ModInt Inverse(int n)
            {
                long t1 = n;
                long num1 = 1;
                long b1 = Mod1 - 2;
                while (b1 > 0)
                {
                    if (b1 % 2 == 1)
                    {
                        num1 *= t1;
                        num1 %= Mod1;
                    }

                    t1 *= t1;
                    t1 %= Mod1;
                    b1 /= 2;
                }

                long t2 = n;
                long num2 = 1;
                long b2 = Mod2 - 2;
                while (b2 > 0)
                {
                    if (b2 % 2 == 1)
                    {
                        num2 *= t2;
                        num2 %= Mod2;
                    }

                    t2 *= t2;
                    t2 %= Mod2;
                    b2 /= 2;
                }

                return new ModInt(num1, num2);
            }
        }
    }
}