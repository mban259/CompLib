using System;
using System.Collections.Generic;
using CompLib.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Algorithm
{
    [TestClass]
    public class TreeTest
    {
        [TestMethod]
        public void Test1()
        {
            var edges = new List<int>[5];

            for (int i = 0; i < 5; i++)
            {
                edges[i] = new List<int>();
            }

            edges[0].Add(1);
            edges[1].Add(0);

            edges[1].Add(2);
            edges[2].Add(1);

            edges[2].Add(3);
            edges[3].Add(2);

            edges[2].Add(4);
            edges[4].Add(2);

            /*
             * 0 -- 1 -- 2 -- 3
             *           |
             *           4
             */

            var tree = new TreeEx(edges, 0);

            Assert.AreEqual(0, tree.Depth(0));
            Assert.AreEqual(1, tree.Depth(1));
            Assert.AreEqual(2, tree.Depth(2));
            Assert.AreEqual(3, tree.Depth(3));
            Assert.AreEqual(3, tree.Depth(4));

            Assert.AreEqual(1, tree.Ancestor(3, 2));
        }
    }
}