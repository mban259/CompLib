using System;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class RangeUpdateQueryTest
    {
        private const int N = 1 << 21;

        [TestMethod]
        public void RandomTest()
        {
            var st = new RangeUpdateQuery<long>((a, b) => b, 0);
            var array = new long[N];
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int type = rnd.Next(2);
                int left = rnd.Next(1000);
                int right = rnd.Next(1000);
                if (left > right)
                {
                    int t = left;
                    left = right;
                    right = t;
                }

                long value = rnd.Next();
                int index = rnd.Next(1000);

                switch (type)
                {
                    case 0:
                        for (int j = left; j < right; j++)
                        {
                            array[j] = value;
                        }

                        st.Update(left, right, value);
                        break;
                    case 1:
                        Assert.AreEqual(array[index], st[index]);
                        break;
                }
            }

            st.Update(0, N, 0);
            for (int i = 0; i < N; i++)
            {
                Assert.AreEqual(0, st[i]);
            }
        }
    }
}