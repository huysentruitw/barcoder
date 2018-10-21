using System;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder
{
    public static class Code128
    {
        public static IBarcodeIntCS Encode(string content, bool includeChecksum = true)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

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
                result.AddBit(Code128Constants.EncodingTable[idx]);
                i++;
            }
            sum = sum % 103;

            if (includeChecksum)
                result.AddBit(Code128Constants.EncodingTable[sum]);
            result.AddBit(Code128Constants.EncodingTable[Code128Constants.StopSymbol]);
            return new Base1DCodeIntCS(result, Constants.TypeCode128, content, sum, Code128Constants.Margin);
        }

        internal static bool ShouldUseCTable(char[] nextChars, byte currentEncoding)
        {
            var requiredDigits = 4;
            if (currentEncoding == Code128Constants.StartCSymbol)
                requiredDigits = 2;
            if (nextChars.Length < requiredDigits)
                return false;
            for (var i = 0; i < requiredDigits; i++)
            {
                if ((i % 2) == 0 && nextChars[i] == Code128Constants.FNC1)
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
                || ch == Code128Constants.FNC1
                || ch == Code128Constants.FNC2
                || ch == Code128Constants.FNC3
                || ch == Code128Constants.FNC4;
        }

        internal static bool ShouldUseATable(char[] nextChars, byte currentEncoding)
        {
            var nextChar = nextChars[0];
            if (!TableContainsChar(Code128Constants.BTable, nextChar) || currentEncoding == Code128Constants.StartASymbol)
                return TableContainsChar(Code128Constants.ATable, nextChar);
            if (currentEncoding == 0)
            {
                foreach (var r in nextChars)
                {
                    if (TableContainsChar(Code128Constants.ABTable, r))
                        continue;
                    if (Code128Constants.AOnlyTable.Contains(r))
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
                    if (curEncoding != Code128Constants.StartCSymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Code128Constants.StartCSymbol);
                        }
                        else
                        {
                            result.AddByte(Code128Constants.CodeCSymbol);
                        }
                        curEncoding = Code128Constants.StartCSymbol;
                    }
                    if (content[i] == Code128Constants.FNC1)
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
                    if (curEncoding != Code128Constants.StartASymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Code128Constants.StartASymbol);
                        }
                        else
                        {
                            result.AddByte(Code128Constants.CodeASymbol);
                        }
                        curEncoding = Code128Constants.StartASymbol;
                    }

                    int idx;
                    switch (content[i])
                    {
                        case Code128Constants.FNC1:
                            idx = 102;
                            break;
                        case Code128Constants.FNC2:
                            idx = 97;
                            break;
                        case Code128Constants.FNC3:
                            idx = 96;
                            break;
                        case Code128Constants.FNC4:
                            idx = 101;
                            break;
                        default:
                            idx = Code128Constants.ATable.IndexOf(content[i]);
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
                    if (curEncoding != Code128Constants.StartBSymbol)
                    {
                        if (curEncoding == 0)
                        {
                            result.AddByte(Code128Constants.StartBSymbol);
                        }
                        else
                        {
                            result.AddByte(Code128Constants.CodeBSymbol);
                        }
                        curEncoding = Code128Constants.StartBSymbol;
                    }

                    int idx;
                    switch (content[i])
                    {
                        case Code128Constants.FNC1:
                            idx = 102;
                            break;
                        case Code128Constants.FNC2:
                            idx = 97;
                            break;
                        case Code128Constants.FNC3:
                            idx = 96;
                            break;
                        case Code128Constants.FNC4:
                            idx = 100;
                            break;
                        default:
                            idx = Code128Constants.BTable.IndexOf(content[i]);
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
