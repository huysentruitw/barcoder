using System;
using System.Text.RegularExpressions;
using Barcoder.Utils;

namespace Barcoder
{
    public static class Codabar
    {
        public static IBarcode Encode(string content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (!Regex.IsMatch(content, @"^[ABCD][0-9\-\$\:/\.\+]*[ABCD]$"))
                throw new InvalidOperationException($"{content} could not be encoded");

            var resBits = new BitList();
            var i = 0;
            foreach (var r in content)
            {
                if (i++ > 0)
                    resBits.AddBit(false);
                resBits.AddBit(CodabarConstants.EncodingTable[r]);
            }

            return new Base1DCode(resBits, BarcodeType.Codabar, content, CodabarConstants.Margin);
        }
    }
}
