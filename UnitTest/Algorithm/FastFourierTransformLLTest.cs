using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompLib.Algorithm;

namespace UnitTest.Algorithm
{
    [TestClass]
    public class FastFourierTransformLLTest
    {
        [TestMethod]
        public void ConvolutionReturnsEmptyForEmptyInput()
        {
            CollectionAssert.AreEqual(
                Array.Empty<long>(),
                FastFourierTransformLL.Convolution(Array.Empty<long>(), new long[] { 1 }));
        }

        [TestMethod]
        public void ConvolutionUsesNaivePath()
        {
            long[] a = { -2, 0, 3 };
            long[] b = { 4, -5 };

            CollectionAssert.AreEqual(
                new long[] { -8, 10, 12, -15 },
                FastFourierTransformLL.Convolution(a, b));
        }

        [TestMethod]
        public void ConvolutionUsesNttPath()
        {
            var a = new long[61];
            var b = new long[73];
            for (int i = 0; i < a.Length; i++)
                a[i] = i % 2 == 0 ? i - 30 : 30 - i;
            for (int i = 0; i < b.Length; i++)
                b[i] = i % 3 == 0 ? -i : i;

            var expected = new long[a.Length + b.Length - 1];
            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < b.Length; j++)
                    expected[i + j] += a[i] * b[j];

            CollectionAssert.AreEqual(expected, FastFourierTransformLL.Convolution(a, b));
        }

        [TestMethod]
        public void ConvolutionPreservesLongBoundaryValues()
        {
            CollectionAssert.AreEqual(
                new[] { long.MinValue, long.MaxValue },
                FastFourierTransformLL.Convolution(
                    new[] { long.MinValue, long.MaxValue },
                    new long[] { 1 }));
        }
    }
}
