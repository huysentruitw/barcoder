using System;

namespace Barcoder.Utils
{
    internal struct GaloisField
    {
        public int Size;
        public int Base;
        public int[] ALogTable;
        public int[] LogTable;

        public GaloisField(int pp, int fieldSize, int @base)
        {
            Size = fieldSize;
            Base = @base;
            ALogTable = new int[fieldSize];
            LogTable = new int[fieldSize];

            for (int i = 0, x = 1; i < fieldSize; i++)
            {
                ALogTable[i] = x;
                x *= 2;
                if (x >= fieldSize)
                    x = (x ^ pp) & (fieldSize - 1);
            }

            for (int i = 0; i < fieldSize; i++)
                LogTable[ALogTable[i]] = i;
        }

        public int AddOrSubtract(int a, int b)
            => a ^ b;

        public int Multiply(int a, int b)
        {
            if (a == 0 || b == 0) return 0;
            return ALogTable[(LogTable[a] + LogTable[b]) % (Size - 1)];
        }

        public int Divide(int a, int b)
        {
            if (b == 0) throw new DivideByZeroException();
            if (a == 0) return 0;
            return ALogTable[(LogTable[a] - LogTable[b]) % (Size - 1)];
        }

        public int Inverse(int number)
            => ALogTable[Size - 1 - LogTable[number]];
    }
}
