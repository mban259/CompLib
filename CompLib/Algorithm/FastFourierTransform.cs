using System;
using CompLib.Internal;
using CompLib.Mathematics;

namespace CompLib.Algorithm
{
    static class FastFourierTransform<T> where T : IMod
    {
        private readonly static ModInt<T>[] sumE, sumIE;
        static FastFourierTransform()
        {
            int mod = default(T).Mod;
            int cnt2 = InternalBit.BSF(mod - 1);
            var es = new ModInt<T>[cnt2 - 1];
            var ies = new ModInt<T>[cnt2 - 1];
            var e = ModInt<T>.Pow(InternalMath.PrimitiveRoot(mod), (mod - 1) >> cnt2);
            var ie = ModInt<T>.Inverse(e);
            for (int i = cnt2; i >= 2; i--)
            {
                es[i - 2] = e;
                ies[i - 2] = ie;
                e *= e;
                ie *= ie;
            }
            sumIE = new ModInt<T>[cnt2 - 1];
            sumE = new ModInt<T>[cnt2 - 1];
            ModInt<T> iNow = 1;
            ModInt<T> now = 1;
            for (int i = 0; i <= cnt2 - 2; i++)
            {
                sumIE[i] = ies[i] * iNow;
                sumE[i] = es[i] * now;
                now *= ies[i];
                iNow *= es[i];
            }
        }

        public static void Butterfly(ModInt<T>[] a, int h)
        {
            for (int ph = 1; ph <= h; ph++)
            {
                int w = 1 << (ph - 1);
                int p = 1 << (h - ph);

                ModInt<T> now = 1;
                for (int s = 0; s < w; s++)
                {
                    int offset = s << (h - ph + 1);
                    for (int i = 0; i < p; i++)
                    {
                        var l = a[i + offset];
                        var r = a[i + offset + p] * now;
                        a[i + offset] = l + r;
                        a[i + offset + p] = l - r;
                    }
                    now *= sumE[InternalBit.GetRevBSF(s)];
                }
            }
        }

        public static void ButterflyInv(ModInt<T>[] a, int h)
        {
            for (int ph = h; ph >= 1; ph--)
            {
                int w = 1 << (ph - 1);
                int p = 1 << (h - ph);
                ModInt<T> iNow = 1;
                for (int s = 0; s < w; s++)
                {
                    int offset = s << (h - ph + 1);
                    for (int i = 0; i < p; i++)
                    {
                        var l = a[i + offset];
                        var r = a[i + offset + p];
                        a[i + offset] = l + r;
                        a[i + offset + p] = (default(T).Mod + l.num - r.num) * iNow;
                    }
                    iNow *= sumIE[InternalBit.GetRevBSF(s)];
                }
            }
        }

        public static ModInt<T>[] Convolution(ModInt<T>[] a, ModInt<T>[] b)
        {
            int n = a.Length;
            int m = b.Length;
            if (n == 0 || m == 0) return Array.Empty<ModInt<T>>();

            int h = InternalBit.CeilPow2(n + m - 1);
            int z = 1 << h;
            ModInt<T>[] a2 = new ModInt<T>[z];
            Array.Copy(a, a2, n);
            Butterfly(a2, h);
            ModInt<T>[] b2 = new ModInt<T>[z];
            Array.Copy(b, b2, m);
            Butterfly(b2, h);

            for (int i = 0; i < z; i++)
            {
                a2[i] *= b2[i];
            }
            ButterflyInv(a2, h);
            Array.Resize(ref a2, n + m - 1);
            ModInt<T> iz = ModInt<T>.Inverse(z);
            for (int i = 0; i < n + m - 1; i++) a2[i] *= iz;
            return a2;
        }

        public static ModInt<T>[] Convolution(long[] a, long[] b)
        {
            int n = a.Length;
            int m = b.Length;
            if (n == 0 || m == 0) return Array.Empty<ModInt<T>>();
            int h = InternalBit.CeilPow2(n + m - 1);
            int z = 1 << h;

            ModInt<T>[] a2 = new ModInt<T>[z];
            for (int i = 0; i < n; i++) a2[i] = a[i];
            Butterfly(a2, h);
            ModInt<T>[] b2 = new ModInt<T>[z];
            for (int i = 0; i < m; i++) b2[i] = b[i];
            Butterfly(b2, h);

            for (int i = 0; i < z; i++)
            {
                a2[i] *= b2[i];
            }
            ButterflyInv(a2, h);
            Array.Resize(ref a2, n + m - 1);
            ModInt<T> iz = ModInt<T>.Inverse(z);
            for (int i = 0; i < n + m - 1; i++) a2[i] *= iz;
            return a2;
        }
    }

    public static class FastFourierTransform
    {
        const ulong Mod1 = 754974721;
        const ulong Mod2 = 167772161;
        const ulong Mod3 = 469762049;
        const ulong M2M3 = Mod2 * Mod3;
        const ulong M1M3 = Mod1 * Mod3;
        const ulong M1M2 = Mod1 * Mod2;
        const ulong M1M2M3 = unchecked(Mod1 * Mod2 * Mod3);
        static readonly ulong[] Offset = { 0, 0, M1M2M3, unchecked(2 * M1M2M3), unchecked(3 * M1M2M3) };

        // internal::inv_gcd(MOD2 * MOD3, MOD1).second;
        const ulong I1 = 190329765;
        // Mod2 * Mod3, Mod2
        const ulong I2 = 58587104;
        // Mod1* Mod2, Mod3
        const ulong I3 = 187290749;

        public static long[] ConvolutionLL(long[] a, long[] b)
        {
            int n = a.Length;
            int m = b.Length;
            if (n == 0 || m == 0) return Array.Empty<long>();
            unchecked
            {
                var c1 = FastFourierTransform<M1>.Convolution(a, b);
                var c2 = FastFourierTransform<M2>.Convolution(a, b);
                var c3 = FastFourierTransform<M3>.Convolution(a, b);

                long[] c = new long[n + m - 1];
                for (int i = 0; i < n + m - 1; i++)
                {
                    ulong x = 0;
                    x += ((ulong)c1[i].num * I1) % Mod1 * M2M3;
                    x += ((ulong)c2[i].num * I2) % Mod2 * M1M3;
                    x += ((ulong)c3[i].num * I3) % Mod3 * M1M2;

                    long diff = c1[i].num - InternalMath.SafeMod((long)x, (long)Mod1);
                    if (diff < 0) diff += (long)Mod1;
                    x -= Offset[diff % 5];
                    c[i] = (long)x;
                }
                return c;
            }
        }

        struct M1 : IMod
        {
            public int Mod => (int)Mod1;
        }

        struct M2 : IMod
        {
            public int Mod => (int)Mod2;
        }

        struct M3 : IMod
        {
            public int Mod => (int)Mod3;
        }
    }


}