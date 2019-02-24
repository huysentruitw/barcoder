using System;
using System.Collections.Generic;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal sealed class AlphaNumericEncoder : InternalEncoderBase
    {
        private const string CharSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        public override (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            bool contentLengthIsOdd = content.Length % 2 == 1;
            int contentBitCount = (content.Length / 2) * 11;
            if (contentLengthIsOdd)
                contentBitCount += 6;

            EncodingMode encodingMode = EncodingMode.AlphaNumeric;
            var versionInfo = VersionInfo.FindSmallestVersionInfo(errorCorrectionLevel, encodingMode, contentBitCount);
            if (versionInfo == null)
                throw new InvalidOperationException("Too much data to encode");

            var bits = new BitList();
            bits.AddBits((uint)encodingMode, 4);
            bits.AddBits((uint)content.Length, versionInfo.CharCountBits(encodingMode));

            var encoder = new Queue<int>(content.Select(x => CharSet.IndexOf(x)));
            for (int i = 0; i < content.Length / 2; i++)
            {
                int c1 = encoder.Dequeue();
                int c2 = encoder.Dequeue();
                if (c1 < 0 || c2 < 0)
                    throw new InvalidOperationException($"{content} can not be ancoded as {encodingMode}");
                bits.AddBits((uint)(c1 * 45 + c2), 11);
            }

            if (contentLengthIsOdd)
            {
                int c = encoder.Dequeue();
                if (c < 0)
                    throw new InvalidOperationException($"{content} can not be ancoded as {encodingMode}");
                bits.AddBits((uint)c, 6);
            }

            AddPaddingAndTerminator(ref bits, versionInfo);
            return (bits, versionInfo);
        }
    }
}
