using System;
using System.Collections.Generic;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class LargeRangeUpdateQueryTest
    {
        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 10000; i++)
            {
                int type = rnd.Next(2);
                int n = rnd.Next(100);
                int r = rnd.Next(10000);
                int l = rnd.Next(r);
                int index = rnd.Next();
                var map = new Dictionary<long, int>();
                var ruq = new LargeRangeUpdateQuery<int>();
                switch (type)
                {
                    case 0:
                        for (int j = l; j < r; j++)
                        {
                            map[j] = n;
                        }

                        ruq.Update(l, r, n);
                        // update
                        break;
                    case 1:
                        // get
                        int mp;
                        map.TryGetValue(index, out mp);
                        int q = ruq.Get(index);
                        Assert.AreEqual(mp, q);
                        break;
                }
            }
        }
    }
}