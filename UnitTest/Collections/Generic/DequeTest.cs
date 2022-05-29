using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using CompLib.Collections.Generic;
using System.Diagnostics;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class DequeTest
    {
        [TestMethod]
        public void TestBack()
        {
            var deq = new Deque<int>();
            const int len = 100000;
            var rnd = new Random();
            int[] array = new int[len];
            for (int i = 0; i < len; i++)
            {
                int r = rnd.Next();
                array[i] = r;
                deq.PushBack(r);
            }


            Assert.AreEqual(len, deq.Count);

            for (int i = 0; i < len; i++)
            {
                Assert.AreEqual(array[i], deq[i]);
            }

            for (int i = len - 1; i >= 0; i--)
            {
                Assert.AreEqual(array[i], deq.PopBack());
            }

            Assert.AreEqual(0, deq.Count);

        }

        [TestMethod]
        public void TestFront()
        {
            var deq = new Deque<int>();
            const int len = 100000;
            var rnd = new Random();
            int[] array = new int[len];
            for (int i = 0; i < len; i++)
            {
                int r = rnd.Next();
                array[i] = r;
                deq.PushFront(r);
            }


            Assert.AreEqual(len, deq.Count);

            for (int i = 0; i < len; i++)
            {
                Assert.AreEqual(array[len - i - 1], deq[i]);
            }

            for (int i = len - 1; i >= 0; i--)
            {
                Assert.AreEqual(array[i], deq.PopFront());
            }

            Assert.AreEqual(0, deq.Count);
        }

        [TestMethod]
        public void TestFB()
        {
            var deq = new Deque<int>();
            var ls = new LinkedList<int>();
            const int len = 100000;
            var rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                int r = rnd.Next();
                if (rnd.Next() % 2 == 0)
                {
                    deq.PushFront(r);
                    ls.AddFirst(r);
                }
                else
                {
                    deq.PushBack(r);
                    ls.AddLast(r);
                }
            }

            Assert.AreEqual(len, deq.Count);

            for (int i = 0; i < len; i++)
            {
                Assert.AreEqual(ls.First.Value, deq.First);
                Assert.AreEqual(ls.Last.Value, deq.Last);
                if (rnd.Next() % 2 == 0)
                {
                    Assert.AreEqual(ls.First.Value, deq.PopFront());
                    ls.RemoveFirst();
                }
                else
                {
                    Assert.AreEqual(ls.Last.Value, deq.PopBack());
                    ls.RemoveLast();
                }
            }

            Assert.AreEqual(0, deq.Count);
        }

        [TestMethod]
        public void InsertTest()
        {
            int len = 1000;
            var ls = new List<int>();
            var deq = new Deque<int>();
            var rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = rnd.Next(0, ls.Count + 1);
                int r = rnd.Next();
                deq.Insert(index, r);
                ls.Insert(index, r);
            }

            Assert.AreEqual(len, deq.Count);

            for (int i = 0; i < len; i++)
            {
                Assert.AreEqual(ls[i], deq[i]);
            }
        }

        [TestMethod]
        public void RemoveAtTest()
        {
            int len = 1000;
            var ls = new List<int>();
            var deq = new Deque<int>();
            var rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                int r = rnd.Next();
                if (rnd.Next() % 2 == 0)
                {
                    deq.PushFront(r);
                    ls.Insert(0, r);
                }
                else
                {
                    deq.PushBack(r);
                    ls.Add(r);
                }
            }

            Assert.AreEqual(len, deq.Count);

            for (int i = len; i >= 1; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    Assert.AreEqual(ls[j], deq[j]);
                }

                int index = rnd.Next(deq.Count);
                ls.RemoveAt(index);
                deq.RemoveAt(index);
            }

            Assert.AreEqual(0, deq.Count);
        }


    }
}
