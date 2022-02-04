using System;
using System.IO;
using System.Numerics;
using Barcoder.Renderer.Image.Internal;
using Barcoder.Renderers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Barcoder.Renderer.Image
{
    public sealed class ImageRenderer<TPixel> : IRenderer where TPixel : struct, IPixel<TPixel>
    {
        private readonly ImageRendererSettings<TPixel> _settings;
        private readonly IImageEncoder _imageEncoder;
        
        public ImageRenderer(ImageRendererSettings<TPixel> settings)
        {
            if (settings.PixelSize <= 0) throw new ArgumentOutOfRangeException(nameof(settings.PixelSize), "Value must be larger than zero");
            if (settings.BarHeightFor1DBarcode <= 0) throw new ArgumentOutOfRangeException(nameof(settings.BarHeightFor1DBarcode), "Value must be larger than zero");
            if (settings.JpegQuality < 0 || settings.JpegQuality > 100) throw new ArgumentOutOfRangeException(nameof(settings.JpegQuality), "Value must be a value between 0 and 100");

            _settings = settings;
            _imageEncoder = GetImageEncoder(settings.ImageFormat, settings.JpegQuality);
        }
        
        public ImageRenderer(
            int pixelSize = 10,
            int barHeightFor1DBarcode = 40,
            ImageFormat imageFormat = ImageFormat.Png,
            int jpegQuality = 75,
            bool includeEanContentAsText = false,
            string eanFontFamily = null): this(new ImageRendererSettings<TPixel>()
        {
            PixelSize = pixelSize,
            BarHeightFor1DBarcode = barHeightFor1DBarcode,
            ImageFormat = imageFormat,
            JpegQuality = jpegQuality,
            IncludeEanContentAsText = includeEanContentAsText,
            EanFontFamily = eanFontFamily ?? "Arial"
        })
        {
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
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _settings.PixelSize;
            int height = (_settings.BarHeightFor1DBarcode + 2 * barcode.Margin) * _settings.PixelSize;

            using (var image = new Image<TPixel>(width, height))
            {
                image.Mutate(ctx =>
                {
                    ctx.Fill(_settings.BackgroundColor);
                    for (var x = 0; x < barcode.Bounds.X; x++)
                    {
                        if (!barcode.At(x, 0))
                            continue;
                        ctx.FillPolygon(
                            _settings.ForegroundColor,
                            new Vector2((barcode.Margin + x) * _settings.PixelSize, barcode.Margin * _settings.PixelSize),
                            new Vector2((barcode.Margin + x + 1) * _settings.PixelSize, barcode.Margin * _settings.PixelSize),
                            new Vector2((barcode.Margin + x + 1) * _settings.PixelSize, (_settings.BarHeightFor1DBarcode + barcode.Margin) * _settings.PixelSize),
                            new Vector2((barcode.Margin + x) * _settings.PixelSize, (_settings.BarHeightFor1DBarcode + barcode.Margin) * _settings.PixelSize));
                    }
                });

                if (_settings.IncludeEanContentAsText && barcode.IsEanBarcode())
                    EanContentRenderer<TPixel>.Render(image, barcode, fontFamily: _settings.EanFontFamily, scale: _settings.PixelSize);

                image.Save(outputStream, _imageEncoder);
            }
        }

        private void Render2D(IBarcode barcode, Stream outputStream)
        {
            int width = (barcode.Bounds.X + 2 * barcode.Margin) * _settings.PixelSize;
            int height = (barcode.Bounds.Y + 2 * barcode.Margin) * _settings.PixelSize;

            using (var image = new Image<TPixel>(width, height))
            {
                image.Mutate(ctx =>
                {
                    ctx.Fill(_settings.BackgroundColor);
                    for (var y = 0; y < barcode.Bounds.Y; y++)
                    {
                        for (var x = 0; x < barcode.Bounds.X; x++)
                        {
                            if (!barcode.At(x, y)) continue;
                            ctx.FillPolygon(
                                _settings.ForegroundColor,
                                new Vector2((barcode.Margin + x) * _settings.PixelSize, (barcode.Margin + y) * _settings.PixelSize),
                                new Vector2((barcode.Margin + x + 1) * _settings.PixelSize, (barcode.Margin + y) * _settings.PixelSize),
                                new Vector2((barcode.Margin + x + 1) * _settings.PixelSize, (barcode.Margin + y + 1) * _settings.PixelSize),
                                new Vector2((barcode.Margin + x) * _settings.PixelSize, (barcode.Margin + y + 1) * _settings.PixelSize));
                        }
                    }
                });

                image.Save(outputStream, _imageEncoder);
            }
        }
    }
}
