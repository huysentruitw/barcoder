using Barcoder.Utils;

namespace Barcoder.DataMatrix
{
    public sealed class DataMatrixCode : IBarcode
    {
        private readonly CodeSize _size;
        private readonly BitList _data;

        internal DataMatrixCode(CodeSize size)
        {
            _size = size;
            Bounds = new Bounds(size.Columns, size.Rows);
            Metadata = new Metadata(BarcodeType.DataMatrix, 2);
            _data = new BitList(size.Rows * size.Columns);
        }

        internal void Set(int x, int y, bool value)
            => _data.SetBit(x * _size.Rows + y, value);

        internal bool Get(int x, int y)
            => _data.GetBit(x * _size.Rows + y);

        public string Content { get; internal set; }

        public Bounds Bounds { get; }

        public int Margin => 5;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => Get(x, y);
    }
}
