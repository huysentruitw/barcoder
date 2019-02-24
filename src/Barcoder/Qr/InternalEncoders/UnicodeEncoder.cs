using System;
using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal sealed class UnicodeEncoder : InternalEncoderBase
    {
        public override (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            throw new NotImplementedException();
        }
    }
}
