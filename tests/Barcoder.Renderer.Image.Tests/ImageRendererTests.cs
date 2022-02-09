using System;
using System.IO;
using Barcoder.Code128;
using Barcoder.Ean;
using Barcoder.Qr;
using Barcoder.Renderers;
using FluentAssertions;
using Moq;
using SixLabors.ImageSharp.Formats;
using Xunit;
using ImageSharp = SixLabors.ImageSharp;

namespace Barcoder.Renderer.Image.Tests
{
    public sealed class ImageRendererTests
    {
        [Fact]
        public void Render_PassNullAsBarcode_ShouldThrowException()
        {
            // Arrange
            var renderer = new ImageRenderer();
            var stream = new MemoryStream();

            // Act
            Action action = () => renderer.Render(null, stream);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("barcode");
        }

        [Fact]
        public void Render_PassNullAsOutputStream_ShouldThrowException()
        {
            // Arrange
            var renderer = new ImageRenderer();
            var barcodeMock = new Mock<IBarcode>();

            // Act
            Action action = () => renderer.Render(barcodeMock.Object, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("outputStream");
        }

        [Fact]
        public void Render_Barcode1D()
        {
            // Arrange
            var renderer = new ImageRenderer();
            IBarcode barcode = Code128Encoder.Encode("Wikipedia");

            // Act
            byte[] data = RenderBarcodeToByteArray(renderer, barcode);

            // Assert
            data.Should().NotBeNull();
        }

        [Fact]
        public void Render_Barcode2D()
        {
            // Arrange
            var renderer = new ImageRenderer();
            IBarcode barcode = QrEncoder.Encode("Hello Unicode\nHave a nice day!", ErrorCorrectionLevel.L, Encoding.Unicode);

            // Act
            byte[] data = RenderBarcodeToByteArray(renderer, barcode);

            // Assert
            data.Should().NotBeNull();
        }

        [Fact]
        public void Render_ImageFormatBmp_ShouldRenderBmp()
        {
            // Arrange
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Bmp);
            IBarcode barcode = QrEncoder.Encode("Hello", ErrorCorrectionLevel.L, Encoding.Unicode);
            using var stream = new MemoryStream();

            // Act
            renderer.Render(barcode, stream);

            // Assert
            stream.Position = 0;
            using var image = ImageSharp.Image.Load(stream, out IImageFormat imageFormat);
            imageFormat.Name.Should().Be("BMP");
        }

        [Fact]
        public void Render_ImageFormatGif_ShouldRenderGif()
        {
            // Arrange
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Gif);
            IBarcode barcode = QrEncoder.Encode("Hello", ErrorCorrectionLevel.L, Encoding.Unicode);
            using var stream = new MemoryStream();

            // Act
            renderer.Render(barcode, stream);

            // Assert
            stream.Position = 0;
            using var image = ImageSharp.Image.Load(stream, out IImageFormat imageFormat);
            imageFormat.Name.Should().Be("GIF");
        }

        [Fact]
        public void Render_ImageFormatJpeg_ShouldRenderJpeg()
        {
            // Arrange
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Jpeg);
            IBarcode barcode = QrEncoder.Encode("Hello", ErrorCorrectionLevel.L, Encoding.Unicode);
            using var stream = new MemoryStream();

            // Act
            renderer.Render(barcode, stream);

            // Assert
            stream.Position = 0;
            using var image = ImageSharp.Image.Load(stream, out IImageFormat imageFormat);
            imageFormat.Name.Should().Be("JPEG");
        }

        [Fact]
        public void Render_ImageFormatPng_ShouldRenderPng()
        {
            // Arrange
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Png);
            IBarcode barcode = QrEncoder.Encode("Hello", ErrorCorrectionLevel.L, Encoding.Unicode);
            using var stream = new MemoryStream();

            // Act
            renderer.Render(barcode, stream);

            // Assert
            stream.Position = 0;
            using var image = ImageSharp.Image.Load(stream, out IImageFormat imageFormat);
            imageFormat.Name.Should().Be("PNG");
        }

        private static byte[] RenderBarcodeToByteArray(IRenderer renderer, IBarcode barcode)
        {
            using var stream = new MemoryStream();
            renderer.Render(barcode, stream);
            return stream.ToArray();
        }

        [Fact(Skip = "Integration test")]
        public void Render_Ean8_IncludeContentAsText()
        {
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Png, includeEanContentAsText: true);
            IBarcode barcode = EanEncoder.Encode("1234567");
            using Stream stream = File.OpenWrite(@"d:\temp\ean8-test.png");
            renderer.Render(barcode, stream);
        }

        [Fact(Skip = "Integration test")]
        public void Render_Ean13_IncludeContentAsText()
        {
            var renderer = new ImageRenderer(imageFormat: ImageFormat.Png, includeEanContentAsText: true);
            IBarcode barcode = EanEncoder.Encode("978020137962");
            using Stream stream = File.OpenWrite(@"d:\temp\ean13-test.png");
            renderer.Render(barcode, stream);
        }
    }
}
