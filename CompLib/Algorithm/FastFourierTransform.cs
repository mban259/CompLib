using System;
using CompLib.Internal;
using CompLib.Mathematics;

namespace CompLib.Algorithm
{
    static class FastFourierTransform
    {
        static FastFourierTransform()
        {
            
        }
    }

    struct ModInt3
    {
        const long Mod1 = 754974721;
        const long Mod2 = 167772161;
        const long Mod3 = 469762049;
        private long num1, num2, num3;

        public ModInt3(long n1, long n2, long n3)
        {
            num1 = n1;
            num2 = n2;
            num3 = n3;
        }
        public static ModInt3 operator +(ModInt3 l, ModInt3 r)
        {
            l.num1 += r.num1;
            if (l.num1 >= Mod1) l.num1 -= Mod1;
            l.num2 += r.num2;
            if (l.num2 >= Mod2) l.num2 -= Mod2;
            l.num3 += r.num3;
            if (l.num3 >= Mod3) l.num3 -= Mod3;
            return l;
        }

        public static ModInt3 operator -(ModInt3 l, ModInt3 r)
        {
            l.num1 -= r.num1;
            if (l.num1 < 0) l.num1 += Mod1;
            l.num2 -= r.num2;
            if (l.num2 < 0) l.num2 += Mod2;
            l.num3 -= r.num3;
            if (l.num3 < 0) l.num3 += Mod3;
            return l;
        }

        public static ModInt3 operator *(ModInt3 l, ModInt3 r)
        {
            return new ModInt3(l.num1 * r.num1 % Mod1, l.num2 * r.num2 % Mod2, l.num3 * r.num3 % Mod3);
        }

        public static implicit operator ModInt3(long n)
        {
            if (n < 0)
            {
                return new ModInt3(n % Mod1 + Mod1, n % Mod2 + Mod2, n % Mod3 + Mod3);
            }
            else
            {
                return new ModInt3(n % Mod1, n % Mod2, n % Mod3);
            }
        }

        public static ModInt3 Pow(long v, long k)
        {
            long ret1 = 1;
            long ret2 = 1;
            long ret3 = 1;
            long v1 = v % Mod1;
            long v2 = v % Mod2;
            long v3 = v % Mod3;
            for (; k > 0; k >>= 1, v1 = v1 * v1 % Mod1, v2 = v2 * v2 % Mod2, v3 = v3 * v3 % Mod3)
            {
                if ((k & 1) == 1)
                {
                    ret1 = ret1 * v1 % Mod1;
                    ret2 = ret2 * v2 % Mod2;
                    ret3 = ret3 * v3 % Mod3;
                }
            }
            return new ModInt3(ret1, ret2, ret3);
        }

        public static ModInt3 Inverse(ModInt3 v)
        {
            long ret1 = 1;
            long v1 = v.num1;
            long k1 = Mod1 - 2;
            for (; k1 > 0; k1 >>= 1, v1 = v1 * v1 % Mod1)
            {
                if ((k1 & 1) == 1) ret1 = ret1 * v1 % Mod1;
            }
            long ret2 = 1;
            long v2 = v.num2;
            long k2 = Mod2 - 2;
            for (; k2 > 0; k2 >>= 1, v2 = v2 * v2 % Mod2)
            {
                if ((k2 & 1) == 1) ret2 = ret2 * v2 % Mod2;
            }
            long ret3 = 1;
            long v3 = v.num3;
            long k3 = Mod3 - 2;
            for (; k3 > 0; k3 >>= 1, v3 = v3 * v3 % Mod3)
            {
                if ((k3 & 1) == 1) ret3 = ret3 * v3 % Mod3;
            }

            return new ModInt3(ret1, ret2, ret3);
        }
        const ulong M1M2 = Mod1 * Mod2;
        const ulong M1M3 = Mod1 * Mod3;
        const ulong M2M3 = Mod2 * Mod3;
        const ulong M1M2M3 = unchecked((ulong)Mod1 * Mod2 * Mod3);
        // internal::inv_gcd(MOD2 * MOD3, MOD1).second;
        const ulong I1 = 190329765;
        // Mod2 * Mod3, Mod2
        const ulong I2 = 58587104;
        // Mod1* Mod2, Mod3
        const ulong I3 = 187290749;

        public long ToLong()
        {
            ulong x = 0;
            unchecked
            {
                x += (ulong)num1 * I1 % Mod1 * M2M3;
                x += (ulong)num2 * I2 % Mod2 * M1M3;
                x += (ulong)num3 * I3 % Mod3 * M1M2;

                long m = (long)x % Mod1;
                if (m < 0) m += Mod1;
                long diff = num1 - m;
                if (diff < 0) diff += Mod1;

                switch (diff % 5)
                {
                    case 2:
                        x -= M1M2M3;
                        break;
                    case 3:
                        x -= 2 * M1M2M3;
                        break;
                    case 4:
                        x -= 3 * M1M2M3;
                        break;
                }
            }
            return (long)x;

        }
    }
}