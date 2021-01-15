using Barcoder.Utils;

namespace Barcoder.RoyalMail
{
    public sealed class RoyalMailFourStateCode : IBarcode
    {
        private readonly BitList _data;
        private readonly int _width;

        internal RoyalMailFourStateCode(string content, BitList data, int width)
        {
            Content = content;
            _data = data;
            _width = width;
            Bounds = new Bounds(width, Constants.BARCODE_HEIGHT);
            Metadata = new Metadata(BarcodeType.RM4SC, 2);
        }

        public string Content { get; }

        public Bounds Bounds { get; }

        public int Margin => 8;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => _data.GetBit(y * _width + x);
    }
}
