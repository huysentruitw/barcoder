using Barcoder.Utils;

namespace Barcoder.Aztec
{
    public sealed class AztecCode : IBarcode
    {
        private readonly BitList _data;

        public AztecCode(int dimension)
        {
            Dimension = dimension;
            Bounds = new Bounds(Dimension, Dimension);
            Metadata = new Metadata(BarcodeType.Aztec, 2);
            _data = new BitList(Dimension * Dimension);
        }

        internal void Set(int x, int y)
            => _data.SetBit(x * Dimension + y, true);

        internal bool Get(int x, int y)
            => _data.GetBit(x * Dimension + y);

        public int Dimension { get; }

        public string Content { get; internal set; }

        public Bounds Bounds { get; }

        public int Margin => 5;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => Get(x, y);
    }
}
