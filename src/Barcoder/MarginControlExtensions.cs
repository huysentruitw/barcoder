using System;

namespace Barcoder
{
    internal class BarcodeWithMargin : IBarcode
    {
        private readonly IBarcode _inner;

        public BarcodeWithMargin(IBarcode inner, int margin)
        {
            if (margin < 0)
                throw new ArgumentOutOfRangeException(nameof(margin), "Margin must be greater than or equal to zero");
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            if (_inner is BarcodeWithMargin nested)
                _inner = nested._inner;
            Margin = margin;
        }

        public string Content => _inner.Content;
        public Bounds Bounds => _inner.Bounds;
        public int Margin { get; private set; }
        public Metadata Metadata => _inner.Metadata;
        public bool At(int x, int y) => _inner.At(x, y);
    }

    public static class MarginControlExtensions
    {
        public static IBarcode WithoutMargin(this IBarcode barcode)
            => new BarcodeWithMargin(barcode, 0);

        public static IBarcode WithMargin(this IBarcode barcode, int margin)
            => new BarcodeWithMargin(barcode, margin);
    }
}
