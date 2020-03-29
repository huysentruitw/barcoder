using Barcoder.Utils;

namespace Barcoder.Pdf417
{
    public sealed class Pdf417Code : IBarcode
    {
        private readonly int _width;
        private readonly BitList _data;

        internal Pdf417Code(string content, BitList data, int width)
        {
            Content = content;
            _data = data;
            _width = width;
            int height = data.Length / width;
            Bounds = new Bounds(width, height * Dimensions.ModuleHeight);
            Metadata = new Metadata(BarcodeType.PDF417, 2);
        }

        public string Content { get; }

        public Bounds Bounds { get; }

        public int Margin => 5;

        public Metadata Metadata { get; }

        public bool At(int x, int y)
            => _data.GetBit((y / Dimensions.ModuleHeight) * _width + x);
    }
}
