using System;

namespace Barcoder.Pdf417
{
    internal static class Dimensions
    {
        public const int MinimumNumberOfColumns = 2;
        public const int MaximumNumberOfColumns = 30;
        public const int MinimumNumberOfRows = 2;
        public const int MaximumNumberOfRows = 30;
        public const int ModuleHeight = 2;
        private const double PreferredRatio = 3.0;

        private static int CalculateNumberOfRows(int m, int k, int c)
        {
            int r = ((m + 1 + k) / c) + 1;
            if (c * r >= (m + 1 + k + c))
                r--;

            return r;
        }

        public static (int Columns, int Rows) CalculateDimensions(int dataWords, int eccWords)
        {
            double ratio = 0.0;
            int columns = 0;
            int rows = 0;

            for (int c = MinimumNumberOfColumns; c <= MaximumNumberOfColumns; c++)
            {
                var r = CalculateNumberOfRows(dataWords, eccWords, c);

                if (r < MinimumNumberOfRows)
                    break;

                if (r > MaximumNumberOfRows)
                    continue;

                double newRatio = (17.0 * columns + 69.0) / (rows * ModuleHeight);
                if (rows != 0 && Math.Abs(newRatio - PreferredRatio) > Math.Abs(ratio - PreferredRatio))
                    continue;

                ratio = newRatio;
                columns = c;
                rows = r;
            }

            if (rows == 0)
            {
                var r = CalculateNumberOfRows(dataWords, eccWords, MinimumNumberOfColumns);
                if (r < MinimumNumberOfRows)
                {
                    rows = MinimumNumberOfRows;
                    columns = MinimumNumberOfColumns;
                }
            }

            return (Columns: columns, Rows: rows);
        }
    }
}
