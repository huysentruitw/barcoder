using System;
using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal sealed class NumericEncoder : InternalEncoderBase
    {
        public override (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var contentBitCount = (content.Length / 3) * 10;
            switch (content.Length % 3)
            {
            case 1:
                contentBitCount += 4;
                break;
            case 2:
                contentBitCount += 7;
                break;
            }

            EncodingMode encodingMode = EncodingMode.Numeric;
            var versionInfo = VersionInfo.FindSmallestVersionInfo(errorCorrectionLevel, encodingMode, contentBitCount);
            if (versionInfo == null)
                throw new InvalidOperationException("Too much data to encode");

            var bits = new BitList();
            bits.AddBits((uint)encodingMode, 4);
            bits.AddBits((uint)content.Length, versionInfo.CharCountBits(encodingMode));

            for (int i = 0; i < content.Length; i += 3)
            {
                string currentContentPart = i + 3 <= content.Length ? content.Substring(i, 3) : content.Substring(i);
                if (!uint.TryParse(currentContentPart, out uint currentNumericalValue))
                    throw new InvalidOperationException($"{content} can not be ancoded as {encodingMode}");
                byte bitCount;
                switch (currentContentPart.Length % 3)
                {
                case 0:
                    bitCount = 10;
                    break;
                case 1:
                    bitCount = 4;
                    break;
                case 2:
                    bitCount = 7;
                    break;
                default:
                    throw new InvalidOperationException();
                }

                bits.AddBits(currentNumericalValue, bitCount);
            }

            AddPaddingAndTerminator(ref bits, versionInfo);
            return (bits, versionInfo);
        }
    }
}
