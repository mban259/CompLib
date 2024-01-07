namespace CompLib.Persistent
{
    using System;
    using System.Collections.Generic;
    public class PersistentArray<T>
    {
        private readonly List<Node> _roots;
        private Node _current;
        public PersistentArray()
        {
            _roots = new List<Node>();
            _current = new Node(_roots.Count);
        }

        public T this[int i]
        {
            get { return _current.Get(i); }
            set { _current.Set(i, value); }
        }

        /// <summary>
        /// 指定されたバージョンの要素を返す
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public T Get(int ver, int i)
        {
            return _roots[ver].Get(i);
        }

        /// <summary>
        /// 配列を保存し、バージョンを返す
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            _roots.Add(_current);
            _current = _current.Copy(_roots.Count);
            return _roots.Count - 1;
        }

        /// <summary>
        /// 配列を復元する
        /// </summary>
        /// <param name="ver">復元するバージョン</param>
        public void Restore(int ver)
        {
            _current = _roots[ver].Copy(Version);
        }

        public int Version
        {
            get { return _roots.Count; }
        }

        class Node
        {
            private const int Split = 20;
            public int _ver;
            public T _item;
            public Node[] _child;

            public Node(int ver)
            {
                _ver = ver;
                _item = default(T);
                _child = new Node[Split];
            }

            public Node(int ver, T item, Node[] ch)
            {
                _ver = ver;
                _item = item;
                _child = ch;
            }

            public T Get(int i)
            {
                if (i == 0) return _item;
                if (_child[i % Split] == null) return default(T);
                return _child[i % Split].Get((i - 1) / Split);
            }

            public void Set(int i, T item)
            {
                if (i == 0)
                {
                    _item = item;
                    return;
                }
                if (_child[i % Split] == null)
                {
                    _child[i % Split] = new Node(_ver);
                }
                else if (_child[i % Split]._ver != _ver)
                {
                    _child[i % Split] = _child[i % Split].Copy(_ver);
                }
                _child[i % Split].Set((i - 1) / Split, item);
            }

            public Node Copy(int newVer)
            {
                var ch = new Node[Split];
                Array.Copy(_child, ch, Split);
                return new Node(newVer, _item, ch);
            }

        }
    }
}