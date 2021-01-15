using System;
using Barcoder.Utils;
using System.Collections.Generic;

namespace Barcoder.RoyalMail
{
    public static class RoyalMailFourStateCodeEncoder
    {
        public static IBarcode Encode(string content)
            => Encode(content, true);

        internal static IBarcode Encode(string content, bool includeHeaders)
        {
            // Determine barcode length
            var numberOfBars = content.Length * 4;
            if (includeHeaders)
                numberOfBars += 6; // start, checksum, stop

            var barcodeWidth = numberOfBars * 2 - 1;
            var bits = new BitList(barcodeWidth * Constants.BARCODE_HEIGHT);

            var i = 0;

            // Start bar
            if (includeHeaders)
            {
                SetBar(BarTypes.Ascender, bits, i, barcodeWidth);
                i += 2;
            }

            // Actual data
            var codepointList = new List<int>(content.Length);
            foreach (var c in content)
            {
                var codepointIndex = GetCodepointIndexFromChar(c);
                codepointList.Add(codepointIndex);
                SetCodepoint(codepointIndex, bits, i, barcodeWidth);
                i += 8;
            }

            // Checksum and stop bar
            if (includeHeaders)
            {
                int topHalf = 0, bottomHalf = 0;
                foreach (var c in codepointList)
                {
                    topHalf += Constants.Symbols[c][0].HasFlag(BarTypes.Ascender) ? 4 : 0;
                    topHalf += Constants.Symbols[c][1].HasFlag(BarTypes.Ascender) ? 2 : 0;
                    topHalf += Constants.Symbols[c][2].HasFlag(BarTypes.Ascender) ? 1 : 0;
                    topHalf += Constants.Symbols[c][3].HasFlag(BarTypes.Ascender) ? 0 : 0;

                    bottomHalf += Constants.Symbols[c][0].HasFlag(BarTypes.Descender) ? 4 : 0;
                    bottomHalf += Constants.Symbols[c][1].HasFlag(BarTypes.Descender) ? 2 : 0;
                    bottomHalf += Constants.Symbols[c][2].HasFlag(BarTypes.Descender) ? 1 : 0;
                    bottomHalf += Constants.Symbols[c][3].HasFlag(BarTypes.Descender) ? 0 : 0;
                }

                // Find checksum 'character'
                var topIdx = (topHalf % 6 == 0) ? 5 : (topHalf % 6 - 1);
                var btmIdx = (bottomHalf % 6 == 0) ? 5 : (bottomHalf % 6 - 1);
                var checkCpIndex = topIdx * 6 + btmIdx;
                SetCodepoint(checkCpIndex, bits, i, barcodeWidth);
                i += 8;

                // Stop bar
                SetBar(BarTypes.FullHeight, bits, i, barcodeWidth);
            }

            return new RoyalMailFourStateCode(content, bits, barcodeWidth);
        }

        private static int GetCodepointIndexFromChar(char character)
        {
            if (character >= '0' && character <= '9')
                return character - '0';

            if (character >= 'A' && character <= 'Z')
                return character - 'A' + 10;

            throw new Exception($"Character 0x{(int)character:x2} is not supported by RM4SC encoding");
        }

        private static void SetCodepoint(int codepointIndex, BitList bits, int xStartPosition, int barcodeWidth)
        {
            var x = xStartPosition;
            foreach (BarTypes barType in Constants.Symbols[codepointIndex])
            {
                SetBar(barType, bits, x, barcodeWidth);
                x += 2;
            }
        }

        private static void SetBar(BarTypes bars, BitList bits, int xPosition, int barcodeWidth)
        {
            SetBit(bits, xPosition, 3, barcodeWidth);
            SetBit(bits, xPosition, 4, barcodeWidth);

            if (bars.HasFlag(BarTypes.Descender))
            {
                SetBit(bits, xPosition, 5, barcodeWidth);
                SetBit(bits, xPosition, 6, barcodeWidth);
                SetBit(bits, xPosition, 7, barcodeWidth);
            }

            if (bars.HasFlag(BarTypes.Ascender))
            {
                SetBit(bits, xPosition, 0, barcodeWidth);
                SetBit(bits, xPosition, 1, barcodeWidth);
                SetBit(bits, xPosition, 2, barcodeWidth);
            }
        }

        private static void SetBit(this BitList bits, int x, int y, int barcodeWidth)
        {
            bits.SetBit(y * barcodeWidth + x, true);
        }
    }
}
