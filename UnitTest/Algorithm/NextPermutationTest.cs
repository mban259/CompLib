using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Algorithm
{
    using Algorithm = CompLib.Algorithm.Algorithm;

    [TestClass]
    public class NextPermutationTest
    {
        [TestMethod]
        public void Test1()
        {
            var array = new[] {1, 2, 3};

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("1 3 2", string.Join(" ", array));

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("2 1 3", string.Join(" ", array));

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("2 3 1", string.Join(" ", array));

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("3 1 2", string.Join(" ", array));

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("3 2 1", string.Join(" ", array));

            Assert.AreEqual(false, Algorithm.NextPermutation(array));
        }

        [TestMethod]
        public void Test2()
        {
            var array = new[] {3, 3, 4};

            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("3 4 3", string.Join(" ", array));


            Assert.AreEqual(true, Algorithm.NextPermutation(array));
            Assert.AreEqual("4 3 3", string.Join(" ", array));

            Assert.AreEqual(false, Algorithm.NextPermutation(array));
        }

        [TestMethod]
        public void Test3()
        {
            var array = new[] {9};
            Assert.AreEqual(false, Algorithm.NextPermutation(array));
        }

        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                var array = new int[7];
                for (int j = 0; j < 7; j++)
                {
                    array[j] = rnd.Next(1000);
                }

                Array.Sort(array);

                var hs = new HashSet<string>();

                var list = Search(array, new bool[7]);

                list.Sort((a, b) =>
                {
                    for (int j = 0; j < a.Count; j++)
                    {
                        if (a[j] != b[j])
                        {
                            return a[j].CompareTo(b[j]);
                        }
                    }

                    return 0;
                });

                foreach (List<int> l in list)
                {
                    hs.Add(string.Join(" ", l));
                }

                var pArray = hs.ToArray();

                int index = 0;

                do
                {
                    Assert.AreEqual(pArray[index], string.Join(" ", array));
                    index++;
                } while (Algorithm.NextPermutation(array));

                Assert.AreEqual(index, pArray.Length);
                Assert.AreEqual(false, Algorithm.NextPermutation(array));
            }
        }

        private List<List<int>> Search(int[] array, bool[] flag)
        {
            var result = new List<List<int>>();
            bool f = true;
            for (int i = 0; i < array.Length; i++)
            {
                if (!flag[i])
                {
                    f = false;
                    flag[i] = true;
                    var l = Search(array, flag);
                    flag[i] = false;
                    foreach (List<int> list in l)
                    {
                        var ll = list.ToList();
                        ll.Add(array[i]);
                        result.Add(ll);
                    }
                }
            }

            if (f)
                result.Add(new List<int>());
            return result;
        }
    }
}