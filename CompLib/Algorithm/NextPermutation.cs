// ねくすと☆ぱ〜みゅて〜しょん!
namespace CompLib.Algorithm
{
    using System;
    using System.Collections.Generic;
    public static class Algorithm
    {
        private static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private static void Reverse<T>(T[] array, int begin)
        {
            // [begin, array.Length)を反転
            if (array.Length - begin >= 2)
            {
                for (int i = begin, j = array.Length - 1; i < j; i++, j--)
                {
                    Swap(ref array[i], ref array[j]);
                }
            }
        }

        /// <summary>
        /// arrayを辞書順で次の順列にする 存在しないときはfalseを返す
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool NextPermutation<T>(T[] array, Comparison<T> comparison)
        {
            for (int i = array.Length - 2; i >= 0; i--)
            {
                if (comparison(array[i], array[i + 1]) < 0)
                {
                    int j = array.Length - 1;
                    for (; j > i; j--)
                    {
                        if (comparison(array[i], array[j]) < 0)
                        {
                            break;
                        }
                    }

                    Swap(ref array[i], ref array[j]);
                    Reverse(array, i + 1);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// arrayを辞書順で次の順列にする 存在しないときはfalseを返す
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool NextPermutation<T>(T[] array, Comparer<T> comparer) =>
            NextPermutation(array, comparer.Compare);

        /// <summary>
        /// arrayを辞書順で次の順列にする 存在しないときはfalseを返す
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool NextPermutation<T>(T[] array) => NextPermutation(array, Comparer<T>.Default);
    }
}
