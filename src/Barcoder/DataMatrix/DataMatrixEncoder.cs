using System;
using System.Collections.Generic;
using System.Linq;

namespace Barcoder.DataMatrix
{
    public static class DataMatrixEncoder
    {
        public static IBarcode Encode(string content, byte fixedNumberOfRows = 0)
        {
            var data = EncodeText(content);

            CodeSize size = fixedNumberOfRows > 0
                ? GetFixedCodeSizeForData(fixedNumberOfRows, data.Length)
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
                throw new ArgumentException($"The fixed code size does not fit {dataLength} codewords");
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

        internal static byte[] EncodeText(string content)
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
                else if (c > 127)
                {
                    // Not correct... needs to be redone later...
                    result.Add(235);
                    result.Add((byte)(c - 127));
                }
                else
                {
                    result.Add((byte)(c + 1));
                }
            }

            return result.ToArray();
        }

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
