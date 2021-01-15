using Barcoder.RoyalMail;

namespace Barcoder.Kix
{
    public static class KixEncoder
    {
        public static IBarcode Encode(string content)
        {
            var royalMailCodeWithoutHeaders = RoyalMailFourStateCodeEncoder.Encode(content, false) as RoyalMailFourStateCode;
            return new KixCode(royalMailCodeWithoutHeaders);
        }
    }
}
