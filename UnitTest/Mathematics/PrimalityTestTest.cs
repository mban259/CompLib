using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompLib.Mathematics;
using UnitTest.Util;
namespace UnitTest.Mathematics
{
    [TestClass]
    public class PrimarityTestTest
    {
        [TestMethod]
        public void CarmichaelNumbers()
        {
            foreach (long l in Num.CarmichaelNumbers)
            {
                Assert.IsFalse(PrimalityTest.IsPrime(l), "{0} is not prime number", l);
            }
        }


        [TestMethod]
        public void StrongPseudoprimesBase2()
        {
            foreach (long l in Num.StrongPseudoprimesBase2)
            {
                Assert.IsFalse(PrimalityTest.IsPrime(l), "{0} is not prime number", l);
            }
        }

        [TestMethod]
        public void LargeTest()
        {
            foreach (long l in Num.PrimeNumbers)
            {
                Assert.IsTrue(PrimalityTest.IsPrime(l), "{0} is prime number", l);
            }
            foreach (long l in Num.CompositeNumbers)
            {
                Assert.IsFalse(PrimalityTest.IsPrime(l), "{0} is not prime number", l);
            }
        }

        [TestMethod]
        public void RandomTest()
        {
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
            {
                int n = r.Next();
                Assert.AreEqual(IsPrime(n), PrimalityTest.IsPrime(n), "{0}", n);
            }
        }

        private bool IsPrime(long n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            for (long i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
    }
}