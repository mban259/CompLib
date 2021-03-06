using System;
using System.Numerics;
using CompLib.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Algorithm
{
    [TestClass]
    public class FastFourierTransformTest
    {
        private const int MaxN = 10000;
        private const int MaxA = 1000000;
        private static long[] _a, _b, _c;

        public static Complex[] D;
        public static Complex[] HatD;

        [ClassInitialize]
        public static void RandomTestInitialize(TestContext context)
        {
            var rnd = new Random();
            int n = rnd.Next(1, MaxN + 1);
            int m = rnd.Next(1, MaxN + 1);
            _a = new long[n];
            for (int i = 0; i < n; i++)
            {
                _a[i] = rnd.Next(MaxA);
            }

            _b = new long[m];
            for (int i = 0; i < m; i++)
            {
                _b[i] = rnd.Next(MaxA);
            }

            _c = new long[n + m - 1];

            for (int k = 0; k < n + m - 1; k++)
            {
                for (int i = 0; i <= Math.Min(k, n - 1); i++)
                {
                    int j = k - i;
                    if (i < n && j < m)
                    {
                        _c[k] += _a[i] * _b[j];
                    }
                }
            }

            int o = rnd.Next(MaxN);
            D = new Complex[o];
            for (int i = 0; i < o; i++)
            {
                D[i] = new Complex(rnd.Next(MaxA), rnd.Next(MaxA));
            }

            HatD = FastFourierTransformTest.Transform(D, false);
        }

        [TestMethod]
        public void Test1()
        {
            long[] a = {0, 1, 2, 3, 4, 0, 0, 0};
            long[] b = {0, 1, 2, 4, 8, 0, 0, 0};

            long[] expected = new long[] {0, 0, 1, 4, 11, 26, 36, 40, 32, 0, 0, 0, 0, 0, 0, 0};
            var c = FastFourierTransform.Convolution(a, b);

            Assert.AreEqual(expected.Length, c.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], c[i]);
            }
        }

        [TestMethod]
        public void Test2()
        {
            long[] fft = FastFourierTransform.Convolution(_a, _b);

            Assert.IsTrue(_c.Length <= fft.Length);
            for (int i = 0; i < _c.Length; i++)
            {
                Assert.AreEqual(_c[i], fft[i]);
            }
        }

        [TestMethod]
        public void ComplexTest1()
        {
            // かなりガバガバテスト
            Complex[] fft = FastFourierTransform.DiscreteFourierTransform(D);
            Assert.AreEqual(HatD.Length, fft.Length);
            for (int i = 0; i < HatD.Length; i++)
            {
                double reDiff = (fft[i].Real - HatD[i].Real) / HatD[i].Real;
                bool re = -0.1 < reDiff && reDiff < 0.1;

                double imDiff = (fft[i].Imaginary - HatD[i].Imaginary) / HatD[i].Imaginary;
                bool im = -0.1 < imDiff && imDiff < 0.1;
                Assert.IsTrue(re && im);
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
                Assert.AreEqual(zetaNi[i].Num, zetaNi[i + n].Num);
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
                Assert.AreEqual(invZetaNi[i].Num, invZetaNi[i + n]);
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

                    Assert.AreEqual(j == k ? n : 0, sum.Num);
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

                    Assert.AreEqual(j == k ? n : 0, sum.Num);
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

        private static Complex[] Transform(Complex[] a, bool inv)
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