using System;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class SegmentTreeTest
    {
        private const int N = 1 << 21;

        [TestMethod]
        public void Test()
        {
            var st = new SegmentTree<long>(5, (a, b) => a + b, 0);
            st[0] = 4;
            st[1] = 3;
            st[2] = 2;
            st[3] = 1;
            st[4] = 0;
            Assert.AreEqual(3, st.Query(2, 5));
        }

        [TestMethod]
        public void RandomTest()
        {
            var st = new SegmentTree<long>(N, Math.Min, long.MaxValue);
            var array = new long[N];
            for (int i = 0; i < N; i++)
            {
                array[i] = long.MaxValue;
            }

            var rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var type = rnd.Next(2);
                int index = rnd.Next(N);
                long value = rnd.Next();
                int left = rnd.Next(N);
                int right = rnd.Next(N);
                if (left > right)
                {
                    int t = left;
                    left = right;
                    right = t;
                }

                switch (type)
                {
                    case 0:
                        st[index] = value;
                        array[index] = value;
                        break;
                    case 1:
                        long min = long.MaxValue;
                        for (int j = left; j < right; j++)
                        {
                            min = Math.Min(min, array[j]);
                        }

                        Assert.AreEqual(min, st.Query(left, right));
                        break;
                }
            }
        }
    }
}