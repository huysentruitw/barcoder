using System;
using System.IO;
using System.Numerics;
using Barcoder.Renderer.Image.Internal;
using Barcoder.Renderers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Barcoder.Renderer.Image
{
    public sealed class ImageRenderer : IRenderer
    {
        private readonly IImageEncoder _imageEncoder;
        private readonly int _pixelSize;
        private readonly int _barHeightFor1DBarcode;
        private readonly bool _includeEanContentAsText;
        private readonly string _eanFontFamily;

        public ImageRenderer(
            int pixelSize = 10,
            int barHeightFor1DBarcode = 40,
            ImageFormat imageFormat = ImageFormat.Png,
            int jpegQuality = 75,
            bool includeEanContentAsText = false,
            string eanFontFamily = null)
        {
            if (pixelSize <= 0) throw new ArgumentOutOfRangeException(nameof(pixelSize), "Value must be larger than zero");
            if (barHeightFor1DBarcode <= 0) throw new ArgumentOutOfRangeException(nameof(barHeightFor1DBarcode), "Value must be larger than zero");
            if (jpegQuality < 0 || jpegQuality > 100) throw new ArgumentOutOfRangeException(nameof(jpegQuality), "Value must be a value between 0 and 100");

            _pixelSize = pixelSize;
            _barHeightFor1DBarcode = barHeightFor1DBarcode;
            _imageEncoder = GetImageEncoder(imageFormat, jpegQuality);
            _includeEanContentAsText = includeEanContentAsText;
            _eanFontFamily = eanFontFamily ?? "Arial";
        }

        private static IImageEncoder GetImageEncoder(ImageFormat imageFormat, int jpegQuality)
        {
            switch (imageFormat)
            {
            case ImageFormat.Bmp: return new BmpEncoder();
            case ImageFormat.Gif: return new GifEncoder();
            case ImageFormat.Jpeg: return new JpegEncoder { Quality = jpegQuality };
            case ImageFormat.Png: return new PngEncoder();
            default:
                throw new NotSupportedException($"Requested image format {imageFormat} is not supported");
            }
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

        private void Render1D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _pixelSize;
            int height = (_barHeightFor1DBarcode + 2 * barcode.Margin) * _pixelSize;

            using (var image = new Image<L8>(width, height))
            {
                image.Mutate(ctx =>
                {
                    ctx.Fill(Color.White);
                    for (var x = 0; x < barcode.Bounds.X; x++)
                    {
                        if (!barcode.At(x, 0))
                            continue;
                        ctx.FillPolygon(
                            Color.Black,
                            new Vector2((barcode.Margin + x) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, (_barHeightFor1DBarcode + barcode.Margin) * _pixelSize),
                            new Vector2((barcode.Margin + x) * _pixelSize, (_barHeightFor1DBarcode + barcode.Margin) * _pixelSize));
                    }
                });

                if (_includeEanContentAsText && barcode.IsEanBarcode())
                    EanContentRenderer.Render(image, barcode, fontFamily: _eanFontFamily, scale: _pixelSize);

                image.Save(outputStream, _imageEncoder);
            }
        }

        private void Render2D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _pixelSize;
            int height = (barcode.Bounds.Y + 2 * barcode.Margin) * _pixelSize;

            using (var image = new Image<L8>(width, height))
            {
                image.Mutate(ctx =>
                {
                    ctx.Fill(Color.White);
                    for (var y = 0; y < barcode.Bounds.Y; y++)
                    {
                        for (var x = 0; x < barcode.Bounds.X; x++)
                        {
                            if (!barcode.At(x, y)) continue;
                            ctx.FillPolygon(
                                Color.Black,
                                new Vector2((barcode.Margin + x) * _pixelSize, (barcode.Margin + y) * _pixelSize),
                                new Vector2((barcode.Margin + x + 1) * _pixelSize, (barcode.Margin + y) * _pixelSize),
                                new Vector2((barcode.Margin + x + 1) * _pixelSize, (barcode.Margin + y + 1) * _pixelSize),
                                new Vector2((barcode.Margin + x) * _pixelSize, (barcode.Margin + y + 1) * _pixelSize));
                        }
                    }
                });

                image.Save(outputStream, _imageEncoder);
            }
        }
    }
}
