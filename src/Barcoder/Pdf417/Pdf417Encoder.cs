using System;
using System.Collections.Generic;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Pdf417
{
    public static class Pdf417Encoder
    {
        private const int PaddingCodeword = 900;

        // Encodes the given data as PDF417 barcode.
        // securityLevel should be between 0 and 8. The higher the number, the more
        // additional error-correction codes are added.
        public static IBarcode Encode(string data, byte securityLevel)
        {
            if (securityLevel >= 9)
                throw new ArgumentOutOfRangeException($"Invalid security level {securityLevel}");

            int[] dataWords = HighLevelEncoding.Encode(data);

            (int columns, int rows) = Dimensions.CalculateDimensions(dataWords.Length, ErrorCorrection.GetErrorCorrectionWordCount(securityLevel));

            if (columns < Dimensions.MinimumNumberOfColumns || columns > Dimensions.MaximumNumberOfColumns
                || rows < Dimensions.MinimumNumberOfRows || rows > Dimensions.MaximumNumberOfRows)
                throw new InvalidOperationException($"Unable to fit data in barcode");

            int[] codeWords = EncodeData(dataWords, columns, securityLevel);

            var grid = new List<int[]>();
            for (int i = 0; i < codeWords.Length; i += columns)
            {
                int length = Math.Min(i + columns, codeWords.Length) - i;
                var row = new int[length];
                Array.Copy(codeWords, i, row, 0, length);
                grid.Add(row);
            }

            var codes = new List<int[]>();
            for (int rowNum = 0; rowNum < grid.Count; rowNum++)
            {
                int[] row = grid[rowNum];
                int table = rowNum % 3;
                var rowCodes = new List<int>(columns + 4);

                rowCodes.Add(CodeWord.StartWord);
                rowCodes.Add(CodeWord.Get(table, GetLeftCodeWord(rowNum, rows, columns, securityLevel)));

                foreach (var word in row)
                    rowCodes.Add(CodeWord.Get(table, word));

                rowCodes.Add(CodeWord.Get(table, GetRightCodeWord(rowNum, rows, columns, securityLevel)));
                rowCodes.Add(CodeWord.StopWord);

                codes.Add(rowCodes.ToArray());
            }

            return new Pdf417Code(data, RenderBarcode(codes), (columns + 4) * 17 + 1);
        }

        private static int[] EncodeData(int[] dataWords, int columns, byte securityLevel)
        {
            int dataCount = dataWords.Length;

            int ecCount = ErrorCorrection.GetErrorCorrectionWordCount(securityLevel);

            int[] padWords = GetPadding(dataCount, ecCount, columns);
            dataWords = dataWords.Concat(padWords).ToArray();

            int length = dataWords.Length + 1;
            dataWords = new[] { length }.Concat(dataWords).ToArray();

            int[] ecWords = ErrorCorrection.Compute(securityLevel, dataWords);

            return dataWords.Concat(ecWords).ToArray();
        }

        private static int GetLeftCodeWord(int rowNum, int rows, int columns, byte securityLevel)
        {
            int tableId = rowNum % 3;

            int x = 0;

            switch (tableId)
            {
                case 0:
                    x = (rows - 3) / 3;
                    break;
                case 1:
                    x = securityLevel * 3;
                    x += (rows - 1) % 3;
                    break;
                case 2:
                    x = columns - 1;
                    break;
            }

            return 30 * (rowNum / 3) + x;
        }

        private static int GetRightCodeWord(int rowNum, int rows, int columns, byte securityLevel)
        {
            int tableId = rowNum % 3;

            int x = 0;

            switch (tableId)
            {
                case 0:
                    x = columns - 1;
                    break;
                case 1:
                    x = (rows - 1) / 3;
                    break;
                case 2:
                    x = securityLevel * 3;
                    x += (rows - 1) % 3;
                    break;
            }

            return 30 * (rowNum / 3) + x;
        }

        private static int[] GetPadding(int dataCount, int ecCount, int columns)
        {
            int totalCount = dataCount + ecCount + 1;
            int mod = totalCount % columns;

            int[] padding = Array.Empty<int>();

            if (mod > 0)
            {
                int padCount = columns - mod;
                padding = new int[padCount];
                for (int i = 0; i < padCount; i++)
                    padding[i] = PaddingCodeword;
            }

            return padding;
        }

        private static BitList RenderBarcode(IEnumerable<int[]> codes)
        {
            var bl = new BitList();
            foreach (int[] row in codes)
            {
                var lastIdx = row.Length - 1;

                for (int i = 0; i < row.Length; i++)
                {
                    int col = row[i];
                    if (i == lastIdx)
                        bl.AddBits((uint)col, 18);
                    else
                        bl.AddBits((uint)col, 17);
                }
            }

            return bl;
        }
    }
}
