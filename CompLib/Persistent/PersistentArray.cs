namespace CompLib.Persistent
{
    using System;
    using System.Collections.Generic;
    public class PersistentArray<T>
    {
        private readonly List<(T item, int ver, int l, int r)> _nodes;
        private readonly List<int> _roots;
        private int _cur;
        public PersistentArray()
        {
            _cur = 0;
            _nodes = new List<(T item, int ver, int l, int r)>() { (default(T), 0, -1, -1) };
            _roots = new List<int>();
        }

        public T this[int i]
        {
            get { return GetItem(_cur, i); }
            set { SetItem(_cur, i, value); }
        }

        private T GetItem(int cur, int i)
        {
            while (i != 0)
            {
                cur = i % 2 == 0 ? _nodes[cur].l : _nodes[cur].r;
                i = (i - 1) >> 1;
                if (cur == -1) return default(T);
            }
            return _nodes[cur].item;
        }

        private void SetItem(int cur, int i, T item)
        {
            while (i != 0)
            {
                (T curItem, int curVer, int curL, int curR) = _nodes[cur];

                if (i % 2 == 0)
                {
                    // l
                    if (curL == -1)
                    {
                        _nodes.Add((default(T), Version, -1, -1));
                        curL = _nodes.Count - 1;
                        _nodes[cur] = (curItem, curVer, curL, curR);
                    }
                    else if (_nodes[curL].ver != Version)
                    {
                        (T toItem, _, int toL, int toR) = _nodes[curL];
                        _nodes.Add((toItem, Version, toL, toR));
                        curL = _nodes.Count - 1;
                        _nodes[cur] = (curItem, curVer, curL, curR);
                    }
                    cur = curL;
                    i = (i - 1) >> 1;
                }
                else
                {
                    if (curR == -1)
                    {
                        _nodes.Add((default(T), Version, -1, -1));
                        curR = _nodes.Count - 1;
                        _nodes[cur] = (curItem, curVer, curL, curR);
                    }
                    else if (_nodes[curR].ver != Version)
                    {
                        (T toItem, _, int toL, int toR) = _nodes[curR];
                        _nodes.Add((toItem, Version, toL, toR));
                        curR = _nodes.Count - 1;
                        _nodes[cur] = (curItem, curVer, curL, curR);
                    }
                    cur = curR;
                    i = (i - 1) >> 1;
                }
            }

            (_, int tmpVer2, int tmpL2, int tmpR2) = _nodes[cur];
            _nodes[cur] = (item, tmpVer2, tmpL2, tmpR2);
        }

        /// <summary>
        /// 指定されたバージョンの要素を返す
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public T Get(int ver, int i)
        {
            return GetItem(_roots[ver], i);
        }

        /// <summary>
        /// 配列を保存し、バージョンをす
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            (T curItem, _, int curL, int curR) = _nodes[_cur];
            _roots.Add(_cur);
            _nodes.Add((curItem, Version, curL, curR));
            _cur = _nodes.Count - 1;
            return _roots.Count - 1;
        }

        /// <summary>
        /// 配列を復元する
        /// </summary>
        /// <param name="ver">復元するバージョン</param>
        public void Restore(int ver)
        {
            (T curItem, _, int curL, int curR) = _nodes[_roots[ver]];
            _nodes.Add((curItem, Version, curL, curR));
            _cur = _nodes.Count - 1;
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