using System;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class RangeMinimumQueryTest
    {
        private const int N = 1 << 21;

        [TestMethod]
        public void RandomTest()
        {
            var st = new RangeMinimumQuery<long>(long.MaxValue);
            var array = new long[N];
            for (int i = 0; i < N; i++)
            {
                array[i] = long.MaxValue;
            }

            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int type = rnd.Next(2);
                int index = rnd.Next(100);
                int value = rnd.Next();
                int left = rnd.Next(100);
                int right = rnd.Next(100);
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

                        Assert.AreEqual(min, st.Minimum(left, right));
                        break;
                }
            }
        }
    }
}