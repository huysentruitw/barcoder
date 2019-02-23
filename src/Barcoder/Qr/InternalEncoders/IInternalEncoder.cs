using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal interface IInternalEncoder
    {
        (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel);
    }
}
