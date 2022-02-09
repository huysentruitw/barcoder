using System;
using System.Collections.Generic;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.DataMatrix
{
    public static class DataMatrixEncoder
    {
        public static IBarcode Encode(string content, int? fixedNumberOfRows = null, int? fixedNumberOfColumns = null, bool gs1ModeEnabled = false)
        {
            var data = gs1ModeEnabled
                ? EncodeGs1(content)
                : EncodeText(content);

            CodeSize size = GetMatchingCodeSize(fixedNumberOfRows, fixedNumberOfColumns, data.Length);

            data = AddPadding(data, size.DataCodewords);
            data = ErrorCorrection.CalculateEcc(data, size);
            var code = Render(data, size)
                ?? throw new InvalidOperationException("Unable to render barcode");
            code.Content = content;
            return code;
        }

        private static CodeSize GetMatchingCodeSize(int? fixedNumberOfRows, int? fixedNumberOfColumns, int dataLength)
        {
            string GetSizeRestrictionText()
                => $"{fixedNumberOfRows?.ToString() ?? "*"} x {fixedNumberOfColumns?.ToString() ?? "*"}";

            IEnumerable<CodeSize> codeSizes = CodeSizes.All;

            if (fixedNumberOfRows.HasValue)
                codeSizes = codeSizes.Where(x => x.Rows == fixedNumberOfRows.Value);

            if (fixedNumberOfColumns.HasValue)
                codeSizes = codeSizes.Where(x => x.Columns == fixedNumberOfColumns.Value);

            return codeSizes.FirstOrDefault(x => x.DataCodewords >= dataLength)
                   ?? throw new InvalidOperationException($"No code size found that fits {dataLength} codewords (size restriction: {GetSizeRestrictionText()})");
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
                int tmp = 129 + r;
                if (tmp > 254)
                    tmp -= 254;

                result.Add((byte)tmp);
            }

            return result.ToArray();
        }
    }
}
