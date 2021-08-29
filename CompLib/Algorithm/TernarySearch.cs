namespace CompLib.Algorithm
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    static partial class Algorithm
    {
        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="begin">区間始め</param>
        /// <param name="end">区間終わり</param>
        /// <param name="f"></param>
        /// <param name="cmp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, long idx) TernarySearchL<T>(long begin, long end, Func<long, T> f, Comparison<T> cmp)
        {
            Debug.Assert(end - begin >= 1);
            if (end - begin == 1)
            {
                return (f(begin), begin);
            }

            var fib = new List<long>() { 1, 2 };
            for (int i = 2; ; i++)
            {
                fib.Add(fib[i - 1] + fib[i - 2]);
                if (fib[i] >= end - begin) break;
            }

            var map = new Dictionary<long, T>();
            T g(long num) => map.TryGetValue(num, out T o) ? o : map[num] = f(num);
            int cmp2(long x, long y) => y < end ? cmp(g(x), g(y)) : 1;

            long l = begin;
            for (int d = fib.Count - 1; fib[d] > 2; d--)
            {
                long m1 = l + fib[d - 2];
                long m2 = l + fib[d - 1];
                if (cmp2(m1, m2) < 0)
                {
                    l = m1;
                }
            }

            long a = l;
            long b = l + 1;
            return cmp2(a, b) >= 0 ? (g(a), a) : (g(b), b);
        }

        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="begin">区間始め</param>
        /// <param name="end">区間終わり</param>
        /// <param name="f"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, long idx) TernarySearchL<T>(long begin, long end, Func<long, T> f)
        {
            return TernarySearchL(begin, end, f, Comparer<T>.Default.Compare);
        }

        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="begin">区間始め</param>
        /// <param name="end">区間終わり</param>
        /// <param name="f"></param>
        /// <param name="cmp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, int idx) TernarySearch<T>(int begin, int end, Func<int, T> f, Comparison<T> cmp)
        {
            var (value, idx) = TernarySearchL(begin, end, (x) => f((int)x), cmp);
            return (value, (int)idx);
        }

        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="begin">区間始め</param>
        /// <param name="end">区間終わり</param>
        /// <param name="f"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, int idx) TernarySearch<T>(int begin, int end, Func<int, T> f)
        {
            return TernarySearch(begin, end, f, Comparer<T>.Default.Compare);
        }

        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <param name="cmp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, double x) TernarySearchD<T>(double l, double r, Func<double, T> f, Comparison<T> cmp)
        {
            double revPhi = 2 / (1 + Math.Sqrt(5));

            double m1 = r - (r - l) * revPhi;
            double m2 = l + (r - l) * revPhi;

            T f1 = f(m1);
            T f2 = f(m2);

            for (int i = 0; i < 100; i++)
            {
                if (cmp(f1, f2) >= 0)
                {
                    r = m2;

                    m2 = m1;
                    f2 = f1;

                    m1 = r - (r - l) * revPhi;
                    f1 = f(m1);
                }
                else
                {
                    l = m1;

                    m1 = m2;
                    f1 = f2;

                    m2 = l + (r - l) * revPhi;
                    f2 = f(m2);
                }
            }

            return (f1, m1);
        }

        /// <summary>
        /// 三分探索 f(x)が最大になる (f(x), x)を返す
        /// </summary>
        /// <param name="value"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (T value, double x) TernarySearchD<T>(double l, double r, Func<double, T> f)
        {
            return TernarySearchD(l, r, f, Comparer<T>.Default.Compare);
        }
    }
}
