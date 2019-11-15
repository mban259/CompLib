using System;
using CompLib.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections
{
    [TestClass]
    public class LazySegmentTreeTest
    {
        [TestMethod]
        public void RangeSumQueryTest()
        {
            AddQueryTest1();
            RangeAddQueryTest1();
            UpdateQueryTest1();
            RangeUpdateQueryTest1();
            CopyArrayTest1();
            RandomTest1();
        }

        [TestMethod]
        public void RangeMinimumQuery()
        {
            UpdateQueryTest2();
            RangeUpdateQueryTest2();
            CopyArrayTest2();
            RandomTest2();
        }

        [TestMethod]
        public void RangeMinimumQueryB()
        {
            AddQueryTest2();
            RangeAddQueryTest3();
            UpdateQueryTest3();
            RangeUpdateQueryTest3();
            CopyArrayTest3();
            RandomTest3();
        }

        public void AddQueryTest1()
        {
            var st = new LazySegmentTreeRsq(10);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Add(0, 9);
            st.Add(3, 7);
            // [9, 0, 0, 7, 0, 0, 0, 0, 0, 0]

            Assert.AreEqual(16, st.Sum(0, 5));
        }

        public void RangeAddQueryTest1()
        {
            var st = new LazySegmentTreeRsq(10);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Add(3, 7, 5);
            // [0, 0, 0, 5, 5, 5, 5, 0, 0, 0]

            st.Add(0, 5, 3);
            // [3, 3, 3, 8, 8, 5, 5, 0, 0, 0]

            Assert.AreEqual(27, st.Sum(1, 6));
        }

        public void UpdateQueryTest1()
        {
            var st = new LazySegmentTreeRsq(10);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Update(0, 9);
            st.Update(3, 7);
            st.Update(0, 3);
            // [3, 0, 0, 7, 0, 0, 0, 0, 0, 0]

            Assert.AreEqual(10, st.Sum(0, 5));
        }

        public void RangeUpdateQueryTest1()
        {
            var st = new LazySegmentTreeRsq(10);
            // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

            st.Update(3, 7, 5);
            // [0, 0, 0, 5, 5, 5, 5, 0, 0, 0]

            st.Update(0, 5, 3);
            // [3, 3, 3, 3, 3, 5, 5, 0, 0, 0]

            Assert.AreEqual(17, st.Sum(1, 6));
        }

        public void CopyArrayTest1()
        {
            var array = new long[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            var st = new LazySegmentTreeRsq(array);
            Assert.AreEqual(27, st.Sum(2, 8));
        }

        public void RandomTest1()
        {
            for (int i = 0; i < 100; i++)
            {
                RangeSumQuery(new LazySegmentTreeRsq(1000), new long[1000]);
            }

            var rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                var array = new long[1000];
                for (int j = 0; j < 1000; j++)
                {
                    array[j] = rnd.Next(-10000, 10000);
                }

                RangeSumQuery(new LazySegmentTreeRsq(array), array);
            }
        }

        private void RangeSumQuery(LazySegmentTreeRsq st, long[] array)
        {
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
                        // 1つ加算
                        array[index] += v;
                        st.Add(index, v);
                        break;
                    case 1:
                        // 範囲加算
                        for (int i = l; i < r; i++)
                        {
                            array[i] += v;
                        }

                        st.Add(l, r, v);
                        break;
                    case 2:
                        // 1つ更新
                        array[index] = v;
                        st.Update(index, v);
                        break;
                    case 3:
                        // 範囲更新
                        for (int i = l; i < r; i++)
                        {
                            array[i] = v;
                        }

                        st.Update(l, r, v);
                        break;
                    case 4:
                        // 合計を求める
                        long sum = 0;
                        for (int i = l; i < r; i++)
                        {
                            sum += array[i];
                        }

                        Assert.AreEqual(sum, st.Sum(l, r));
                        break;
                }
            }
        }

        public void UpdateQueryTest2()
        {
            var st = new LazySegmentTreeRmq<int>(10, int.MaxValue);
            st.Update(3, 10);
            st.Update(6, 99);
            st.Update(2, 1);
            // [Inf, Inf, 1, 10, Inf, Inf, 99, Inf, Inf, Inf]

            Assert.AreEqual(1, st.Minimum(1, 5));
        }

        public void RangeUpdateQueryTest2()
        {
            var st = new LazySegmentTreeRmq<int>(10, int.MaxValue);
            st.Update(0, 10, 6);
            // [6, 6, 6, 6, 6, 6, 6, 6, 6, 6]

            st.Update(4, 7, 24);
            // [6, 6, 6, 6, 24, 24, 24, 6, 6, 6]

            st.Update(7, 9, 4);
            // [6, 6, 6, 6, 24, 24, 24, 4, 4, 6]

            Assert.AreEqual(4, st.Minimum(3, 10));
        }

        public void CopyArrayTest2()
        {
            var array = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            var st = new LazySegmentTreeRmq<int>(array, int.MaxValue);
            Assert.AreEqual(2, st.Minimum(2, 8));
        }

        public void RandomTest2()
        {
            for (int i = 0; i < 1000; i++)
            {
                var array = new int[1000];
                for (int j = 0; j < 1000; j++)
                {
                    array[j] = int.MaxValue;
                }

                RangeMinimumQuery(new LazySegmentTreeRmq<int>(1000, int.MaxValue), array);
            }

            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var array = new int[1000];
                for (int j = 0; j < 1000; j++)
                {
                    array[j] = rnd.Next(-10000, 10000);
                }

                RangeMinimumQuery(new LazySegmentTreeRmq<int>(array, int.MaxValue), array);
            }
        }

        private void RangeMinimumQuery(LazySegmentTreeRmq<int> st, int[] array)
        {
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
                        int min = int.MaxValue;

                        for (int i = l; i < r; i++)
                        {
                            min = Math.Min(min, array[i]);
                        }

                        Assert.AreEqual(min, st.Minimum(l, r));
                        break;
                }
            }
        }

        public void AddQueryTest2()
        {
            var st = new LazySegmentTreeRmq(10);
            st.Add(0, 50);
            st.Add(1, 63);
            st.Add(2, 67);
            st.Add(3, 10);
            st.Add(4, 29);
            st.Add(2, 3);

            // [50, 63, 70, 10, 29, 0, 99, 0, 0, 0]

            Assert.AreEqual(10, st.Minimum(1, 5));
        }

        public void RangeAddQueryTest3()
        {
            var st = new LazySegmentTreeRmq(10);
            st.Add(0, 10, 6);
            // [6, 6, 6, 6, 6, 6, 6, 6, 6, 6]

            st.Add(4, 7, 24);
            // [6, 6, 6, 6, 30, 30, 30, 6, 6, 6]

            st.Add(7, 9, 4);
            // [6, 6, 6, 6, 30, 30, 30, 10, 10, 6]

            Assert.AreEqual(6, st.Minimum(3, 10));
        }

        public void UpdateQueryTest3()
        {
            var st = new LazySegmentTreeRmq(10);
            st.Update(0, 50);
            st.Update(1, 63);
            st.Update(2, 67);
            st.Update(3, 10);
            st.Update(4, 29);
            st.Update(2, 3);

            // [50, 63, 3, 10, 29, 0, 99, 0, 0, 0]

            Assert.AreEqual(3, st.Minimum(1, 5));
        }

        public void RangeUpdateQueryTest3()
        {
            var st = new LazySegmentTreeRmq(10);
            st.Update(0, 10, 6);
            // [6, 6, 6, 6, 6, 6, 6, 6, 6, 6]

            st.Update(4, 7, 24);
            // [6, 6, 6, 6, 24, 24, 24, 6, 6, 6]

            st.Update(7, 9, 4);
            // [6, 6, 6, 6, 24, 24, 24, 4, 4, 6]

            Assert.AreEqual(4, st.Minimum(3, 10));
        }

        public void CopyArrayTest3()
        {
            var array = new long[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            var st = new LazySegmentTreeRmq(array);
            Assert.AreEqual(2, st.Minimum(2, 8));
        }

        public void RandomTest3()
        {
            var array = new long[1000];
            RangeMinimumQueryB(new LazySegmentTreeRmq(1000), array);

            var rnd = new Random();
            array = new long[1000];
            for (int j = 0; j < 1000; j++)
            {
                array[j] = rnd.Next(-10000, 10000);
            }

            RangeMinimumQueryB(new LazySegmentTreeRmq(array), array);
        }

        private void RangeMinimumQueryB(LazySegmentTreeRmq st, long[] array)
        {
            var rnd = new Random();
            for (int j = 0; j < 100000; j++)
            {
                int type = rnd.Next(5);
                int index = rnd.Next(1000);
                int l = rnd.Next(1000);
                int r = rnd.Next(1000);
                if (r < l)
                {
                    int t = l;
                    r = t;
                    l = r;
                }

                int v = rnd.Next(-10000, 10000);
                switch (type)
                {
                    case 0:
                        // 1つだけ更新
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
                        // 1つ加算
                        array[index] += v;
                        st.Add(index, v);
                        break;
                    case 3:
                        // 範囲加算
                        for (int i = l; i < r; i++)
                        {
                            array[i] += v;
                        }

                        st.Add(l, r, v);
                        break;
                    case 4:
                        // 最小値
                        long min = long.MaxValue;

                        for (int i = l; i < r; i++)
                        {
                            min = Math.Min(min, array[i]);
                        }

                        Assert.AreEqual(min, st.Minimum(l, r));
                        break;
                }
            }
        }
    }
}