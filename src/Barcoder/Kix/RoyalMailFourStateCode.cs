namespace Barcoder.RoyalMail
{
    public sealed class KixCode : IBarcode
    {
        private readonly RoyalMailFourStateCode _royalMailCodeWithoutHeaders;

        internal KixCode(RoyalMailFourStateCode royalMailCodeWithoutHeaders)
        {
            _royalMailCodeWithoutHeaders = royalMailCodeWithoutHeaders;
            Metadata = new Metadata(BarcodeType.KixCode, _royalMailCodeWithoutHeaders.Metadata.Dimensions);
        }

        public string Content => _royalMailCodeWithoutHeaders.Content;

        public Bounds Bounds => _royalMailCodeWithoutHeaders.Bounds;

        public int Margin => _royalMailCodeWithoutHeaders.Margin;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => _royalMailCodeWithoutHeaders.At(x, y);
    }
}
