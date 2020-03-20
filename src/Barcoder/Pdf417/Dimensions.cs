using System;

namespace Barcoder.Pdf417
{
    internal static class Dimensions
    {
        private const int MinCols = 2;
        private const int MaxCols = 30;
        private const int MinRows = 2;
        private const int MaxRows = 30;
        private const int ModuleHeight = 2;
        private const double PreferredRatio = 3.0;

        private static int CalculateNumberOfRows(int m, int k, int c)
        {
            int r = ((m + 1 + k) / c) + 1;
            if (c * r >= (m + 1 + k + c))
                r--;

            return r;
        }

        public static (int Cols, int Rows) CalculateDimensions(int dataWords, int eccWords)
        {
            double ratio = 0.0;
            int cols = 0;
            int rows = 0;

            for (int c = MinCols; c <= MaxCols; c++)
            {
                var r = CalculateNumberOfRows(dataWords, eccWords, c);

                if (r < MinRows)
                    break;

                if (r > MaxRows)
                    continue;

                double newRatio = (17.0 * cols + 69.0) / (rows * ModuleHeight);
                if (rows != 0 && Math.Abs(newRatio - PreferredRatio) > Math.Abs(ratio - PreferredRatio))
                    continue;

                ratio = newRatio;
                cols = c;
                rows = r;
            }

            if (rows == 0)
            {
                var r = CalculateNumberOfRows(dataWords, eccWords, MinCols);
                if (r < MinRows)
                {
                    rows = MinRows;
                    cols = MinCols;
                }
            }

            return (Cols: cols, Rows: rows);
        }
    }
}
