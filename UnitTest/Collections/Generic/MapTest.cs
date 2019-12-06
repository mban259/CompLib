using System;
using System.Collections.Generic;
using System.Linq;
using CompLib.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Collections.Generic
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void RandomTest()
        {
            var r = new Random();
            var dictionary = new Dictionary<int, int>();
            var map = new Map<int, int>();
            for (int i = 0; i < 2000000; i++)
            {
                int type = r.Next(3);
                var key = r.Next(1000);
                var value = r.Next();
                switch (type)
                {
                    // 更新
                    case 0:
                        map[key] = value;
                        dictionary[key] = value;
                        break;
                    // 削除
                    case 1:
                    {
                        bool f1 = dictionary.Remove(key);
                        bool f2 = map.Erase(key);
                        Assert.AreEqual(f1, f2);
                    }
                        break;
                    // 取得
                    case 2:
                    {
                        int o;
                        bool f1 = dictionary.TryGetValue(key, out o);
                        bool f2 = dictionary.ContainsKey(key);
                        Assert.AreEqual(f1, f2);
                        Assert.AreEqual(o, map[key]);
                    }
                        break;
                }

                Assert.AreEqual(dictionary.Count, map.Count, $"{type}");
            }
        }

        [TestMethod]
        public void LowerBoundTest()
        {
            KeyValuePair<int, int>[] array;
            MakeKeyValueArray(out array);
            var map = new Map<int, int>();

            foreach (KeyValuePair<int, int> pair in array)
            {
                Assert.IsTrue(map.Add(pair.Key, pair.Value));
            }

            Array.Sort(array, (a, b) => a.Key.CompareTo(b.Key));

            var rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                int t = rnd.Next();
                int arrayIndex;
                if (array[array.Length - 1].Key < t)
                {
                    arrayIndex = array.Length;
                }
                else
                {
                    int ng = -1;
                    int ok = array.Length - 1;
                    while (ok - ng > 1)
                    {
                        int med = (ng + ok) / 2;
                        if (array[med].Key >= t)
                        {
                            ok = med;
                        }
                        else
                        {
                            ng = med;
                        }
                    }

                    arrayIndex = ok;
                }

                Assert.AreEqual(arrayIndex, map.LowerBound(t));
                if (arrayIndex < array.Length)
                {
                    Assert.AreEqual(array[arrayIndex].Value, map.Get(arrayIndex).Value);
                }
                else
                {
                    Assert.ThrowsException<NullReferenceException>(() => map.Get(arrayIndex));
                }
            }
        }

        [TestMethod]
        public void UpperBoundTest()
        {
            KeyValuePair<int, int>[] array;
            MakeKeyValueArray(out array);
            var map = new Map<int, int>();

            foreach (KeyValuePair<int, int> pair in array)
            {
                Assert.IsTrue(map.Add(pair.Key, pair.Value));
            }

            Array.Sort(array, (a, b) => a.Key.CompareTo(b.Key));

            var rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                int t = rnd.Next();
                int arrayIndex;
                if (array[array.Length - 1].Key <= t)
                {
                    arrayIndex = array.Length;
                }
                else
                {
                    int ng = -1;
                    int ok = array.Length - 1;
                    while (ok - ng > 1)
                    {
                        int med = (ng + ok) / 2;
                        if (array[med].Key > t)
                        {
                            ok = med;
                        }
                        else
                        {
                            ng = med;
                        }
                    }

                    arrayIndex = ok;
                }

                Assert.AreEqual(arrayIndex, map.UpperBound(t));
                if (arrayIndex < array.Length)
                {
                    Assert.AreEqual(array[arrayIndex].Value, map.Get(arrayIndex).Value);
                }
                else
                {
                    Assert.ThrowsException<NullReferenceException>(() => map.Get(arrayIndex));
                }
            }
        }

        public void MakeKeyValueArray(out KeyValuePair<int, int>[] kv)
        {
            var rnd = new Random();
            var hs = new HashSet<int>();
            for (int i = 0; i < 1000000; i++)
            {
                hs.Add(rnd.Next((int) 1e9));
            }

            var keyArray = hs.ToArray();
            kv = new KeyValuePair<int, int>[keyArray.Length];
            for (int i = 0; i < keyArray.Length; i++)
            {
                kv[i] = new KeyValuePair<int, int>(keyArray[i], rnd.Next());
            }
        }
    }
}