using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Barcoder.Renderers;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using ImageSharp = SixLabors.ImageSharp;

namespace Barcoder.Renderer.Image
{
    public sealed class ImageRenderer : IRenderer
    {
        private readonly IImageEncoder _imageEncoder;
        private readonly int _pixelSize;
        private readonly int _barHeightFor1DBarcode;

        public ImageRenderer(
            int pixelSize = 10,
            int barHeightFor1DBarcode = 40,
            ImageFormat imageFormat = ImageFormat.Png,
            int jpegQuality = 75)
        {
            if (pixelSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelSize), "Value must be larger than zero");
            if (barHeightFor1DBarcode <= 0)
                throw new ArgumentOutOfRangeException(nameof(barHeightFor1DBarcode), "Value must be larger than zero");
            if (jpegQuality < 0 || jpegQuality > 100)
                throw new ArgumentOutOfRangeException(nameof(jpegQuality), "Value must be a value between 0 and 100");
            _pixelSize = pixelSize;
            _barHeightFor1DBarcode = barHeightFor1DBarcode;
            _imageEncoder = GetImageEncoder(imageFormat, jpegQuality);
        }

        private static IImageEncoder GetImageEncoder(ImageFormat imageFormat, int jpegQuality)
        {
            switch (imageFormat)
            {
            case ImageFormat.Bmp:
                return new BmpEncoder();
            case ImageFormat.Gif:
                return new GifEncoder();
            case ImageFormat.Jpeg:
                return new JpegEncoder { Quality = jpegQuality };
            case ImageFormat.Png:
                return new PngEncoder();
            default:
                throw new NotSupportedException($"Requested image format {imageFormat} is not supported");
            }
        }

        public void Render(IBarcode barcode, Stream outputStream)
        {
            barcode = barcode ?? throw new ArgumentNullException(nameof(barcode));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
            if (barcode.Bounds.Y == 1)
            {
                if (barcode.Metadata.CodeKind == BarcodeType.EAN13)
                {
                    Render1DEan13(barcode, outputStream);
                }
                else
                {
                    Render1D(barcode, outputStream);
                }
            }
            else if (barcode.Bounds.Y > 1)
                Render2D(barcode, outputStream);
            else
                throw new NotSupportedException($"Y value of {barcode.Bounds.Y} is invalid");
        }

        private void Render1D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _pixelSize;
            int height = (_barHeightFor1DBarcode + 2 * barcode.Margin) * _pixelSize;

            using (var image = new ImageSharp.Image<Gray8>(width, height))
            {
                image.Mutate(ctx =>
                {
                    var black = new Gray8(0);
                    ctx.Fill(new Gray8(255));
                    for (var x = 0; x < barcode.Bounds.X; x++)
                    {
                        if (!barcode.At(x, 0))
                            continue;
                        ctx.FillPolygon(
                            black,
                            new Vector2((barcode.Margin + x) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, (_barHeightFor1DBarcode + barcode.Margin) * _pixelSize),
                            new Vector2((barcode.Margin + x) * _pixelSize, (_barHeightFor1DBarcode + barcode.Margin) * _pixelSize));
                    }
                });

                image.Save(outputStream, _imageEncoder);
            }
        }

        private void Render1DEan13(IBarcode barcode, Stream outputStream)
        {
            int contentMargin = 10;
            int eanHeightFor1DBarcode = _barHeightFor1DBarcode + contentMargin;
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _pixelSize;
            int height = (eanHeightFor1DBarcode + 2 * barcode.Margin) * _pixelSize;

            using (var image = new ImageSharp.Image<Gray8>(width, height))
            {
                //Available bars
                //4, 5, 6, 8, 9, 13, 16, 18, 19, 22, 23, 26, 29, 30, 32, 37, 39, 43, 44, 46, 48, 50, 53, 54, 55, 57, 59, 64, 68, 71, 74, 78, 79, 80, 82, 85, 89, 92, 94
                var longerBars = new int[] { 0, 2, 46, 48, 92, 94 };
                image.Mutate(ctx =>
                {
                    var black = new Gray8(0);
                    ctx.Fill(new Gray8(255));
                    for (var x = 0; x < barcode.Bounds.X; x++)
                    {
                        if (!barcode.At(x, 0))
                            continue;

                        if (!longerBars.Contains(x))
                        {
                            ctx.FillPolygon(
                            black,
                            new Vector2((barcode.Margin + x) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, (eanHeightFor1DBarcode - 10 + barcode.Margin) * _pixelSize),
                            new Vector2((barcode.Margin + x) * _pixelSize, (eanHeightFor1DBarcode - 10 + barcode.Margin) * _pixelSize));
                        }
                        else
                        {
                            ctx.FillPolygon(
                            black,
                            new Vector2((barcode.Margin + x) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, barcode.Margin * _pixelSize),
                            new Vector2((barcode.Margin + x + 1) * _pixelSize, (eanHeightFor1DBarcode + barcode.Margin) * _pixelSize),
                            new Vector2((barcode.Margin + x) * _pixelSize, (eanHeightFor1DBarcode + barcode.Margin) * _pixelSize));
                        }

                    }

                    FontCollection collection = new FontCollection();
                    var assembly = Assembly.GetExecutingAssembly();
                    var stream = assembly.GetManifestResourceStream(GetType(), "arial.ttf");
                    FontFamily family = collection.Install(stream);
                    float fontSize = 8;
                    Font font = family.CreateFont(fontSize * _pixelSize, FontStyle.Italic);
                    float y = (eanHeightFor1DBarcode + (contentMargin / 4)) * _pixelSize;
                    string text1 = barcode.Content.Substring(0, 1);
                    string text2 = barcode.Content.Substring(1, 6);
                    string text3 = barcode.Content.Substring(7);
                    image.Mutate(x => x.DrawText(text1, font, black, new PointF(5 * _pixelSize, y)));
                    image.Mutate(x => x.DrawText(text2, font, black, new PointF(22 * _pixelSize, y)));
                    image.Mutate(x => x.DrawText(text3, font, black, new PointF(67 * _pixelSize, y)));

                });
                image.Save(outputStream, _imageEncoder);
            }
        }

        private void Render2D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _pixelSize;
            int height = (barcode.Bounds.Y + 2 * barcode.Margin) * _pixelSize;

            using (var image = new ImageSharp.Image<Gray8>(width, height))
            {
                image.Mutate(ctx =>
                {
                    var black = new Gray8(0);
                    ctx.Fill(new Gray8(255));
                    for (var y = 0; y < barcode.Bounds.Y; y++)
                    {
                        for (var x = 0; x < barcode.Bounds.X; x++)
                        {
                            if (!barcode.At(x, y))
                                continue;
                            ctx.FillPolygon(
                                black,
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
