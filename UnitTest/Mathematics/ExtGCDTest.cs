using System;
using CompLib.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Mathematics
{
    [TestClass]
    public class ExtGCDTest
    {
        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                long a = rnd.Next();
                long b = rnd.Next();

                long p, q;
                long gcd = EMath.ExtGCD(a, b, out p, out q);

                Assert.IsTrue(a % gcd == 0);
                Assert.IsTrue(b % gcd == 0);
                Assert.AreEqual(a * p + b * q, gcd);
            }
        }
    }
}