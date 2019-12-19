using System;
using System.Numerics;
using System.Text;
using CompLib.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Algorithm
{
    [TestClass]
    public class FastFourierTransformTest
    {
        private const int MaxN = 100;
        private const int MaxA = 100;
        private static long[] A, B, C;

        public static Complex[] D;
        public static Complex[] DFTD;

        [ClassInitialize]
        public static void RandomTestInitialize(TestContext context)
        {
            var rnd = new Random();
            int n = rnd.Next(1, MaxN + 1);
            int m = rnd.Next(1, MaxN + 1);
            A = new long[n];
            for (int i = 0; i < n; i++)
            {
                A[i] = rnd.Next(MaxA);
            }

            B = new long[m];
            for (int i = 0; i < m; i++)
            {
                B[i] = rnd.Next(MaxA);
            }

            C = new long[n + m - 1];

            for (int k = 0; k < n + m - 1; k++)
            {
                for (int i = 0; i <= Math.Min(k, n - 1); i++)
                {
                    int j = k - i;
                    if (i < n && j < m)
                    {
                        C[k] += A[i] * B[j];
                    }
                }
            }

            int o = rnd.Next(MaxN);
            D = new Complex[o];
            for (int i = 0; i < o; i++)
            {
                D[i] = new Complex(rnd.Next(MaxA), rnd.Next(MaxA));
            }

            DFTD = FastFourierTransformTest.DFT(D, false);
        }

        [TestMethod]
        public void Test1()
        {
            long[] a = {0, 1, 2, 3, 4, 0, 0, 0};
            long[] b = {0, 1, 2, 4, 8, 0, 0, 0};

            long[] expected = new long[] {0, 0, 1, 4, 11, 26, 36, 40, 32, 0, 0, 0, 0, 0, 0, 0};
            var c = FastFourierTransform.Multiplication(a, b);

            Assert.AreEqual(expected.Length, c.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], c[i]);
            }
        }

        [TestMethod]
        public void Test2()
        {
            var rnd = new Random();
            Complex[] a = new Complex[16];
            for (int i = 0; i < 16; i++)
            {
                a[i] = new Complex(rnd.Next(10), rnd.Next(10));
            }

            var dft = DFT(a, false);
            var fft = FastFourierTransform.DiscreteFourierTransform(a);

            Assert.AreEqual(dft.Length, fft.Length);

            for (int i = 0; i < 8; i++)
            {
                bool re = Math.Abs(dft[i].Real - fft[i].Real) < 0.000000001;
                bool im = Math.Abs(dft[i].Real - fft[i].Real) < 0.000000001;
                Assert.IsTrue(re && im);
            }
        }


        [TestMethod]
        public void Test3()
        {
            long[] fft = FastFourierTransform.Multiplication(A, B);

            Assert.IsTrue(C.Length <= fft.Length);
            for (int i = 0; i < C.Length; i++)
            {
                Assert.AreEqual(C[i], fft[i]);
            }
        }

        [TestMethod]
        public void Test4()
        {
            // かなりガバガバテスト
            Complex[] fft = FastFourierTransform.DiscreteFourierTransform(D);
            Assert.AreEqual(DFTD.Length, fft.Length);
            for (int i = 0; i < DFTD.Length; i++)
            {
                double reDiff = (fft[i].Real - DFTD[i].Real) / DFTD[i].Real;
                bool re = -0.1 < reDiff && reDiff < 0.1;

                double imDiff = (fft[i].Imaginary - DFTD[i].Imaginary) / DFTD[i].Imaginary;
                bool im = -0.1 < imDiff && imDiff < 0.1;
            }
        }

        [TestMethod]
        public void ZTest1()
        {
            // ζ_n^i = ζ_n^i ⇔ i = j mod n

            int n = 32;
            var zetaN = FastFourierTransform.Z[5];

            var zetaNi = new FastFourierTransform.ModInt[2 * n];
            zetaNi[0] = 1;
            for (int i = 1; i < 2 * n; i++)
            {
                zetaNi[i] = zetaNi[i - 1] * zetaN;
            }

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(zetaNi[i]._num, zetaNi[i + n]._num);
            }

            var invZetaN = FastFourierTransform.InvZ[4];
            var invZetaNi = new FastFourierTransform.ModInt[2 * n];
            invZetaNi[0] = 1;
            for (int i = 1; i < 2 * n; i++)
            {
                invZetaNi[i] = invZetaNi[i - 1] * invZetaN;
            }

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(invZetaNi[i]._num, invZetaNi[i + n]);
            }
        }

        [TestMethod]
        public void ZTest2()
        {
            int n = 32;
            var zetaN = FastFourierTransform.Z[5];
            var invZetaN = FastFourierTransform.InvZ[5];
            var zetaNt = new FastFourierTransform.ModInt[n];
            var invZetaNt = new FastFourierTransform.ModInt[n];
            zetaNt[0] = 1;
            invZetaNt[0] = 1;
            for (int i = 1; i < n; i++)
            {
                zetaNt[i] = zetaNt[i - 1] * zetaN;
                invZetaNt[i] = invZetaNt[i - 1] * invZetaN;
            }

            for (int j = 0; j < n; j++)
            {
                var zetaNj = zetaNt[j];
                for (int k = 0; k < n; k++)
                {
                    var barZetaNk = invZetaNt[k];

                    FastFourierTransform.ModInt sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        FastFourierTransform.ModInt zetaNji = 1;
                        FastFourierTransform.ModInt barZetaNki = 1;
                        for (int l = 0; l < i; l++)
                        {
                            zetaNji *= zetaNj;
                            barZetaNki *= barZetaNk;
                        }

                        sum += zetaNji * barZetaNki;
                    }

                    Assert.AreEqual(j == k ? n : 0, sum._num);
                }
            }

            for (int j = 0; j < n; j++)
            {
                var invZetaNj = invZetaNt[j];
                for (int k = 0; k < n; k++)
                {
                    var invBarZetaNk = zetaNt[k];
                    FastFourierTransform.ModInt sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        FastFourierTransform.ModInt invZetaNji = 1;
                        FastFourierTransform.ModInt invBarZetaNki = 1;
                        for (int l = 0; l < i; l++)
                        {
                            invZetaNji *= invZetaNj;
                            invBarZetaNki *= invBarZetaNk;
                        }

                        sum += invZetaNji * invBarZetaNki;
                    }

                    Assert.AreEqual(j == k ? n : 0, sum._num);
                }
            }
        }

        private static Complex Zeta(int n)
        {
            double rad = 2 * Math.PI / n;
            double re = Math.Cos(rad);
            double im = Math.Sin(rad);
            return new Complex(re, im);
        }

        private static Complex[] DFT(Complex[] a, bool inv)
        {
            int n = 1;
            while (n < a.Length)
            {
                n *= 2;
            }

            var zetaN = Zeta(inv ? -n : n);
            Complex zetaNi = 1;
            var result = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                // f(ζ_n^i)
                Complex fZetaNi = 0;
                Complex zetaNij = 1;
                for (int j = 0; j < a.Length; j++)
                {
                    fZetaNi += a[j] * zetaNij;
                    zetaNij *= zetaNi;
                }

                result[i] = fZetaNi;

                zetaNi *= zetaN;
            }

            return result;
        }
    }
}