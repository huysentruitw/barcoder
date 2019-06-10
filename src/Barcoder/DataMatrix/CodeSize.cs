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

        public int RegionRows => (Rows - (RegionCountHorizontal * 2)) / RegionCountHorizontal;

        public int RegionColumns => (Columns - (RegionCountVertical * 2)) / RegionCountVertical;

        public int MatrixRows => RegionRows * RegionCountHorizontal;

        public int MatrixColumns => RegionColumns * RegionCountVertical;

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
    }
}
