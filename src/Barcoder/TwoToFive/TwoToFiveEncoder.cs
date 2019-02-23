using System;
using System.Text.RegularExpressions;
using Barcoder.Utils;

namespace Barcoder.TwoToFive
{
    public static class TwoToFiveEncoder
    {
        public static IBarcodeIntCS Encode(string content, bool interleaved, bool includeChecksum)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (!Regex.IsMatch(content, @"^[0-9]*$"))
                throw new InvalidOperationException("Can only encode numerical digits (0-9)");

            char checksum = GetChecksum(content);
            if (includeChecksum)
                content = content + checksum;

            if (interleaved && (content.Length % 2) == 1)
                throw new InvalidOperationException("Can only encode an even number of digits in interleaved mode");

            Constants.EncodeInfo mode = Constants.Modes[interleaved];
            var resBits = new BitList();
            resBits.AddBit(mode.Start);

            char? lastChar = null;
            foreach (var r in content)
            {
                bool[] a, b;
                if (interleaved)
                {
                    if (!lastChar.HasValue)
                    {
                        lastChar = r;
                        continue;
                    }
                    else
                    {
                        a = Constants.EncodingTable[lastChar.Value];
                        b = Constants.EncodingTable[r];
                        lastChar = null;
                    }
                }
                else
                {
                    a = Constants.EncodingTable[r];
                    b = Constants.NonInterleavedSpace;
                }

                for (var i = 0; i < Constants.PatternWidth; i++)
                {
                    for (var x = 0; x < mode.Widths[a[i]]; x++)
                        resBits.AddBit(true);
                    for (var x = 0; x < mode.Widths[b[i]]; x++)
                        resBits.AddBit(false);
                }
            }

            resBits.AddBit(mode.End);

            return new Base1DCodeIntCS(resBits, interleaved ? BarcodeType.TwoOfFiveInterleaved : BarcodeType.TwoOfFive, content, checksum, Constants.Margin);
        }

        private static char GetChecksum(string content)
        {
            var sum = 0;
            var even = (content.Length % 2) == 1;
            foreach (char r in content)
            {
                var value = r - '0';
                if (even)
                    sum += value * 3;
                else
                    sum += value;
                even = !even;
            }
            sum %= 10;
            return (char)(sum + '0');
        }
    }
}
