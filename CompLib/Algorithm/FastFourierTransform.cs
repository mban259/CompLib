

namespace CompLib.Algorithm
{
    using System;
    using CompLib.Mathematics;

    // https://bitbucket.org/camypaper/complib
    public static class FastFourierTransform
    {
        private const int NaiveThreshold = 60;
        private const int MaxTransformLength = 1 << 23;
        private const long PrimitiveRoot = 3;

        /// <summary>
        /// Computes the coefficients of the product of two polynomials modulo 998244353.
        /// </summary>
        /// <param name="a">The coefficients of the first polynomial, in ascending order.</param>
        /// <param name="b">The coefficients of the second polynomial, in ascending order.</param>
        /// <returns>The <c>a.Length + b.Length - 1</c> coefficients of the product.</returns>
        public static ModInt[] Convolution(ModInt[] a, ModInt[] b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (a.Length == 0 || b.Length == 0) return Array.Empty<ModInt>();

            long resultLength = (long)a.Length + b.Length - 1;
            if (resultLength > MaxTransformLength)
                throw new ArgumentException("The convolution exceeds the maximum supported transform length.");

            int outputLength = (int)resultLength;
            if (Math.Min(a.Length, b.Length) <= NaiveThreshold)
                return ConvolutionNaive(a, b, outputLength);

            int transformLength = 1;
            while (transformLength < outputLength)
                transformLength <<= 1;

            var fa = new ModInt[transformLength];
            var fb = new ModInt[transformLength];
            Array.Copy(a, fa, a.Length);
            Array.Copy(b, fb, b.Length);

            Transform(fa, inverse: false);
            Transform(fb, inverse: false);
            for (int i = 0; i < transformLength; i++)
                fa[i] *= fb[i];
            Transform(fa, inverse: true);

            
            ModInt inverseLength = ModInt.Inverse(transformLength);
            for (int i = 0; i < transformLength; i++)
                fa[i] *= inverseLength;
            

            var result = new ModInt[outputLength];
            Array.Copy(fa, result, outputLength);
            return result;
        }

        private static ModInt[] ConvolutionNaive(ModInt[] a, ModInt[] b, int resultLength)
        {
            var result = new ModInt[resultLength];
            if (a.Length < b.Length)
            {
                for (int j = 0; j < b.Length; j++)
                    for (int i = 0; i < a.Length; i++)
                        result[i + j] += a[i] * b[j];
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                    for (int j = 0; j < b.Length; j++)
                        result[i + j] += a[i] * b[j];
            }

            return result;
        }

        public static void Transform(ModInt[] values, bool inverse)
        {
            int n = values.Length;
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                for (; (j & bit) != 0; bit >>= 1)
                    j ^= bit;
                j ^= bit;
                if (i < j)
                    (values[i], values[j]) = (values[j], values[i]);
            }

            for (int length = 2; length <= n; length <<= 1)
            {
                ModInt root = ModInt.Pow(PrimitiveRoot, (ModInt.Mod - 1) / length);
                if (inverse) root = ModInt.Inverse(root);
                int halfLength = length >> 1;

                for (int start = 0; start < n; start += length)
                {
                    ModInt factor = 1;
                    for (int i = 0; i < halfLength; i++)
                    {
                        ModInt even = values[start + i];
                        ModInt odd = values[start + i + halfLength] * factor;
                        values[start + i] = even + odd;
                        values[start + i + halfLength] = even - odd;
                        factor *= root;
                    }
                }
            }

        
        }
    }   
}

 namespace CompLib.Mathematics
    {
        #region ModInt

        /// <summary>
        /// [0,<see cref="Mod"/>) までの値を取るような数
        /// </summary>
        public struct ModInt
        {
            /// <summary>
            /// 剰余を取る値．
            /// </summary>
            // public const long Mod = (int)1e9 + 7;
            public const long Mod = 998244353;

            /// <summary>
            /// 実際の数値．
            /// </summary>
            public long num;

            /// <summary>
            /// 値が <paramref name="n"/> であるようなインスタンスを構築します．
            /// </summary>
            /// <param name="n">インスタンスが持つ値</param>
            /// <remarks>パフォーマンスの問題上，コンストラクタ内では剰余を取りません．そのため，<paramref name="n"/> ∈ [0,<see cref="Mod"/>) を満たすような <paramref name="n"/> を渡してください．このコンストラクタは O(1) で実行されます．</remarks>
            public ModInt(long n)
            {
                num = n;
            }

            /// <summary>
            /// このインスタンスの数値を文字列に変換します．
            /// </summary>
            /// <returns>[0,<see cref="Mod"/>) の範囲内の整数を 10 進表記したもの．</returns>
            public override string ToString()
            {
                return num.ToString();
            }

            public static ModInt operator +(ModInt l, ModInt r)
            {
                l.num += r.num;
                if (l.num >= Mod) l.num -= Mod;
                return l;
            }

            public static ModInt operator -(ModInt l, ModInt r)
            {
                l.num -= r.num;
                if (l.num < 0) l.num += Mod;
                return l;
            }

            public static ModInt operator *(ModInt l, ModInt r)
            {
                return new ModInt(l.num * r.num % Mod);
            }

            public static implicit operator ModInt(long n)
            {
                n %= Mod;
                if (n < 0) n += Mod;
                return new ModInt(n);
            }

            /// <summary>
            /// 与えられた 2 つの数値からべき剰余を計算します．
            /// </summary>
            /// <param name="v">べき乗の底</param>
            /// <param name="k">べき指数</param>
            /// <returns>繰り返し二乗法により O(N log N) で実行されます．</returns>
            public static ModInt Pow(ModInt v, long k)
            {
                return Pow(v.num, k);
            }

            /// <summary>
            /// 与えられた 2 つの数値からべき剰余を計算します．
            /// </summary>
            /// <param name="v">べき乗の底</param>
            /// <param name="k">べき指数</param>
            /// <returns>繰り返し二乗法により O(N log N) で実行されます．</returns>
            public static ModInt Pow(long v, long k)
            {
                long ret = 1;
                if (Mod == 998244353 || Mod == (long)1e9 + 7) k %= Mod - 1;
                for (; k > 0; k >>= 1, v = v * v % Mod)
                    if ((k & 1) == 1)
                        ret = ret * v % Mod;
                return new ModInt(ret);
            }

            /// <summary>
            /// 与えられた数の逆元を計算します．
            /// </summary>
            /// <param name="v">逆元を取る対象となる数</param>
            /// <returns>逆元となるような値</returns>
            /// <remarks>法が素数であることを仮定して，フェルマーの小定理に従って逆元を O(log N) で計算します．</remarks>
            public static ModInt Inverse(ModInt v)
            {
                return Pow(v, Mod - 2);
            }
        }

        #endregion

        #region Binomial Coefficient

        public class BinomialCoefficient
        {
            public ModInt[] fact, ifact;

            public BinomialCoefficient(int n)
            {
                fact = new ModInt[n + 1];
                ifact = new ModInt[n + 1];
                fact[0] = 1;
                for (int i = 1; i <= n; i++)
                    fact[i] = fact[i - 1] * i;
                ifact[n] = ModInt.Inverse(fact[n]);
                for (int i = n - 1; i >= 0; i--)
                    ifact[i] = ifact[i + 1] * (i + 1);
                ifact[0] = ifact[1];
            }

            public ModInt this[int n, int r]
            {
                get
                {
                    if (n >= 0)
                    {
                        if (!(0 <= r && r <= n)) return 0;
                        return fact[n] * ifact[n - r] * ifact[r];
                    }
                    else
                    {
                        if (!(0 <= r)) throw new ArgumentOutOfRangeException();
                        return (r % 2 == 0 ? 1 : -1) * this[-n + r - 1, r];
                    }
                }
            }

            public ModInt RepeatedCombination(int n, int k)
            {
                if (k == 0) return 1;
                return this[n + k - 1, k];
            }
        }

        #endregion
    }