using System;
using CompLib.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections
{
    [TestClass]
    public class RangeSumQueryTest
    {
        private int N = 1 << 21;

        [TestMethod]
        public void RandomTest()
        {
            var st = new RangeSumQuery();
            var array = new long[N];

            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int type = rnd.Next(2);
                int index = rnd.Next(1000);
                long value = rnd.Next();
                int left = rnd.Next(1000);
                int right = rnd.Next(1000);
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
                        long sum = 0;
                        for (int j = left; j < right; j++)
                        {
                            sum += array[j];
                        }

                        Assert.AreEqual(sum, st.Sum(left, right));
                        break;
                }
            }
        }
    }
}