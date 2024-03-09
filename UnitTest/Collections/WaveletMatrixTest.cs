using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompLib.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections
{
    [TestClass]
    public class WaveletMatrixTest
    {
        [TestMethod]
        public void BitVectorGetTest()
        {
            var rnd = new Random();
            const int Len = 1000000;
            var bits = new bool[Len];
            for (int i = 0; i < 500000; i++)
            {
                bits[rnd.Next(Len)] ^= true;
            }

            var bv = new BitVector(bits);
            for (int i = 0; i < Len; i++)
            {
                Assert.AreEqual(bits[i], bv[i]);
            }
        }
        [TestMethod]
        public void BitVectorRankTest()
        {
            var rnd = new Random();
            const int Len = 1000000;
            var bits = new bool[Len];
            for (int i = 0; i < 500000; i++)
            {
                bits[rnd.Next(Len)] ^= true;
            }

            var bv = new BitVector(bits);

            int cnt = 0;
            for (int i = 0; i < Len; i++)
            {
                Assert.AreEqual(bv.Rank1(i), cnt);
                if (bits[i]) cnt++;
            }
            Assert.AreEqual(bv.Rank1(Len), cnt);
        }

        [TestMethod]
        public void BitVectorSelectTest()
        {
            var rnd = new Random();
            const int Len = 1000000;
            var bits = new bool[Len];
            for (int i = 0; i < 500000; i++)
            {
                bits[rnd.Next(Len)] ^= true;
            }

            var bv = new BitVector(bits);

            int cnt0 = 0;
            int cnt1 = 0;
            for (int i = 0; i < Len; i++)
            {
                if (bits[i])
                {
                    cnt1++;
                    Assert.AreEqual(i, bv.Select1(cnt1));
                }
                else
                {
                    cnt0++;
                    Assert.AreEqual(i, bv.Select0(cnt0));
                }
            }
        }

    }
}
