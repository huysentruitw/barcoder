using System;
using System.Collections.Generic;
using System.Text;
using Barcoder.Utils;

namespace Barcoder
{
    public static class Code93
    {
        public static IBarcode Encode(string content, bool includeChecksum, bool fullAsciiMode)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (fullAsciiMode)
                content = Prepare(content);
            else if (content.Contains("*"))
                throw new InvalidOperationException("Invalid data! Try full ASCII mode");

            var data = content;
            if (includeChecksum)
            {
                data += GetChecksum(content, 20);
                data += GetChecksum(data, 15);
            }

            data = "*" + data + "*";

            var result = new BitList();
            foreach (var r in data)
            {
                if (!Code93Constants.EncodingTable.TryGetValue(r, out (int value, uint data) info))
                    throw new InvalidOperationException("Invalid data");
                result.AddBits(info.data, 9);
            }
            result.AddBit(true);

            return new Base1DCode(result, Constants.TypeCode93, content, Code93Constants.Margin);
        }

        private static string Prepare(string content)
        {
            var result = new StringBuilder();
            foreach (var r in content)
            {
                if (r > 127)
                    throw new InvalidOperationException("Only ASCII strings can be encoded");
                result.Append(Code93Constants.ExtendedTable[r]);
            }
            return result.ToString();
        }

        private static char GetChecksum(string content, int maxWeight)
        {
            var weight = 1;
            var total = 0;

            var data = content.ToCharArray();
            for (var i = data.Length - 1; i >= 0; i--)
            {
                char r = data[i];
                if (!Code93Constants.EncodingTable.TryGetValue(r, out (int value, uint data) info))
                    return ' ';
                total += info.value * weight;
                if (++weight > maxWeight)
                    weight = 1;
            }
            total = total % 47;
            foreach (KeyValuePair<char, (int value, uint data)> kvp in Code93Constants.EncodingTable)
            {
                if (kvp.Value.value == total)
                    return kvp.Key;
            }
            return ' ';
        }
    }
}
