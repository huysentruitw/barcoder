using System;
using System.Linq;
using System.Text.RegularExpressions;
using Barcoder.Utils;

namespace Barcoder.UpcE
{
    public static class UpcEEncoder
    {
        public static IBarcode Encode(string content, UpcENumberSystem numberSystem)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (!Regex.IsMatch(content, @"^[0-9]*$"))
                throw new InvalidOperationException("Can only encode numerical digits (0-9)");

            if (numberSystem != UpcENumberSystem.Zero && numberSystem != UpcENumberSystem.One)
                throw new InvalidOperationException("Only number systems 0 and 1 are supported by UPC E");

            if (content.Length != 6)
                throw new InvalidOperationException("Invalid content length. Should be 6");

            BitList bitlist = EncodeUpcE(content, numberSystem);
            return new Base1DCode(bitlist, BarcodeType.UPCE, content, Constants.Margin);
        }

        private static BitList EncodeUpcE(string content, UpcENumberSystem numberSystem)
        {
            var result = new BitList();

            // Find the correct parity pattern
            // To find it we need the check digit of the UPC-A for which this UPC-E barcode encodes
            var upcA = GetUpcAFromUpcE(content, numberSystem);
            var upcACheckDigit = upcA.Last();
            Constants.ParityPatterns parityPatternTable = Constants.ParityPatternTable[upcACheckDigit];
            Constants.Parity[] parityPattern = numberSystem == UpcENumberSystem.Zero ? 
                parityPatternTable.NumberSystemZero : parityPatternTable.NumberSystemOne;

            // Start bars
            result.AddBit(true, false, true);
            
            // Data bars
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                Constants.EncodedNumber num = Constants.EncodingTable[c];
                Constants.Parity parity = parityPattern[i];

                if (parity == Constants.Parity.Even)
                    result.AddBit(num.Even);
                else
                    result.AddBit(num.Odd);
            }

            // Stop bars
            result.AddBit(false, true, false, true, false, true);

            return result;
        }

        private static string GetUpcAFromUpcE(string content, UpcENumberSystem numberSystem)
        {
            var firstChar = numberSystem == UpcENumberSystem.Zero ? '0' : '1';
            var upcA = firstChar.ToString();

            switch (content.Last())
            {
            case '0':
            case '1':
            case '2':
                upcA += $"{content.Substring(0, 2)}{content.Last()}0000{content.Substring(2, 3)}";
                break;
            case '3':
                upcA += $"{content.Substring(0, 3)}00000{content.Substring(3, 2)}";
                break;
            case '4':
                upcA += $"{content.Substring(0, 4)}00000{content.Substring(4, 1)}";
                break;
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                upcA += $"{content.Substring(0, 5)}0000{content.Last()}";
                break;
            }

            upcA += CalculatedUpcAChecksum(upcA);
            return upcA;
        }

        private static char CalculatedUpcAChecksum(string content)
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
