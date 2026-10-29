namespace CompLib.Algorithm
{
    using System;
    /// <summary>
    /// Computes convolutions of signed 64-bit integers using three number-theoretic transforms.
    /// </summary>
    public static class FastFourierTransformLL
    {
        private const int MaxResultLength = 1 << 24;
        private const int NaiveThreshold = 60;

        private const long Mod1 = 754974721;
        private const long Mod2 = 167772161;
        private const long Mod3 = 469762049;

        private const long PrimitiveRoot1 = 11;
        private const long PrimitiveRoot2 = 3;
        private const long PrimitiveRoot3 = 3;

        private static readonly long InverseM2M3Mod1 = InverseMod(Mod2 * Mod3, Mod1);
        private static readonly long InverseM1M3Mod2 = InverseMod(Mod1 * Mod3, Mod2);
        private static readonly long InverseM1M2Mod3 = InverseMod(Mod1 * Mod2, Mod3);

        private static readonly ulong M2M3 = (ulong)Mod2 * (ulong)Mod3;
        private static readonly ulong M1M3 = (ulong)Mod1 * (ulong)Mod3;
        private static readonly ulong M1M2 = (ulong)Mod1 * (ulong)Mod2;

        private static readonly ulong M1M2M3 = unchecked(M1M2 * (ulong)Mod3);
        private static readonly ulong[] ReconstructionOffsets =
        {
            0,
            0,
            M1M2M3,
            unchecked(2 * M1M2M3),
            unchecked(3 * M1M2M3)
        };

        /// <summary>
        /// Computes the integer convolution of two signed 64-bit sequences.
        /// </summary>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <returns>The convolution coefficients as signed 64-bit integers.</returns>
        /// <remarks>
        /// The result length must not exceed 2^24. As with the ACL implementation,
        /// coefficients are reconstructed from three modular convolutions and therefore
        /// must fit the reconstruction range for the returned values to be exact.
        /// </remarks>
        public static long[] Convolution(long[] a, long[] b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (a.Length == 0 || b.Length == 0) return Array.Empty<long>();

            long resultLengthLong = (long)a.Length + b.Length - 1;
            if (resultLengthLong > MaxResultLength)
                throw new ArgumentException("The convolution result length must not exceed 2^24.");

            int resultLength = (int)resultLengthLong;
            long[] c1 = ConvolutionMod(a, b, Mod1, PrimitiveRoot1);
            long[] c2 = ConvolutionMod(a, b, Mod2, PrimitiveRoot2);
            long[] c3 = ConvolutionMod(a, b, Mod3, PrimitiveRoot3);
            var result = new long[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                ulong x;
                unchecked
                {
                    x = (ulong)((c1[i] * InverseM2M3Mod1) % Mod1) * M2M3;
                    x += (ulong)((c2[i] * InverseM1M3Mod2) % Mod2) * M1M3;
                    x += (ulong)((c3[i] * InverseM1M2Mod3) % Mod3) * M1M2;
                }

                long xModuloMod1 = unchecked((long)x) % Mod1;
                if (xModuloMod1 < 0) xModuloMod1 += Mod1;
                long difference = c1[i] - xModuloMod1;
                if (difference < 0) difference += Mod1;

                x = unchecked(x - ReconstructionOffsets[difference % 5]);
                result[i] = unchecked((long)x);
            }

            return result;
        }

        private static long[] ConvolutionMod(long[] a, long[] b, long mod, long primitiveRoot)
        {
            int resultLength = a.Length + b.Length - 1;
            if (Math.Min(a.Length, b.Length) <= NaiveThreshold)
            {
                var result = new long[resultLength];
                for (int i = 0; i < a.Length; i++)
                {
                    long ai = Normalize(a[i], mod);
                    for (int j = 0; j < b.Length; j++)
                    {
                        long product = ai * Normalize(b[j], mod) % mod;
                        result[i + j] = (result[i + j] + product) % mod;
                    }
                }

                return result;
            }

            int transformLength = 1;
            while (transformLength < resultLength)
                transformLength <<= 1;

            var fa = new long[transformLength];
            var fb = new long[transformLength];
            for (int i = 0; i < a.Length; i++) fa[i] = Normalize(a[i], mod);
            for (int i = 0; i < b.Length; i++) fb[i] = Normalize(b[i], mod);

            Transform(fa, mod, primitiveRoot, inverse: false);
            Transform(fb, mod, primitiveRoot, inverse: false);
            for (int i = 0; i < transformLength; i++)
                fa[i] = fa[i] * fb[i] % mod;
            Transform(fa, mod, primitiveRoot, inverse: true);

            var resultArray = new long[resultLength];
            Array.Copy(fa, resultArray, resultLength);
            return resultArray;
        }

        private static void Transform(long[] values, long mod, long primitiveRoot, bool inverse)
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
                long root = PowMod(primitiveRoot, (mod - 1) / length, mod);
                if (inverse) root = PowMod(root, mod - 2, mod);
                int halfLength = length >> 1;

                for (int start = 0; start < n; start += length)
                {
                    long factor = 1;
                    for (int i = 0; i < halfLength; i++)
                    {
                        long even = values[start + i];
                        long odd = values[start + i + halfLength] * factor % mod;
                        long sum = even + odd;
                        if (sum >= mod) sum -= mod;
                        long difference = even - odd;
                        if (difference < 0) difference += mod;

                        values[start + i] = sum;
                        values[start + i + halfLength] = difference;
                        factor = factor * root % mod;
                    }
                }
            }

            if (inverse)
            {
                long inverseLength = PowMod(n, mod - 2, mod);
                for (int i = 0; i < n; i++)
                    values[i] = values[i] * inverseLength % mod;
            }
        }

        private static long Normalize(long value, long mod)
        {
            long result = value % mod;
            return result < 0 ? result + mod : result;
        }

        private static long PowMod(long value, long exponent, long mod)
        {
            long result = 1;
            value %= mod;
            while (exponent > 0)
            {
                if ((exponent & 1) != 0)
                    result = result * value % mod;
                value = value * value % mod;
                exponent >>= 1;
            }

            return result;
        }

        private static long InverseMod(long value, long mod)
        {
            long oldR = mod, r = value % mod;
            long oldS = 0, s = 1;
            while (r != 0)
            {
                long quotient = oldR / r;
                (oldR, r) = (r, oldR - quotient * r);
                (oldS, s) = (s, oldS - quotient * s);
            }

            if (oldR != 1)
                throw new InvalidOperationException("The CRT moduli must be pairwise coprime.");
            return Normalize(oldS, mod);
        }
    }
}