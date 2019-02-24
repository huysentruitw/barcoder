using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal abstract class InternalEncoderBase
    {
        public abstract (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel);

        protected void AddPaddingAndTerminator(ref BitList bits, VersionInfo versionInfo)
        {
            int totalDataBits = versionInfo.TotalDataBytes() * 8;
            for (int i = 0; i < 4 && bits.Length < totalDataBits; i++)
                bits.AddBit(false);

            while (bits.Length % 8 != 0)
                bits.AddBit(false);

            for (int i = 0; bits.Length < totalDataBits; i++)
                bits.AddByte((i % 2) == 0 ? (byte)236 : (byte)17);
        }
    }
}
