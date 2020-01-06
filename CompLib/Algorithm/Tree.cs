using System;
using System.Collections.Generic;
using System.Linq;

namespace CompLib.Algorithm
{
    public class Tree
    {
        private readonly List<int>[] _edge;
        protected List<int>[] _child;
        protected readonly int _root;
        protected readonly int _size;

        private readonly int[] _depth;
        private readonly int[] _parent;

        public Tree(List<int>[] edge, int root = 0)
        {
            _root = root;
            _size = edge.Length;
            _edge = edge;
            _child = new List<int>[_size];
            for (int i = 0; i < _size; i++)
            {
                _child[i] = new List<int>();
            }

            _depth = new int[_size];
            _parent = new int[_size];
            _parent[_root] = -1;

            Search(_root, 0, -1);
        }

        private void Search(int i, int d, int p)
        {
            _depth[i] = d;
            _parent[i] = p;

            foreach (int j in _edge[i])
            {
                if (p == j) continue;
                _child[i].Add(j);
                Search(j, d + 1, i);
            }
        }

        /// <summary>
        /// 各頂点の深さ
        /// </summary>
        /// <returns></returns>
        public int[] DepthAll()
        {
            return _depth;
        }

        /// <summary>
        /// 頂点nの深さ
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int Depth(int n)
        {
            return _depth[n];
        }

        /// <summary>
        /// 各頂点の親 根の親は-1
        /// </summary>
        /// <returns></returns>
        public int[] ParentAll()
        {
            return _parent;
        }

        /// <summary>
        /// 頂点nの親 nが根なら-1を返す
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int Parent(int n)
        {
            return _parent[n];
        }

        /// <summary>
        /// 根がnの部分木の頂点集合
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int[] SubTreeNodes(int n)
        {
            var l = new List<int>();
            SearchNodes(n, l);
            return l.ToArray();
        }

        private void SearchNodes(int r, List<int> l)
        {
            l.Add(r);
            foreach (int i in _child[r])
            {
                SearchNodes(i, l);
            }
        }


        public int DeepestSubtreeNode(int r, out int node)
        {
            var l = SubTreeNodes(r);

            int result = Depth(r);
            node = r;

            foreach (int i in l)
            {
                if (Depth(i) > result)
                {
                    result = Depth(i);
                    node = i;
                }
            }

            return result;
        }
    }
}