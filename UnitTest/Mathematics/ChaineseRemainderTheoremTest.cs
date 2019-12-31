using System;
using CompLib.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Mathematics
{
    [TestClass]
    public class ChaineseRemainderTheoremTest
    {
        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                long n = rnd.Next();
                long p = rnd.Next();
                long q = rnd.Next();

                long a = n % p;
                long b = n % q;

                long pq;
                long ans = EMath.ChaineseRemainderTheorem(a, p, b, q, out pq);

                Assert.IsTrue(pq % p == 0);
                Assert.IsTrue(pq % q == 0);
                Assert.AreEqual(ans % pq, n % pq);
            }

            for (int i = 0; i < 100000; i++)
            {
                long a = rnd.Next();
                long b = rnd.Next();
                long p = rnd.Next();
                long q = rnd.Next();

                long pq;
                long ans = EMath.ChaineseRemainderTheorem(a, p, b, q, out pq);

                long gcd = p * q / pq;

                if (a % gcd == b % gcd)
                {
                    Assert.AreEqual(ans % p, a % p);
                    Assert.AreEqual(ans % q, b % q);
                }
                else
                {
                    Assert.AreEqual(ans, -1);
                }
            }
        }
    }
}