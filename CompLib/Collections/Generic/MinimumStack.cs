namespace CompLib.Collections.Generic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 最小値を持てるスタック
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MinimumStack<T>
    {
        private readonly Comparison<T> _compare;
        // 値、最大値
        private readonly Stack<Pair> _stack;

        public MinimumStack(Comparison<T> comparison)
        {
            _compare = comparison;
            _stack = new Stack<Pair>();
        }

        public MinimumStack(IComparer<T> comparer) : this(comparer.Compare) { }

        public MinimumStack() : this(Comparer<T>.Default) { }

        /// <summary>
        /// itemを末尾に追加する
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            if (_stack.Count == 0) _stack.Push(new Pair(item, item));
            else _stack.Push(new Pair(item, _Min(item, _stack.Peek().Second)));
        }

        /// <summary>
        /// 末尾の要素を取り出す
        /// </summary>
        /// <returns></returns>
        public T Pop() => _stack.Pop().First;

        /// <summary>
        /// 末尾の要素を取得する
        /// </summary>
        /// <returns></returns>
        public T Peek() => _stack.Peek().First;

        /// <summary>
        /// 要素の最小値
        /// </summary>
        /// <returns></returns>
        public T Min() => _stack.Peek().Second;

        public int Count => _stack.Count;

        private T _Min(T x, T y)
        {
            return _compare(x, y) <= 0 ? x : y;
        }

        struct Pair
        {
            public T First, Second;
            public Pair(T f, T s)
            {
                First = f;
                Second = s;
            }
        }
    }
}