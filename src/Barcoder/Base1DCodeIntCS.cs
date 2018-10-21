using Barcoder.Utils;

namespace Barcoder
{
    public class Base1DCodeIntCS : Base1DCode, IBarcodeIntCS
    {
        internal Base1DCodeIntCS(BitList bitList, string kind, string content, int checksum, int margin)
            : base(bitList, kind, content, margin)
        {
            Checksum = checksum;
        }

        public int Checksum { get; }
    }
}
