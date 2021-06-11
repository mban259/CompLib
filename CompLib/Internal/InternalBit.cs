using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace CompLib.Internal
{
    static class InternalBit
    {
        private static readonly int[] RevBSF;

        static InternalBit()
        {
            RevBSF = new int[1 << 24];
            for (int i = 0; i < (1 << 24); i++) RevBSF[i] = BSF(~i);
        }
        public static int CeilPow2(int n)
        {
            int x = 0;
            while ((1 << x) < n) x++;
            return x;
        }

        public static int BSF(int n)
        {
            switch (n)
            {
                case 998244352:
                    return 23;
                case 754974720:
                    return 24;
                case 167772160:
                    return 25;
                case 469762048:
                    return 26;
            }
            Debug.Assert(n != 0);
            int x = 0;
            while (((1 << x) & n) == 0) x++;
            return x;
        }

        public static int GetRevBSF(int n)
        {
            return RevBSF[n];
        }
    }
}