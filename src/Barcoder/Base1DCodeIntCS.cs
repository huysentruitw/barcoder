using Barcoder.Utils;

namespace Barcoder
{
    public class Base1DCodeIntCS : Base1DCode, IBarcodeIntCS
    {
        internal Base1DCodeIntCS(BitList bitList, string kind, string content, int checkSum)
            : base(bitList, kind, content)
        {
            CheckSum = checkSum;
        }

        public int CheckSum { get; }
    }
}
