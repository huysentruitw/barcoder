using Barcoder.Utils;

namespace Barcoder.Rm4scc
{
    public sealed class Rm4sccCode : IBarcode
    {
        private readonly BitList _data;

        private readonly int _width;

        private const int HEIGHT = 8;

        internal Rm4sccCode(BitList data, int width, bool isKixCode)
        {
            Metadata = new Metadata(isKixCode ? BarcodeType.KixCode : BarcodeType.RM4SCC, 2);
            _data = data;
            _width = width;
            Bounds = new Bounds(width, HEIGHT);
        }

        public string Content { get; internal set; }

        public Bounds Bounds { get; }

        public int Margin => 8;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => _data.GetBit(y * _width + x);
    }
}
