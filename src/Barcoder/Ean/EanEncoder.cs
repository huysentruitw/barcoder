using System;
using System.Text.RegularExpressions;
using Barcoder.Utils;

namespace Barcoder.Ean
{
    public static class EanEncoder
    {
        public static IBarcodeIntCS Encode(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (!Regex.IsMatch(content, @"^[0-9]*$")) throw new InvalidOperationException("Can only encode numerical digits (0-9)");

            var checksum = 0;
            if (content.Length == 7 || content.Length == 12)
            {
                char checksumChar = CalcChecksum(content);
                content += checksumChar;
                checksum = checksumChar - '0';
            }
            else if (content.Length == 8 || content.Length == 13)
            {
                char checksumChar = CalcChecksum(content.Substring(0, content.Length - 1));
                if (content[content.Length - 1] != checksumChar)
                    throw new InvalidOperationException("Checksum mismatch");
                checksum = checksumChar - '0';
            }

            if (content.Length == 8)
            {
                BitList result = EncodeEan8(content);
                return new Base1DCodeIntCS(result, BarcodeType.EAN8, content, checksum, Constants.Margin);
            }
            else if (content.Length == 13)
            {
                BitList result = EncodeEan13(content);
                return new Base1DCodeIntCS(result, BarcodeType.EAN13, content, checksum, Constants.Margin);
            }

            throw new InvalidOperationException("Invalid content length. Should be 7 or 12 if the code does not include a checksum, 8 or 13 if the code already includes a checksum");
        }

        private static BitList EncodeEan8(string content)
        {
            var result = new BitList();
            result.AddBit(true, false, true);
            var cpos = 0;
            foreach (var r in content)
            {
                Constants.EncodedNumber num = Constants.EncodingTable[r];
                bool[] data = cpos < 4 ? num.LeftOdd : num.Right;
                if (cpos == 4)
                    result.AddBit(false, true, false, true, false);
                result.AddBit(data);
                cpos++;
            }
            result.AddBit(true, false, true);
            return result;
        }

        private static BitList EncodeEan13(string content)
        {
            var result = new BitList();
            result.AddBit(true, false, true);
            bool[] firstNum = null;
            var cpos = 0;
            foreach (var r in content)
            {
                Constants.EncodedNumber num = Constants.EncodingTable[r];
                if (firstNum == null)
                {
                    firstNum = num.Checksum;
                    cpos++;
                    continue;
                }
                bool[] data = cpos < 7 ? firstNum[cpos - 1] ? num.LeftEven : num.LeftOdd : num.Right;
                if (cpos == 7)
                    result.AddBit(false, true, false, true, false);
                result.AddBit(data);
                cpos++;
            }
            result.AddBit(true, false, true);
            return result;
        }

        private static char CalcChecksum(string content)
        {
            var x3 = content.Length == 7;
            var sum = 0;
            foreach (var r in content)
            {
                var curNum = r - '0';
                if (x3)
                    curNum = curNum * 3;
                x3 = !x3;
                sum += curNum;
            }

            return (char)(((10 - (sum % 10)) % 10) + '0');
        }
    }
}
