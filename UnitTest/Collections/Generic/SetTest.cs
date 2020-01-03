using System;
using System.Collections.Generic;
using System.Linq;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class SetTest
    {
        #region Set

        [TestMethod]
        public void Test1()
        {
            var set = new Set<int>();
            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));
            Assert.IsTrue(set.Add(4));
            Assert.IsTrue(set.Add(5));
            Assert.IsFalse(set.Add(3));
            Assert.IsFalse(set.Add(2));
            Assert.AreEqual(5, set.Count());
            var ar = set.ToArray();
            Assert.AreEqual(1, ar[0]);
            Assert.AreEqual(2, ar[1]);
            Assert.AreEqual(3, ar[2]);
            Assert.AreEqual(4, ar[3]);
            Assert.AreEqual(5, ar[4]);
        }

        [TestMethod]
        public void RandomAddTest()
        {
            var hashSet = new HashSet<int>();
            var set = new Set<int>();
            var rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                bool add = rnd.Next(2) == 1;
                int n = rnd.Next(100);
                if (add)
                {
                    Assert.AreEqual(hashSet.Add(n), set.Add(n));
                }
                else
                {
                    Assert.AreEqual(hashSet.Contains(n), set.Contains(n));
                }

                Assert.AreEqual(hashSet.Count, set.Count());
            }
        }

        [TestMethod]
        public void RandomRemoveTest()
        {
            var rnd = new Random();
            var hashset = new HashSet<int>();
            var set = new Set<int>();
            for (int i = 0; i < 200000; i++)
            {
                int type = rnd.Next(3);
                int n = rnd.Next();
                switch (type)
                {
                    case 0:
                        // add
                        Assert.AreEqual(hashset.Add(n), set.Add(n));
                        break;
                    case 1:
                        // remove
                        Assert.AreEqual(hashset.Remove(n), set.Remove(n));
                        break;
                    case 2:
                        // get
                        Assert.AreEqual(hashset.Contains(n), set.Contains(n));
                        break;
                }

                Assert.AreEqual(hashset.Count, set.Count());
            }
        }

        [TestMethod]
        public void RandomRemoveRangeTest()
        {
            var rnd = new Random();
            var hashSet = new HashSet<int>();
            var set = new Set<int>();

            for (int i = 0; i < 10000; i++)
            {
                int type = rnd.Next(3);
                int n = rnd.Next(100);
                int l = rnd.Next(hashSet.Count);
                int r = rnd.Next(l, hashSet.Count);

                switch (type)
                {
                    case 0:
                        // add
                        Assert.AreEqual(hashSet.Add(n), set.Add(n));
                        break;
                    case 1:
                        // remove
                        var ar = hashSet.ToArray();
                        Array.Sort(ar);
                        for (int j = l; j < r; j++)
                        {
                            hashSet.Remove(ar[j]);
                        }

                        set.RemoveRange(l, r);
                        Assert.AreEqual(hashSet.Count, set.Count());
                        break;
                    case 2:
                        Assert.AreEqual(hashSet.Contains(n), set.Contains(n));
                        break;
                }
            }
        }

        [TestMethod]
        public void RandomCountTest()
        {
            var set = new Set<int>();
            var rnd = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                bool add = rnd.Next(2) == 0;
                int n = rnd.Next(1000);
                if (add)
                {
                    set.Add(n);
                }
                else
                {
                    // count
                    Assert.AreEqual(set.Contains(n) ? 1 : 0, set.Count(n));
                }
            }
        }

        [TestMethod]
        public void RandomToArrayTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var set = new Set<int>();
                var hs = new HashSet<int>();
                for (int j = 0; j < 1000; j++)
                {
                    int n = rnd.Next(100);
                    Assert.AreEqual(hs.Add(n), set.Add(n));
                }

                var hsAr = hs.ToArray();
                Array.Sort(hsAr);
                var setAr = set.ToArray();
                Assert.AreEqual(hsAr.Length, set.Count());
                for (int j = 0; j < hsAr.Length; j++)
                {
                    Assert.AreEqual(hsAr[j], setAr[j]);
                }
            }
        }

        [TestMethod]
        public void TupleTest()
        {
            var set = new Set<Tuple<int, int>>((a, b) => a.Item1.CompareTo(b.Item1));

            Assert.IsTrue(set.Add(new Tuple<int, int>(0, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(1, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(2, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(3, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(4, 0)));

            Assert.IsFalse(set.Add(new Tuple<int, int>(0, 1)));
            Assert.IsFalse(set.Add(new Tuple<int, int>(3, 1)));
            Assert.IsFalse(set.Add(new Tuple<int, int>(4, 1)));

            Assert.IsTrue(set.Add(new Tuple<int, int>(5, 1)));

            var ar = set.ToArray();

            Assert.AreEqual(0, ar[0].Item1);
            Assert.AreEqual(0, ar[0].Item2);
            Assert.AreEqual(1, ar[1].Item1);
            Assert.AreEqual(0, ar[1].Item2);
            Assert.AreEqual(2, ar[2].Item1);
            Assert.AreEqual(0, ar[2].Item2);
            Assert.AreEqual(3, ar[3].Item1);
            Assert.AreEqual(0, ar[3].Item2);
            Assert.AreEqual(4, ar[4].Item1);
            Assert.AreEqual(0, ar[4].Item2);
            Assert.AreEqual(5, ar[5].Item1);
            Assert.AreEqual(1, ar[5].Item2);
        }

        #endregion

        #region MultiSet

        [TestMethod]
        public void MultiTest1()
        {
            var set = new Set<int>(true);
            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));
            Assert.IsTrue(set.Add(4));
            Assert.IsTrue(set.Add(5));
            Assert.IsTrue(set.Add(3));
            Assert.IsTrue(set.Add(2));
            Assert.AreEqual(7, set.Count());
            var ar = set.ToArray();
            Assert.AreEqual(1, ar[0]);
            Assert.AreEqual(2, ar[1]);
            Assert.AreEqual(2, ar[2]);
            Assert.AreEqual(3, ar[3]);
            Assert.AreEqual(3, ar[4]);
            Assert.AreEqual(4, ar[5]);
            Assert.AreEqual(5, ar[6]);
        }

        [TestMethod]
        public void MultiRandomAddTest()
        {
            var list = new List<int>();
            var set = new Set<int>(true);
            var rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                bool add = rnd.Next(2) == 1;
                int n = rnd.Next(100);
                if (add)
                {
                    Assert.IsTrue(set.Add(n));
                    list.Add(n);
                }
                else
                {
                    Assert.AreEqual(list.Contains(n), set.Contains(n));
                }

                Assert.AreEqual(list.Count, set.Count());
                list.Sort();
                for (int j = 0; j < list.Count(); j++)
                {
                    Assert.AreEqual(list[i], set[i]);
                }
            }
        }

        [TestMethod]
        public void MultiRandomRemoveTest()
        {
            var rnd = new Random();
            var list = new List<int>();
            var set = new Set<int>();
            for (int i = 0; i < 200000; i++)
            {
                int type = rnd.Next(3);
                int n = rnd.Next();
                switch (type)
                {
                    case 0:
                        // add
                        Assert.IsTrue(set.Add(n));
                        list.Add(n);
                        break;
                    case 1:
                        // remove
                        Assert.AreEqual(list.Remove(n), set.Remove(n));
                        break;
                    case 2:
                        // get
                        Assert.AreEqual(list.Contains(n), set.Contains(n));
                        break;
                }

                Assert.AreEqual(list.Count, set.Count());
                list.Sort();
                for (int j = 0; j < list.Count(); j++)
                {
                    Assert.AreEqual(list[i], set[i]);
                }
            }
        }

        [TestMethod]
        public void MultiRandomRemoveRangeTest()
        {
            var rnd = new Random();
            var list = new List<int>();
            var set = new Set<int>(true);

            for (int i = 0; i < 10000; i++)
            {
                int type = rnd.Next(3);
                int n = rnd.Next(100);
                int l = rnd.Next(list.Count);
                int r = rnd.Next(l, list.Count);

                switch (type)
                {
                    case 0:
                        // add
                        Assert.IsTrue(set.Add(n));
                        break;
                    case 1:
                        // remove
                        list.Sort();
                        for (int j = 0; j < r - l; j++)
                        {
                            list.RemoveAt(l);
                        }

                        set.RemoveRange(l, r);
                        Assert.AreEqual(list.Count, set.Count());
                        break;
                    case 2:
                        Assert.AreEqual(list.Contains(n), set.Contains(n));
                        break;
                }
            }

            Assert.AreEqual(list.Count, set.Count());
            list.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], set[i]);
            }
        }

        [TestMethod]
        public void MultiRandomCountTest()
        {
            var set = new Set<int>();
            var rnd = new Random();
            var list = new List<int>();
            for (int i = 0; i < 1000000; i++)
            {
                bool add = rnd.Next(2) == 0;
                int n = rnd.Next(1000);
                if (add)
                {
                    set.Add(n);
                    list.Add(n);
                }
                else
                {
                    // count
                    Assert.AreEqual(list.Count(x => x == n), set.Count(n));
                }
            }
        }

        [TestMethod]
        public void MultiRandomToArrayTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var set = new Set<int>(true);
                var list = new List<int>();
                for (int j = 0; j < 1000; j++)
                {
                    int n = rnd.Next(100);
                    list.Add(n);
                    set.Add(n);
                }

                list.Sort();
                var setAr = set.ToArray();
                Assert.AreEqual(list.Count, setAr.Length);
                for (int j = 0; j < list.Count; j++)
                {
                    Assert.AreEqual(list[j], setAr[j]);
                }
            }
        }

        [TestMethod]
        public void MultiTupleTest()
        {
            var set = new Set<Tuple<int, int>>((a, b) => a.Item1.CompareTo(b.Item1), true);

            Assert.IsTrue(set.Add(new Tuple<int, int>(0, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(1, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(2, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(3, 0)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(4, 0)));

            Assert.IsTrue(set.Add(new Tuple<int, int>(0, 1)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(3, 1)));
            Assert.IsTrue(set.Add(new Tuple<int, int>(4, 1)));

            Assert.IsTrue(set.Add(new Tuple<int, int>(5, 1)));

            var ar = set.ToArray();

            Assert.AreEqual(0, ar[0].Item1);
            Assert.AreEqual(0, ar[1].Item1);
            Assert.AreEqual(1, ar[2].Item1);
            Assert.AreEqual(2, ar[3].Item1);
            Assert.AreEqual(3, ar[4].Item1);
            Assert.AreEqual(3, ar[5].Item1);
            Assert.AreEqual(4, ar[6].Item1);
            Assert.AreEqual(4, ar[7].Item1);
            Assert.AreEqual(5, ar[8].Item1);
        }

        #endregion
    }
}