using System;
using Barcoder.Utils;
using System.Collections.Generic;

namespace Barcoder.Rm4scc
{
    public static class Rm4sccEncoder
    {
        private const int BARCODE_HEIGHT = 8;

        public static IBarcode Encode(string content, bool encodeAsKix = false)
        {
            // Determine barcode length
            var nBars = content.Length * 4;
            if (!encodeAsKix)
                nBars += 6; // start, checksum, stop
            var barcodeWidth = nBars * 2 - 1;
            var bitlist = new BitList(barcodeWidth * BARCODE_HEIGHT);

            var i = 0;

            // Start bar
            if (!encodeAsKix)
            {
                SetBar(BarTypes.Ascender, bitlist, i, barcodeWidth);
                i += 2;
            }

            // Actual data
            var codepointList = new List<int>(content.Length);
            foreach (var c in content)
            {
                var codepointIndex = GetCodepointIndexFromChar(c);
                codepointList.Add(codepointIndex);
                SetCodepoint(codepointIndex, bitlist, i, barcodeWidth);
                i += 8;
            }

            // Checksum and stop bar
            if (!encodeAsKix)
            {
                int topHalf = 0, btmHalf = 0;
                foreach (var c in codepointList)
                {
                    topHalf += ConstantsAndEnums.Symbols[c][0].HasFlag(BarTypes.Ascender) ? 4 : 0;
                    topHalf += ConstantsAndEnums.Symbols[c][1].HasFlag(BarTypes.Ascender) ? 2 : 0;
                    topHalf += ConstantsAndEnums.Symbols[c][2].HasFlag(BarTypes.Ascender) ? 1 : 0;
                    topHalf += ConstantsAndEnums.Symbols[c][3].HasFlag(BarTypes.Ascender) ? 0 : 0;

                    btmHalf += ConstantsAndEnums.Symbols[c][0].HasFlag(BarTypes.Descender) ? 4 : 0;
                    btmHalf += ConstantsAndEnums.Symbols[c][1].HasFlag(BarTypes.Descender) ? 2 : 0;
                    btmHalf += ConstantsAndEnums.Symbols[c][2].HasFlag(BarTypes.Descender) ? 1 : 0;
                    btmHalf += ConstantsAndEnums.Symbols[c][3].HasFlag(BarTypes.Descender) ? 0 : 0;
                }

                // Find checksum 'character'
                var topIdx = (topHalf % 6 == 0) ? 5 : (topHalf % 6 - 1);
                var btmIdx = (btmHalf % 6 == 0) ? 5 : (btmHalf % 6 - 1);
                var checkCpIndex = topIdx * 6 + btmIdx;
                SetCodepoint(checkCpIndex, bitlist, i, barcodeWidth);
                i += 8;

                // Stop bar
                SetBar(BarTypes.FullHeight, bitlist, i, barcodeWidth);
            }

            return new Rm4sccCode(bitlist, barcodeWidth, encodeAsKix);
        }

        private static int GetCodepointIndexFromChar(char character)
        {
            if (character >= '0' && character <= '9')
                return character - '0';
            else if (character >= 'A' && character <= 'Z')
                return character - 'A' + 10;
            else
                throw new Exception($"Character 0x{(int)character:x2} is not supported by RM4SC encoding");
        }

        private static void SetCodepoint(int codepointIndex, BitList bitlist, int xStartPosition, int barcodeWidth)
        {
            var x = xStartPosition;
            foreach (BarTypes barType in ConstantsAndEnums.Symbols[codepointIndex])
            {
                SetBar(barType, bitlist, x, barcodeWidth);
                x += 2;
            }
        }

        private static void SetBar(BarTypes bars, BitList bitlist, int xPosition, int barcodeWidth)
        {
            SetBit(bitlist, xPosition, 3, barcodeWidth);
            SetBit(bitlist, xPosition, 4, barcodeWidth);
            if (bars.HasFlag(BarTypes.Descender))
            {
                SetBit(bitlist, xPosition, 5, barcodeWidth);
                SetBit(bitlist, xPosition, 6, barcodeWidth);
                SetBit(bitlist, xPosition, 7, barcodeWidth);
            }
            if (bars.HasFlag(BarTypes.Ascender))
            {
                SetBit(bitlist, xPosition, 0, barcodeWidth);
                SetBit(bitlist, xPosition, 1, barcodeWidth);
                SetBit(bitlist, xPosition, 2, barcodeWidth);
            }
        }

        private static void SetBit(this BitList bitlist, int x, int y, int barcodeWidth)
        {
            bitlist.SetBit(y * barcodeWidth + x, true);
        }
    }
}
