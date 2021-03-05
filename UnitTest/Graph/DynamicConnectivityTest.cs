using System.Collections.Generic;
using System.Linq;
using System;
using CompLib.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Graph
{
    [TestClass]
    public class DynamicConnectivityTest
    {
        [TestMethod]
        public void CF100551A()
        {
            var dc = new DynamicConnectivity(5);

            Assert.AreEqual(5, dc.Count);
            Assert.IsTrue(dc.Connect(0, 1));
            Assert.IsTrue(dc.Connect(1, 2));
            Assert.IsTrue(dc.Connect(2, 3));
            Assert.IsTrue(dc.Connect(3, 4));
            Assert.IsFalse(dc.Connect(4, 0));
            Assert.AreEqual(1, dc.Count);
            Assert.IsFalse(dc.Disconnect(1, 2));
            Assert.IsTrue(dc.Disconnect(3, 4));
            Assert.AreEqual(2, dc.Count);
        }

        [TestMethod]
        public void Random()
        {
            const int N = 50000;
            const int M = 50000;
            var rnd = new Random();
            var dc = new DynamicConnectivity(N);
            var edge = new List<(int u, int v)>();
            for (int i = 0; i < M; i++)
            {
                int t = rnd.Next(3);
                if (t == 0 || (t == 1 && edge.Count == 0))
                {
                    int u = rnd.Next(N);
                    int v = rnd.Next(N);
                    if (u > v) (u, v) = (v, u);
                    edge.Add((u, v));
                    dc.Connect(u, v);
                }
                else if (t == 1)
                {
                    var p = rnd.Next(edge.Count);
                    (edge[p], edge[edge.Count - 1]) = (edge[edge.Count - 1], edge[p]);
                    dc.Disconnect(edge[edge.Count - 1].u, edge[edge.Count - 1].v);
                    edge.RemoveAt(edge.Count - 1);
                }
                else
                {
                    int u = rnd.Next(N);
                    int v = rnd.Next(N);
                    if (u > v) (u, v) = (v, u);
                    dc.Same(u, v);
                }
            }
        }
    }
}