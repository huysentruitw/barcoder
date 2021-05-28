using System;
using System.Linq;
using System.Text.RegularExpressions;
using Barcoder.Utils;

namespace Barcoder.UpcA
{
    public static class UpcAEncoder
    {
        public static IBarcode Encode(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (!Regex.IsMatch(content, @"^[0-9]*$")) throw new InvalidOperationException("Can only encode numerical digits (0-9)");

            var checksum = 0;
            if (content.Length == 11)
            {
                char checksumChar = CalcChecksum(content);
                content += checksumChar;
                checksum = checksumChar - '0';
            }
            else if (content.Length == 12)
            {
                char checksumChar = CalcChecksum(content.Substring(0, content.Length - 1));
                if (content[content.Length - 1] != checksumChar)
                    throw new InvalidOperationException("Checksum mismatch");
                checksum = checksumChar - '0';
            }

            if (content.Length == 12)
            {
                BitList result = EncodeUpcA(content);
                return new Base1DCodeIntCS(result, BarcodeType.UPCA, content, checksum, Constants.Margin);
            }

            throw new InvalidOperationException("Invalid content length. Should be 11 if the code does not include a checksum, 12 if the code already includes a checksum");
        }

        private static BitList EncodeUpcA(string content)
        {
            var result = new BitList();

            // Start bars
            result.AddBit(true, false, true);

            var cpos = 0;
            foreach (var r in content)
            {
                Constants.EncodedNumber num = Constants.EncodingTable[r];
                bool[] data = cpos < 6 ? num.Left : num.Right;

                // Middle bars
                if (cpos == 6)
                    result.AddBit(false, true, false, true, false);

                result.AddBit(data);
                cpos++;
            }

            // Stop bars
            result.AddBit(true, false, true);

            return result;
        }

        private static char CalcChecksum(string content)
        {
            var numVals = content.Select(x => x - '0').ToArray(); // Convert UTF-16 string to array of numeric values
            var result = 3 * (numVals[0] + numVals[2] + numVals[4] + numVals[6] + numVals[8] + numVals[10]);
            result += numVals[1] + numVals[3] + numVals[5] + numVals[7] + numVals[9];
            result %= 10;

            result = result == 0 ? 0 : (10 - result);
            return (char)(result + '0'); // Convert numeric value to UTF-16 value
        }
    }
}
