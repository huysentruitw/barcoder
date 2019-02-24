using Barcoder.Utils;

namespace Barcoder.Qr.InternalEncoders
{
    internal abstract class InternalEncoderBase
    {
        public abstract (BitList, VersionInfo) Encode(string content, ErrorCorrectionLevel errorCorrectionLevel);

        protected void AddPaddingAndTerminator(ref BitList bitList, VersionInfo versionInfo)
        {
            int totalDataBits = versionInfo.TotalDataBytes() * 8;
            for (int i = 0; i < 4 && bitList.Length < totalDataBits; i++)
                bitList.AddBit(false);

            while (bitList.Length % 8 != 0)
                bitList.AddBit(false);

            for (int i = 0; bitList.Length < totalDataBits; i++)
                bitList.AddByte((i % 2) == 0 ? (byte)236 : (byte)17);
        }
    }
}
