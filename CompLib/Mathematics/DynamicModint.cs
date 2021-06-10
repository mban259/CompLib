namespace CompLib.Mathematics
{
    struct ModInt<T> where T : IMod
    {
        public long num;
        public ModInt(long n) { num = n; }
        public override string ToString() { return num.ToString(); }
        public static ModInt<T> operator +(ModInt<T> l, ModInt<T> r) { l.num += r.num; int mod = default(T).Mod; if (l.num >= mod) l.num -= mod; return l; }
        public static ModInt<T> operator -(ModInt<T> l, ModInt<T> r) { l.num -= r.num; if (l.num < 0) { int mod = default(T).Mod; l.num += mod; } return l; }
        public static ModInt<T> operator *(ModInt<T> l, ModInt<T> r) { int mod = default(T).Mod; return new ModInt<T>(l.num * r.num % mod); }
        public static implicit operator ModInt<T>(long n) { int mod = default(T).Mod; n %= mod; if (n < 0) n += mod; return new ModInt<T>(n); }
        public static ModInt<T> Pow(long v, long k)
        {
            int mod = default(T).Mod;
            long ret = 1;
            for (k %= mod - 1; k > 0; k >>= 1, v = v * v % mod)
                if ((k & 1) == 1) ret = ret * v % mod;
            return new ModInt<T>(ret);
        }
        public static ModInt<T> Pow(ModInt<T> v, long k) { return Pow(v.num, k); }
        public static ModInt<T> Inverse(ModInt<T> v) { return Pow(v, default(T).Mod - 2); }
    }

    interface IMod
    {
        int Mod { get; }
    }

    public struct Mod998244353 : IMod
    {
        public int Mod { get { return 998244353; } }
    }

    public struct Mod1000000007 : IMod
    {
        public int Mod { get { return 1000000007; } }
    }
}