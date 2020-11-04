namespace Barcoder.DataMatrix
{
    internal sealed class CodeSize
    {
        public CodeSize(int rows, int columns, int regionCountHorizontal, int regionCountVertical, int eccCount, int blockCount)
        {
            Rows = rows;
            Columns = columns;
            RegionCountHorizontal = regionCountHorizontal;
            RegionCountVertical = regionCountVertical;
            EccCount = eccCount;
            BlockCount = blockCount;
        }

        public int Rows { get; }

        public int Columns { get; }

        public int RegionCountHorizontal { get; }

        public int RegionCountVertical { get; }

        public int EccCount { get; }

        public int BlockCount { get; }

        public int RegionRows => (Rows - (RegionCountVertical * 2)) / RegionCountVertical;

        public int RegionColumns => (Columns - (RegionCountHorizontal * 2)) / RegionCountHorizontal;

        public int MatrixRows => RegionRows * RegionCountVertical;

        public int MatrixColumns => RegionColumns * RegionCountHorizontal;

        public int DataCodewords => ((MatrixColumns * MatrixRows) / 8) - EccCount;

        public int DataCodewordsForBlock(int index)
        {
            if (Rows == 144 && Columns == 144)
            {
                // Special case...
                if (index < 8)
                    return 156;

                return 155;
            }

            return DataCodewords / BlockCount;
        }

        public int ErrorCorrectionCodewordsPerBlock => EccCount / BlockCount;

        public override string ToString()
        {
            return $"{Rows}x{Columns}";
        }
    }

    internal static class CodeSizes
    {
        public static readonly CodeSize[] All = new[]
        {
            /**
             * Square CodeSize
             */
            new CodeSize(10, 10, 1, 1, 5, 1),
            new CodeSize(12, 12, 1, 1, 7, 1),
            new CodeSize(14, 14, 1, 1, 10, 1),
            new CodeSize(16, 16, 1, 1, 12, 1),
            new CodeSize(18, 18, 1, 1, 14, 1),
            new CodeSize(20, 20, 1, 1, 18, 1),
            new CodeSize(22, 22, 1, 1, 20, 1),
            new CodeSize(24, 24, 1, 1, 24, 1),
            new CodeSize(26, 26, 1, 1, 28, 1),
            new CodeSize(32, 32, 2, 2, 36, 1),
            new CodeSize(36, 36, 2, 2, 42, 1),
            new CodeSize(40, 40, 2, 2, 48, 1),
            new CodeSize(44, 44, 2, 2, 56, 1),
            new CodeSize(48, 48, 2, 2, 68, 1),
            new CodeSize(52, 52, 2, 2, 84, 2),
            new CodeSize(64, 64, 4, 4, 112, 2),
            new CodeSize(72, 72, 4, 4, 144, 4),
            new CodeSize(80, 80, 4, 4, 192, 4),
            new CodeSize(88, 88, 4, 4, 224, 4),
            new CodeSize(96, 96, 4, 4, 272, 4),
            new CodeSize(104, 104, 4, 4, 336, 6),
            new CodeSize(120, 120, 6, 6, 408, 6),
            new CodeSize(132, 132, 6, 6, 496, 8),
            new CodeSize(144, 144, 6, 6, 620, 10),

            /*
             * Rectangular CodeSize
             */
            new CodeSize(8, 18, 1, 1, 7, 1),
            new CodeSize(8, 32, 2, 1, 11, 1),
            new CodeSize(12, 26, 1, 1, 14, 1),
            new CodeSize(12, 36, 2, 1, 18, 1),
            new CodeSize(16, 36, 2, 1, 24, 1),
            new CodeSize(16, 48, 2, 1, 28, 1),
        };
    }
}
