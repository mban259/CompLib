using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CompLib.Persistent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Persistent
{
    [TestClass]
    public class PersistentArrayTest
    {
        [TestMethod]
        public void AssignmentTest()
        {
            var ar = new PersistentArray<int>();
            ar[1] = 2;
            Assert.AreEqual(ar[1], 2);
            Assert.AreEqual(ar[2], 0);
            ar[2] = 3;
            Assert.AreEqual(ar[1], 2);
            Assert.AreEqual(ar[2], 3);
            ar[2] = 1;
            Assert.AreEqual(ar[2], 1);
        }

        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random();
            const int Len = 100;
            var primitive = new int[Len];
            List<int[]> ls = new List<int[]>();

            var pArray = new PersistentArray<int>();

            for (int i = 0; i < 100000; i++)
            {
                int type = rnd.Next(5);
                if (type == 0)
                {
                    // 代入
                    int index = rnd.Next(Len);
                    int value = rnd.Next();
                    pArray[index] = value;
                    primitive[index] = value;
                }
                else if (type == 1)
                {
                    // 取得
                    int index = rnd.Next(Len);
                    Assert.AreEqual(primitive[index], pArray[index]);
                }
                else if (type == 2)
                {
                    // セーブ
                    pArray.Save();
                    ls.Add(primitive.ToArray());
                    Assert.AreEqual(ls.Count, pArray.Version);
                }
                else if (type == 3)
                {
                    // 復元
                    Assert.AreEqual(ls.Count, pArray.Version);
                    int version = rnd.Next(ls.Count + 1);
                    if (version < ls.Count)
                    {
                        primitive = ls[version].ToArray();
                        pArray.Restore(version);
                    }
                    else
                    {
                        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                        {
                            pArray.Restore(version);
                        });
                    }
                }
                else if (type == 4)
                {
                    Assert.AreEqual(ls.Count, pArray.Version);
                    int version = rnd.Next(ls.Count + 50);
                    int index = rnd.Next(Len);
                    if (version < ls.Count)
                    {
                        Assert.AreEqual(ls[version][index], pArray.Get(version, index));
                    }
                    else
                    {
                        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                        {
                            pArray.Get(version, index);
                        });
                    }
                }
            }
        }
    }
}