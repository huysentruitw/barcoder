using System;
using System.IO;
using Barcoder.Renderers;

namespace Barcoder.Renderer.Image
{
    public sealed class ImageRenderer : IRenderer
    {
        private readonly int _pixelSize;

        public ImageRenderer(int pixelSize = 10)
        {
            if (pixelSize <= 0) throw new ArgumentOutOfRangeException(nameof(pixelSize), "Value must be larger than zero");
            _pixelSize = pixelSize;
        }

        public void Render(IBarcode barcode, Stream outputStream)
        {
            barcode = barcode ?? throw new ArgumentNullException(nameof(barcode));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
            if (barcode.Bounds.Y == 1)
                Render1D(barcode, outputStream);
            else if (barcode.Bounds.Y > 1)
                Render2D(barcode, outputStream);
            else
                throw new NotSupportedException($"Y value of {barcode.Bounds.Y} is invalid");
        }

        private static void Render1D(IBarcode barcode, Stream outputStream)
        {
            throw new NotImplementedException();
        }

        private static void Render2D(IBarcode barcode, Stream outputStream)
        {
            throw new NotImplementedException();
        }
    }
}
