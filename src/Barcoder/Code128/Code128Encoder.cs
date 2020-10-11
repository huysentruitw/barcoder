using System;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Code128
{
    public static class Code128Encoder
    {
        public static IBarcodeIntCS Encode(string content, bool includeChecksum = true, bool gs1ModeEnabled = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (gs1ModeEnabled)
                content = Gs1Encoder.Encode(content, Constants.FNC1);

            char[] contentChars = content.ToCharArray();
            if (contentChars.Length <= 0 || contentChars.Length > 80)
                throw new ArgumentException($"Content length should be between 1 and 80 but got {contentChars.Length}", nameof(content));

            BitList? idxList = GetCodeIndexList(contentChars);
            if (!idxList.HasValue)
                throw new InvalidOperationException($"{content} could not be encoded");

            var result = new BitList();
            var sum = 0;
            var i = 0;
            foreach (var idx in idxList.Value.GetBytes())
            {
                if (i == 0)
                {
                    sum = idx;
                }
                else
                {
                    sum += i * idx;
                }
                result.AddBit(Constants.EncodingTable[idx]);
                i++;
            }
            sum = sum % 103;

            if (includeChecksum)
                result.AddBit(Constants.EncodingTable[sum]);
            result.AddBit(Constants.EncodingTable[Constants.StopSymbol]);
            return new Base1DCodeIntCS(result, BarcodeType.Code128, content, sum, Constants.Margin);
        }

        internal static bool ShouldUseCTable(char[] nextChars, byte currentEncoding)
        {
            var requiredDigits = 4;
            if (currentEncoding == Constants.StartCSymbol)
                requiredDigits = 2;
            if (nextChars.Length < requiredDigits)
                return false;
            for (var i = 0; i < requiredDigits; i++)
            {
                if ((i % 2) == 0 && nextChars[i] == Constants.FNC1)
                {
                    requiredDigits++;
                    if (nextChars.Length < requiredDigits)
                        return false;
                    continue;
                }
                if (nextChars[i] < '0' || nextChars[i] > '9')
                    return false;
            }
            return true;
        }

        private static bool TableContainsChar(string table, char ch)
        {
            return table.Contains(ch)
                || ch == Constants.FNC1
                || ch == Constants.FNC2
                || ch == Constants.FNC3
                || ch == Constants.FNC4;
        }

        internal static bool ShouldUseATable(char[] nextChars, byte currentEncoding)
        {
            var nextChar = nextChars[0];
            if (!TableContainsChar(Constants.BTable, nextChar) || currentEncoding == Constants.StartASymbol)
                return TableContainsChar(Constants.ATable, nextChar);
            if (currentEncoding == 0)
            {
                foreach (var r in nextChars)
                {
                    if (TableContainsChar(Constants.ABTable, r))
                        continue;
                    if (Constants.AOnlyTable.Contains(r))
                        return true;
                    break;
                }
            }
            return false;
        }

        private static BitList? GetCodeIndexList(char[] content)
        {
            var result = new BitList();
            byte curEncoding = 0;
            for (var i = 0; i < content.Length; i++)
            {
                var nextChars = content.Skip(i).ToArray();
                if (ShouldUseCTable(nextChars, curEncoding))
                {
                    if (curEncoding != Constants.StartCSymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Constants.StartCSymbol);
                        }
                        else
                        {
                            result.AddByte(Constants.CodeCSymbol);
                        }
                        curEncoding = Constants.StartCSymbol;
                    }
                    if (content[i] == Constants.FNC1)
                    {
                        result.AddByte(102);
                    }
                    else
                    {
                        var idx = (content[i] - '0') * 10;
                        i++;
                        idx = idx + (content[i] - '0');
                        result.AddByte((byte)idx);
                    }
                }
                else if (ShouldUseATable(nextChars, curEncoding))
                {
                    if (curEncoding != Constants.StartASymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Constants.StartASymbol);
                        }
                        else
                        {
                            result.AddByte(Constants.CodeASymbol);
                        }
                        curEncoding = Constants.StartASymbol;
                    }

                    int idx;
                    switch (content[i])
                    {
                        case Constants.FNC1:
                            idx = 102;
                            break;
                        case Constants.FNC2:
                            idx = 97;
                            break;
                        case Constants.FNC3:
                            idx = 96;
                            break;
                        case Constants.FNC4:
                            idx = 101;
                            break;
                        default:
                            idx = Constants.ATable.IndexOf(content[i]);
                            break;
                    }
                    if (idx < 0)
                    {
                        return null;
                    }
                    result.AddByte((byte)idx);
                }
                else
                {
                    if (curEncoding != Constants.StartBSymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Constants.StartBSymbol);
                        }
                        else
                        {
                            result.AddByte(Constants.CodeBSymbol);
                        }
                        curEncoding = Constants.StartBSymbol;
                    }

                    int idx;
                    switch (content[i])
                    {
                        case Constants.FNC1:
                            idx = 102;
                            break;
                        case Constants.FNC2:
                            idx = 97;
                            break;
                        case Constants.FNC3:
                            idx = 96;
                            break;
                        case Constants.FNC4:
                            idx = 100;
                            break;
                        default:
                            idx = Constants.BTable.IndexOf(content[i]);
                            break;
                    }

                    if (idx < 0)
                    {
                        return null;
                    }
                    result.AddByte((byte)idx);
                }
            }
            return result;
        }
    }
}
