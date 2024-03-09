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


        [TestMethod]
        public void GetTest()
        {
            var rnd = new Random();
            const int Len = 100000;
            var array = new long[Len];
            for (int i = 0; i < Len; i++)
            {
                array[i] = rnd.Next();
            }

            var wm = new WaveletMatrix(array);

            for (int i = 0; i < Len; i++)
            {
                Assert.AreEqual(array[i], wm[i]);
            }
        }

        [TestMethod]
        public void RankTest()
        {
            var rnd = new Random();
            const int Len = 1000;
            var array = new long[Len];
            for (int i = 0; i < Len; i++)
            {
                array[i] = rnd.Next(10);
            }

            var wm = new WaveletMatrix(array);


            for (int i = 0; i < 10000; i++)
            {
                int right = rnd.Next(Len + 1);
                int num = rnd.Next(10);

                int expect = 0;
                for (int j = 0; j < right; j++)
                {
                    if (array[j] == num) expect++;
                }

                Assert.AreEqual(expect, wm.Rank(right, num));
            }
        }
    }
}
