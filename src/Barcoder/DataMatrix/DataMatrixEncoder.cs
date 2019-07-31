using System;
using System.Collections.Generic;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.DataMatrix
{
    public static class DataMatrixEncoder
    {
        public static IBarcode Encode(string content, int? fixedNumberOfRows = null, bool gs1ModeEnabled = false)
        {
            var data = gs1ModeEnabled
                ? EncodeGs1(content)
                : EncodeText(content);

            CodeSize size = fixedNumberOfRows.HasValue
                ? GetFixedCodeSizeForData(fixedNumberOfRows.Value, data.Length)
                : GetSmallestCodeSizeForData(data.Length);

            data = AddPadding(data, size.DataCodewords);
            data = ErrorCorrection.CalculateEcc(data, size);
            var code = Render(data, size)
                ?? throw new InvalidOperationException("Unable to render barcode");
            code.Content = content;
            return code;
        }

        private static CodeSize GetFixedCodeSizeForData(int fixedNumberOfRows, int dataLength)
        {
            CodeSize codeSize = CodeSizes.All.FirstOrDefault(x => x.Rows == fixedNumberOfRows)
                ?? throw new InvalidOperationException($"No code size found with fixed number of rows {fixedNumberOfRows}");
            if (codeSize.DataCodewords < dataLength)
                throw new InvalidOperationException($"The fixed code size does not fit {dataLength} codewords");
            return codeSize;
        }

        private static CodeSize GetSmallestCodeSizeForData(int dataLength)
        {
            return CodeSizes.All.FirstOrDefault(x => x.DataCodewords >= dataLength)
                ?? throw new InvalidOperationException($"No code size found that fits {dataLength} codewords");
        }

        private static DataMatrixCode Render(byte[] data, CodeSize size)
        {
            var codeLayout = new CodeLayout(size);
            codeLayout.SetValues(data);
            return codeLayout.Merge();
        }

        internal static byte[] EncodeText(string content, bool skipFnc1 = false)
        {
            var result = new List<byte>();
            for (int i = 0; i < content.Length;)
            {
                char c = content[i];
                i++;

                if (c >= '0' && c <= '9' && i < content.Length && content[i] >= '0' && content[i] <= '9')
                {
                    // Two numbers...
                    char c2 = content[i];
                    i++;
                    result.Add((byte)((c - '0') * 10 + (c2 - '0') + 130));
                }
                else if (c == DataMatrixSpecialCodewords.FNC1 && skipFnc1)
                {
                    result.Add((byte)c);
                }
                else if (c > 127)
                {
                    result.Add(DataMatrixSpecialCodewords.UpperShiftToExtendedAscii);
                    result.Add((byte)(c - 127));
                }
                else
                {
                    result.Add((byte)(c + 1));
                }
            }

            return result.ToArray();
        }

        internal static byte[] EncodeGs1(string content)
            => EncodeText(Gs1Encoder.Encode(content, (char)DataMatrixSpecialCodewords.FNC1), skipFnc1: true);

        internal static byte[] AddPadding(byte[] data, int toCount)
        {
            var result = new List<byte>(data);
            if (result.Count < toCount)
                result.Add(129);

            while (result.Count < toCount)
            {
                int r = ((149 * (result.Count + 1)) % 253) + 1;
                result.Add((byte)((129 + r) % 254));
            }

            return result.ToArray();
        }
    }
}
