using System;
using System.Collections.Generic;
using CompLib.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Algorithm
{
    [TestClass]
    public class TreeTest
    {
        private const int MaxTreeSize = 200000;

        private List<int>[] RandomTreeEdge, LineTreeEdge;

        [ClassInitialize]
        public void Init()
        {
            var rnd = new Random();
            int rtSize = rnd.Next(MaxTreeSize / 2, MaxTreeSize);
            int ltSize = rnd.Next(MaxTreeSize / 2, MaxTreeSize);
            var rt = new MakeTree(rtSize);
            var lt = new MakeTree(ltSize);
            for (int i = 0; i < rtSize - 1; i++)
            {
                while (true)
                {
                    int a = rnd.Next(rtSize);
                    int b = rnd.Next(rtSize);
                    if (rt.Connect(a, b))
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < ltSize - 1; i++)
            {
                lt.Connect(i, i + 1);
            }

            RandomTreeEdge = rt.ToEdges();
            LineTreeEdge = lt.ToEdges();
        }

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

        [TestMethod]
        public void RandomTest()
        {
        }
    }

    class MakeTree
    {
        private readonly int[] _parent;
        private readonly int[] _rank;
        private readonly List<int>[] _edge;

        public MakeTree(int size)
        {
            _parent = new int[size];
            _rank = new int[size];
            for (int i = 0; i < size; i++)
            {
                _parent[i] = i;
            }

            _edge = new List<int>[size];
            for (int i = 0; i < size; i++)
            {
                _edge[i] = new List<int>();
            }
        }

        public bool Connect(int a, int b)
        {
            a = Root(a);
            b = Root(b);
            if (a == b) return false;

            if (_rank[a] < _rank[b])
            {
                _parent[a] = b;
            }
            else
            {
                _parent[b] = a;
                if (_rank[a] == _rank[b])
                {
                    _rank[a]++;
                }
            }

            _edge[a].Add(b);
            _edge[b].Add(a);
            return true;
        }

        public bool Connected(int a, int b)
        {
            return Root(a) == Root(b);
        }

        private int Root(int i)
        {
            if (_parent[i] == i)
            {
                return i;
            }

            return Root(_parent[i]);
        }

        public List<int>[] ToEdges()
        {
            return _edge;
        }
    }
}
