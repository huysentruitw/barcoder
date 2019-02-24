using System;
using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal sealed class AutoEncoder : InternalEncoderBase
    {
        public override (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            try
            {
                return new NumericEncoder().Encode(content, errorCorrectionLevel);
            }
            catch
            {
            }

            try
            {
                return new AlphaNumericEncoder().Encode(content, errorCorrectionLevel);
            }
            catch
            {
            }

            try
            {
                return new UnicodeEncoder().Encode(content, errorCorrectionLevel);
            }
            catch
            {
            }

            throw new InvalidOperationException($"No encoding found to encode {content}");
        }
    }
}
