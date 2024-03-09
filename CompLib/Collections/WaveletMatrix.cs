

namespace CompLib.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Num = System.Int64;
    public class WaveletMatrix
    {

        private readonly int H;
        public readonly int Count;

        BitVector[] _bv;
        int[] _count0;

        Dictionary<Num, int> _leftMap;


        public WaveletMatrix(Num[] nums, int h = 31)
        {
            H = h;
            Count = nums.Length;
            _bv = new BitVector[H];
            _count0 = new int[H];
            var curNums = new Num[Count];
            Array.Copy(nums, curNums, Count);
            var nextNums = new Num[Count];
            var ones = new List<Num>();
            for (int b = H - 1; b >= 0; b--)
            {
                var bits = new bool[Count];
                for (int i = 0; i < Count; i++)
                {
                    if ((curNums[i] & ((Num)1 << b)) == 0)
                    {
                        nextNums[_count0[b]++] = curNums[i];
                    }
                    else
                    {
                        ones.Add(curNums[i]);
                        bits[i] = true;
                    }
                }
                _bv[b] = new BitVector(bits);
                for (int i = 0; i < ones.Count; i++)
                {
                    nextNums[_count0[b] + i] = ones[i];
                }
                (curNums, nextNums) = (nextNums, curNums);
                ones.Clear();
            }

            _leftMap = new Dictionary<Num, int>();
            for (int i = 0; i < Count; i++)
            {
                if (i == 0 || curNums[i - 1] != curNums[i])
                {
                    _leftMap[curNums[i]] = i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Num Get(int i)
        {
            Num ans = 0;
            for (int b = H - 1; b >= 0; b--)
            {
                if (_bv[b][i])
                {
                    ans |= (Num)1 << b;
                    i = _count0[b] + _bv[b].Rank1(i);
                }
                else
                {
                    i = _bv[b].Rank0(i);
                }
            }
            return ans;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank(int end, Num n)
        {
            int left;
            if (!_leftMap.TryGetValue(n, out left)) return 0;
            for (int b = H - 1; b >= 0; b--)
            {
                if (((n >> b) & 1) != 0)
                {
                    end = _count0[b] + _bv[b].Rank1(end);
                }
                else
                {
                    end = _bv[b].Rank0(end);
                }
            }

            return end - left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank(int begin, int end, Num n)
        {
            return Rank(end, n) - Rank(begin, n);
        }

        public Num this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Get(i); }
        }
    }

    public class BitVector
    {
        public readonly int Count;


        const int ULongRightShift = 6;
        const int ULongSize = 1 << ULongRightShift;
        const int ULongAnd = ULongSize - 1;
        private readonly ulong[] _ul;



        // 1chunk 64個ずつのBlockに分ける 累積和
        const int BlockSize = 64;
        const int ChunkBlock = 4;
        // 256個ずつのChunkに分ける 累積和
        const int ChunkSize = ChunkBlock * BlockSize;
        private readonly int[] _chunks;
        private readonly byte[] _blocks;

        public BitVector(bool[] bits)
        {
            Count = bits.Length;
            _ul = new ulong[(Count >> ULongRightShift) + 1];
            for (int i = 0; i < Count; i++)
            {
                if (bits[i])
                {
                    _ul[i >> ULongRightShift] |= (1UL << (i & ULongAnd));
                }
            }

            _chunks = new int[Count / ChunkSize + 2];
            _blocks = new byte[Count / BlockSize + 2];
            for (int i = 0; i < Count; i++)
            {
                if (bits[i])
                {
                    int c = i / ChunkSize;
                    int b = i / BlockSize;
                    _chunks[c + 1]++;
                    if ((b + 1) % ChunkBlock != 0) _blocks[b + 1]++;
                }
            }
            for (int i = 0; i < (Count + 1) / ChunkSize; i++)
            {
                _chunks[i + 1] += _chunks[i];
            }
            for (int i = 0; i < (Count + 1) / BlockSize; i++)
            {
                if ((i + 1) % ChunkBlock != 0) _blocks[i + 1] += _blocks[i];
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(int i)
        {
            Debug.Assert(0 <= i && i < Count);
            return (_ul[i >> ULongRightShift] & (1UL << (i & ULongAnd))) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank0(int end)
        {
            Debug.Assert(0 <= end && end <= Count);
            return end - Rank1(end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank0(int begin, int end)
        {
            return end - begin - (Rank1(begin, end));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank1(int end)
        {
            Debug.Assert(0 <= end && end <= Count);
            int chunkIdx = end / ChunkSize;
            int blockIdx = end / BlockSize;
            ulong ulAnd = (1UL << (end % BlockSize)) - 1;
            return _chunks[chunkIdx] + _blocks[blockIdx] + PopCount(_ul[blockIdx] & ulAnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Rank1(int begin, int end)
        {
            Debug.Assert(0 <= begin && begin <= end && end <= Count);
            return Rank1(end) - Rank1(begin);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Select1(int k)
        {
            int okC = 0;
            int ngC = Count / ChunkSize + 1;
            while (ngC - okC > 1)
            {
                int mid = (okC + ngC) / 2;
                if (_chunks[mid] < k) okC = mid;
                else ngC = mid;
            }

            int ok = okC * ChunkSize;
            int ng = Math.Min(Count, ok + ChunkSize);
            while (ng - ok > 1)
            {
                int mid = (ok + ng) / 2;
                if (Rank1(mid) < k) ok = mid;
                else ng = mid;
            }
            return ok;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Select0(int k)
        {
            int okC = 0;
            int ngC = Count / ChunkSize + 1;
            while (ngC - okC > 1)
            {
                int mid = (okC + ngC) / 2;
                if (mid * ChunkSize - _chunks[mid] < k) okC = mid;
                else ngC = mid;
            }

            int ok = okC * ChunkSize;
            int ng = Math.Min(Count, ok + ChunkSize);
            while (ng - ok > 1)
            {
                int mid = (ok + ng) / 2;
                if (Rank0(mid) < k) ok = mid;
                else ng = mid;
            }
            return ok;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PopCount(ulong n)
        {
            n = (n & 0x5555555555555555) + (n >> 1 & 0x5555555555555555);
            n = (n & 0x3333333333333333) + (n >> 2 & 0x3333333333333333);
            n = (n & 0x0f0f0f0f0f0f0f0f) + (n >> 4 & 0x0f0f0f0f0f0f0f0f);
            n = (n & 0x00ff00ff00ff00ff) + (n >> 8 & 0x00ff00ff00ff00ff);
            n = (n & 0x0000ffff0000ffff) + (n >> 16 & 0x0000ffff0000ffff);
            return (int)((n & 0x00000000ffffffff) + (n >> 32 & 0x00000000ffffffff));
        }

        public bool this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Get(i); }
        }
    }
}
