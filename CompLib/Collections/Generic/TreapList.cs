using System;

namespace CompLib.Collections.Generic
{
    public class TreapList<T>
    {
        private Node _root;
        private readonly Random _random;

        public TreapList()
        {
            _random = new Random();
        }

        private void Split(Node n,int index, out Node left, out Node right)
        {
            
        }

        private void Insert(ref Node n, int index, Node node)
        {
            if (n == null)
            {
                n = node;
            }

            if (n.Rank < node.Rank)
            {
                
            }

            else
            {
                
            }
        }

        public void Insert(int index, T item)
        {
            var node = new Node(item, _random.Next());
            Insert(ref _root, index, node);
        }

        public void Add(T item)
        {
            Insert(Count, item);
        }

        public bool Remove(int index)
        {
        }

        private Node Get(Node n, int i)
        {
            int left = n.LeftCount();
            if (i < left)
            {
                return Get(n.Left, i);
            }

            if (i == left)
            {
                return n;
            }

            // i > left
            return Get(n.Right, i - left - 1);
        }

        private Node Get(int i)
        {
            if (i >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            return Get(_root, i);
        }

        public T this[int i]
        {
            set { Get(i).Value = value; }
            get { return Get(i).Value; }
        }

        public int Count => _root?.Count ?? 0;

        private class Node
        {
            public readonly int Rank;
            public T Value { get; set; }

            public Node Left, Right;
            public int Count { get; private set; }

            public int LeftCount()
            {
                return Left?.Count ?? 0;
            }

            public void Update()
            {
                Count = LeftCount() + 1 + (Right?.Count ?? 0);
            }

            public Node(T v, int rank)
            {
                Value = v;
                Rank = rank;
            }
        }
    }
}