using System;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class LazySegmentTreeTest
    {
        private const int N = 1 << 21;

        private long Sum(long a, long b)
        {
            return a + b;
        }

        private long Mul(long a, int b)
        {
            return a * b;
        }

        private long Update(long a, long b)
        {
            return b;
        }

        private long Minimum(long a, int b)
        {
            return a;
        }

        [TestMethod]
        public void AddQueryTest1()
        {
            var st = new LazySegmentTree<long>(N, Sum, 0, Mul, Sum, 0);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0...]

            st.Update(0, 9);
            st.Update(3, 7);
            // [9, 0, 0, 7, 0, 0, 0, 0, 0, 0...]

            Assert.AreEqual(16, st.Query(0, 5));
        }

        [TestMethod]
        public void RangeAddQueryTest1()
        {
            var st = new LazySegmentTree<long>(N, Sum, 0, Mul, Sum, 0);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0...]

            st.Update(3, 7, 5);
            // [0, 0, 0, 5, 5, 5, 5, 0, 0, 0...]

            st.Update(0, 5, 3);
            // [3, 3, 3, 8, 8, 5, 5, 0, 0, 0...]
            Assert.AreEqual(27, st.Query(1, 6));
        }

        [TestMethod]
        public void UpdateQueryTest1()
        {
            var st = new LazySegmentTree<long>(N, Sum, 0, Mul, Update, 0);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Update(0, 9);
            st.Update(3, 7);
            st.Update(0, 3);
            // [3, 0, 0, 7, 0, 0, 0, 0, 0, 0]

            Assert.AreEqual(10, st.Query(0, 5));
        }

        [TestMethod]
        public void RangeUpdateQueryTest1()
        {
            var st = new LazySegmentTree<long>(N, Sum, 0, Mul, Update, 0);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Update(3, 7, 5);
            // [0, 0, 0, 5, 5, 5, 5, 0, 0, 0]

            st.Update(0, 5, 3);
            // [3, 3, 3, 3, 3, 5, 5, 0, 0, 0]

            Assert.AreEqual(17, st.Query(1, 6));
        }

        [TestMethod]
        public void RandomTest1()
        {
            for (int i = 0; i < 100; i++)
            {
                RangeSumQuery();
            }
        }

        private void RangeSumQuery()
        {
            var st = new LazySegmentTree<long>(N, Sum, 0, Mul, Update, 0);
            var array = new long[N];
            var rnd = new Random();
            for (int j = 0; j < 100; j++)
            {
                int type = rnd.Next(5);
                int l = rnd.Next(1000);
                int r = rnd.Next(1000);
                if (r < l)
                {
                    int t = l;
                    l = r;
                    r = t;
                }

                int index = rnd.Next(1000);
                long v = rnd.Next(-10000, 10000);
                switch (type)
                {
                    case 0:
                        // 1つ更新
                        array[index] = v;
                        st.Update(index, v);
                        break;
                    case 1:
                        // 範囲更新
                        for (int i = l; i < r; i++)
                        {
                            array[i] = v;
                        }

                        st.Update(l, r, v);
                        break;
                    case 2:
                        // 合計を求める
                        long sum = 0;
                        for (int i = l; i < r; i++)
                        {
                            sum += array[i];
                        }

                        Assert.AreEqual(sum, st.Query(l, r));
                        break;
                }
            }
        }

        [TestMethod]
        public void UpdateQueryTest2()
        {
            var st = new LazySegmentTree<long>(N, Math.Min, long.MaxValue, Minimum, Update, long.MaxValue);
            st.Update(3, 10);
            st.Update(6, 99);
            st.Update(2, 1);
            // [Inf, Inf, 1, 10, Inf, Inf, 99, Inf, Inf, Inf]

            Assert.AreEqual(1, st.Query(1, 5));
        }

        [TestMethod]
        public void RangeUpdateQueryTest2()
        {
            var st = new LazySegmentTree<long>(N, Math.Min, long.MaxValue, Minimum, Update, long.MaxValue);
            st.Update(0, 10, 6);
            // [6, 6, 6, 6, 6, 6, 6, 6, 6, 6]

            st.Update(4, 7, 24);
            // [6, 6, 6, 6, 24, 24, 24, 6, 6, 6]

            st.Update(7, 9, 4);
            // [6, 6, 6, 6, 24, 24, 24, 4, 4, 6]

            Assert.AreEqual(4, st.Query(3, 10));
        }

        [TestMethod]
        public void RandomTest2()
        {
            for (int i = 0; i < 100; i++)
            {
                RangeMinimumQuery();
            }
        }

        private void RangeMinimumQuery()
        {
            var st = new LazySegmentTree<long>(N, Math.Min, long.MaxValue, Minimum, Update, long.MaxValue);
            var array = new long[N];
            for (int i = 0; i < N; i++)
            {
                array[i] = long.MaxValue;
            }

            var rnd = new Random();
            for (int j = 0; j < 100; j++)
            {
                int type = rnd.Next(3);
                int index = rnd.Next(1000);
                int v = rnd.Next(-10000, 10000);
                int l = rnd.Next(1000);
                int r = rnd.Next(1000);
                if (r < l)
                {
                    int t = l;
                    l = r;
                    r = t;
                }

                switch (type)
                {
                    case 0:
                        // 1つだけ更新;
                        array[index] = v;
                        st.Update(index, v);
                        break;
                    case 1:
                        // 範囲更新
                        for (int i = l; i < r; i++)
                        {
                            array[i] = v;
                        }

                        st.Update(l, r, v);
                        break;
                    case 2:
                        // 最小値
                        long min = long.MaxValue;

                        for (int i = l; i < r; i++)
                        {
                            min = Math.Min(min, array[i]);
                        }

                        Assert.AreEqual(min, st.Query(l, r));
                        break;
                }
            }
        }
    }
}