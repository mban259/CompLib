using System;
using System.Numerics;

namespace CompLib.Algorithm
{
    public static class FastFourierTransform
    {
        private const int E = 50;
        private const long N = 1L << E;

        // 1の原始N乗根
        private const long O = 1521;
        private const long InvO = 546857209190701451;

        private const long Mod = 1012 * N + 1;

        // private const int E = 27;
        // private const long N = 1L << E;
        // private const long O = 137;
        // private const long InvO = 749463956;
        // private const long Mod = 15 * N + 1;


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

        // ふぁっきん小数点誤差なのでmodint
        // 整数論ばんざい
        /// <summary>
        /// a,bからc[k]=Σ(a[i]*b[k-i])を求める
        /// c[k] < Mod
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long[] Multiplication(long[] a, long[] b)
        {
            int len = 1;
            int l = 0;
            while (len < (a.Length + b.Length))
            {
                l++;
                len *= 2;
            }

            var MA = new ModInt[len];
            for (int i = 0; i < a.Length; i++)
            {
                MA[i] = a[i];
            }

            var MB = new ModInt[len];
            for (int i = 0; i < b.Length; i++)
            {
                MB[i] = b[i];
            }

            ModInt[] hatG = FFT(MA, l, false);
            ModInt[] hatH = FFT(MB, l, false);
            ModInt[] hatGH = new ModInt[len];
            for (int i = 0; i < len; i++)
            {
                hatGH[i] = hatG[i] * hatH[i];
            }

            ModInt[] nF = FFT(hatGH, l, true);

            ModInt invLen = ModInt.Pow(len, Mod - 2);
            long[] f = new long[len];
            for (int i = 0; i < len; i++)
            {
                f[i] = (nF[i] * invLen)._num;
            }

            return f;
        }

        private static ModInt[] FFT(ModInt[] a, int l, bool inverse)
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

            ModInt[] hatF0 = FFT(a0, l - 1, inverse);
            ModInt[] hatF1 = FFT(a1, l - 1, inverse);

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
        public static double[] Multiplication(double[] a, double[] b)
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

            Complex[] hatG = FFT(compA);
            Complex[] hatH = FFT(compB);
            Complex[] hatGH = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                hatGH[i] = hatG[i] * hatH[i];
            }

            Complex[] nF = FFT(hatGH, true);

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

            return FFT(aa, inverse);
        }

        // f(x) = Σa_i*x^i の離散フーリエ変換
        // deg(f)は2羃
        private static Complex[] FFT(Complex[] a, bool inverse = false)
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

            Complex[] hatF0 = FFT(a0, inverse);
            Complex[] hatF1 = FFT(a1, inverse);

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
            public long _num { get; private set; }

            public ModInt(long n)
            {
                _num = n % Mod;
            }

            public static implicit operator ModInt(long n)
            {
                return new ModInt(n);
            }

            public static ModInt operator *(ModInt a, ModInt b)
            {
                if (Mod < int.MaxValue)
                {
                    return (a._num * b._num) % Mod;
                }

                long bb = b._num;
                ModInt result = 0;
                while (bb > 0)
                {
                    if (bb % 2 == 1)
                    {
                        result += a;
                    }

                    a += a;
                    bb /= 2;
                }

                return result;
            }

            public static ModInt operator +(ModInt a, ModInt b)
            {
                return (a._num + b._num) % Mod;
            }

            // a^(2^b)
            public static ModInt Pow2(ModInt a, int b)
            {
                ModInt result = a;
                for (int i = 0; i < b; i++)
                {
                    result *= result;
                }

                return result;
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
        }
    }
}