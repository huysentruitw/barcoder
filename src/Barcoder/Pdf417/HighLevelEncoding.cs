using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Barcoder.Pdf417
{
    internal static class HighLevelEncoding
    {
        internal enum EncodingMode
        {
            Text,
            Numeric,
            Binary,
        }

        private enum SubMode
        {
            Upper,
            Lower,
            Mixed,
            Punctuation,
        }

        private const int LatchToText = 900;
        private const int LatchToBytePadded = 901;
        private const int LatchToNumeric = 902;
        private const int LatchToByte = 924;
        private const int ShiftToByte = 913;

        private const int MinNumericCount = 13;

        private static readonly IReadOnlyDictionary<char, int> MixedMap;
        private static readonly IReadOnlyDictionary<char, int> PunctuationMap;

        static HighLevelEncoding()
        {
            var mixedMap = new Dictionary<char, int>();
            int[] mixedRaw = new[]
            {
                48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 38, 13, 9, 44, 58,
                35, 45, 46, 36, 47, 43, 37, 42, 61, 94, 0, 32, 0, 0, 0,
            };
            for (int i = 0; i < mixedRaw.Length; i++)
            {
                char ch = (char)mixedRaw[i];
                if (ch > 0) mixedMap[ch] = i;
            }
            MixedMap = mixedMap;

            var punctuationMap = new Dictionary<char, int>();
            int[] punctuationRaw = new[]
            {
                59, 60, 62, 64, 91, 92, 93, 95, 96, 126, 33, 13, 9, 44, 58,
                10, 45, 46, 36, 47, 34, 124, 42, 40, 41, 63, 123, 125, 39, 0,
            };
            for (int i = 0; i < punctuationRaw.Length; i++)
            {
                char ch = (char)punctuationRaw[i];
                if (ch > 0) punctuationMap[ch] = i;
            }
            PunctuationMap = punctuationMap;
        }

        public static int[] Encode(string data)
        {
            EncodingMode encodingMode = EncodingMode.Text;
            SubMode textSubMode = SubMode.Upper;

            int[] result = Array.Empty<int>();

            while (data.Length > 0)
            {
                var numericCount = DetermineConsecutiveDigitCount(data);
                if (numericCount >= MinNumericCount || numericCount == data.Length)
                {
                    result = result.Append(LatchToNumeric).ToArray();
                    encodingMode = EncodingMode.Numeric;
                    textSubMode = SubMode.Upper;
                    int[] numData = EncodeNumeric(data.Substring(0, numericCount));
                    result = result.Concat(numData).ToArray();
                    data = data.Substring(numericCount);
                }
                else
                {
                    var textCount = DetermineConsecutiveTextCount(data);
                    if (textCount >= 5 || textCount == data.Length)
                    {
                        if (encodingMode != EncodingMode.Text)
                        {
                            result = result.Append(LatchToText).ToArray();
                            encodingMode = EncodingMode.Text;
                            textSubMode = SubMode.Upper;
                        }

                        int[] txtData = EncodeText(data.Substring(0, textCount), ref textSubMode);
                        result = result.Concat(txtData).ToArray();
                        data = data.Substring(textCount);
                    }
                    else
                    {
                        var binaryCount = DetermineConsecutiveBinaryCount(data);
                        if (binaryCount == 0)
                            binaryCount = 1;

                        string bytes = data.Substring(0, binaryCount);
                        if (bytes.Length != 1 || encodingMode != EncodingMode.Text)
                        {
                            encodingMode = EncodingMode.Binary;
                            textSubMode = SubMode.Upper;
                        }
                        int[] byteData = EncodeBinary(bytes, encodingMode);
                        result = result.Concat(byteData).ToArray();
                        data = data.Substring(binaryCount);
                    }
                }
            }

            return result;
        }

        private static int DetermineConsecutiveDigitCount(string data)
        {
            int cnt = 0;
            foreach (char r in data)
            {
                if (r < '0' || r > '9')
                    break;

                cnt++;
            }

            return cnt;
        }

        private static int[] EncodeNumeric(string digits)
        {
            int digitCount = digits.Length;
            int chunkCount = digitCount / 44;
            if (digitCount % 44 != 0)
                chunkCount++;

            int[] codewords = Array.Empty<int>();

            for (int i = 0; i < chunkCount; i++)
            {
                int start = i * 44;
                int end = start + 44;
                if (end > digitCount)
                    end = digitCount;

                var chunk = digits.Substring(start, end - start);

                if (!BigInteger.TryParse($"1{chunk}", out BigInteger chunkNum))
                    throw new InvalidOperationException($"Failed converting: {chunk}");

                int[] cws = Array.Empty<int>();

                while (chunkNum > 0)
                {
                    BigInteger cw = chunkNum % 900;
                    chunkNum = chunkNum / 900;
                    cws = new[] { (int)cw }.Concat(cws).ToArray();
                }

                codewords = codewords.Concat(cws).ToArray();
            }

            return codewords;
        }

        private static int DetermineConsecutiveTextCount(string msg)
        {
            int result = 0;

            bool IsText(char ch) => ch == '\t' || ch == '\n' || ch == '\r' || (ch >= 32 && ch <= 126);

            for (int i = 0; i < msg.Length; i++)
            {
                int numericCount = DetermineConsecutiveDigitCount(msg.Substring(i));
                if (numericCount > MinNumericCount || (numericCount == 0 && !IsText(msg[i])))
                    break;

                result++;
            }

            return result;
        }

        private static int[] EncodeText(string text, ref SubMode subMode)
        {
            bool IsAlphaUpper(char ch) => ch == ' ' || (ch >= 'A' && ch <= 'Z');
            bool IsAlphaLower(char ch) => ch == ' ' || (ch >= 'a' && ch <= 'z');
            bool IsMixed(char ch) => MixedMap.ContainsKey(ch);
	        bool IsPunctuation(char ch) => PunctuationMap.ContainsKey(ch);

            int idx = 0;
            int[] tmp = Array.Empty<int>();
	        while (idx < text.Length)
            {
                char ch = text[idx];
		        switch (subMode)
                {
		        case SubMode.Upper:
			        if (IsAlphaUpper(ch))
                    {
                        if (ch == ' ')
                            tmp = tmp.Append(26).ToArray(); //space
                        else
                            tmp = tmp.Append(ch - 'A').ToArray();
                    }
                    else
                    {
				        if (IsAlphaLower(ch))
                        {
                            subMode = SubMode.Lower;
                            tmp = tmp.Append(27).ToArray(); // lower latch
                            continue;
                        }

                        if (IsMixed(ch))
                        {
                            subMode = SubMode.Mixed;
                            tmp = tmp.Append(28).ToArray(); // mixed latch
                            continue;
                        }

                        tmp = tmp.Append(29).ToArray(); // punctuation switch
                        tmp = tmp.Append(PunctuationMap[ch]).ToArray();
                    }
                    break;

                case SubMode.Lower:
			        if (IsAlphaLower(ch))
                    {
                        if (ch == ' ')
                            tmp = tmp.Append(26).ToArray(); //space
                        else
                            tmp = tmp.Append(ch - 'a').ToArray();
                    }
                    else
                    {
				        if (IsAlphaUpper(ch))
                        {
                            tmp = tmp.Append(27).ToArray(); //upper switch
                            tmp = tmp.Append(ch - 'A').ToArray();
                            break;
                        }

                        if (IsMixed(ch))
                        {
                            subMode = SubMode.Mixed;
                            tmp = tmp.Append(28).ToArray(); //mixed latch
                            continue;
                        }

                        tmp = tmp.Append(29).ToArray(); //punctuation switch
                        tmp = tmp.Append(PunctuationMap[ch]).ToArray();
                    }
                    break;

		        case SubMode.Mixed:
			        if (IsMixed(ch))
                    {
                        tmp = tmp.Append(MixedMap[ch]).ToArray();
                    }
                    else
                    {
				        if (IsAlphaUpper(ch))
                        {
                            subMode = SubMode.Upper;
                            tmp = tmp.Append(28).ToArray(); //upper latch
                            continue;
                        }

                        if (IsAlphaLower(ch))
                        {
                            subMode = SubMode.Lower;
                            tmp = tmp.Append(27).ToArray(); //lower latch
                            continue;
                        }

					    if (idx + 1 < text.Length)
                        {
                            char next = text[idx + 1];
						    if (IsPunctuation(next))
                            {
                                subMode = SubMode.Punctuation;
                                tmp = tmp.Append(25).ToArray(); //punctuation latch
                                continue;
                            }
					    }

                        tmp = tmp.Append(29).ToArray(); //punctuation switch
                        tmp = tmp.Append(PunctuationMap[ch]).ToArray();
                    }
                    break;

                case SubMode.Punctuation:
			        if (IsPunctuation(ch))
                    {
                        tmp = tmp.Append(PunctuationMap[ch]).ToArray();
                    }
                    else
                    {
                        subMode = SubMode.Upper;
                        tmp = tmp.Append(29).ToArray(); //upper latch
                        continue;
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown submode {subMode}");
		        }

                idx++;
            }

            int h = 0;
            int[] result = Array.Empty<int>();
            for (int i = 0; i < tmp.Length; i++)
            {
                int val = tmp[i];
		        if (i % 2 != 0)
                {
                    h = (h * 30) + val;
                    result = result.Append(h).ToArray();
                }
                else
                {
                    h = val;
                }
	        }

            if (tmp.Length % 2 != 0)
                result = result.Append((h * 30) + 29).ToArray();

            return result;
        }

        private static int DetermineConsecutiveBinaryCount(string msg)
        {
            int result = 0;

            for (int i = 0; i < msg.Length; i++)
            {
                int numericCount = DetermineConsecutiveDigitCount(msg.Substring(i));
                if (numericCount >= MinNumericCount)
                    break;
                int textCount = DetermineConsecutiveTextCount(msg.Substring(i));
                if (textCount > 5)
                    break;
                result++;
            }

            return result;
        }

        internal static int[] EncodeBinary(string data, EncodingMode startMode)
        {
            int[] result = Array.Empty<int>();

            int count = data.Length;
            if (count == 1 && startMode == EncodingMode.Text)
                result = result.Append(ShiftToByte).ToArray();
            else if ((count % 6) == 0)
                result = result.Append(LatchToByte).ToArray();
            else
                result = result.Append(LatchToBytePadded).ToArray();

            int idx = 0;

            // Encode six-packs
            if (count >= 6)
            {
                var words = new int[5];
                while ((count - idx) >= 6)
                {
                    long t = 0L;
                    for (int i = 0; i < 6; i++)
                    {
                        t = t << 8;
                        t += data[idx + i];
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        words[4 - i] = (int)(t % 900);
                        t = t / 900;
                    }

                    result = result.Concat(words).ToArray();
                    idx += 6;
                }
            }

            // Encode rest (remaining n < 5 bytes if any)
            for (int i = idx; i < count; i++)
                result = result.Append(data[i] & 0xff).ToArray();

            return result;
        }
    }
}
