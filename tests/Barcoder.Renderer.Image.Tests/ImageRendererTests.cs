using System;
using System.IO;
using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.Renderers;
using FluentAssertions;
using Moq;
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
            var data = RenderBarcodeToByteArray(renderer, barcode);

            // Assert
            data.Should().NotBeNull();
        }

        private static byte[] RenderBarcodeToByteArray(IRenderer renderer, IBarcode barcode)
        {
            using (var stream = new MemoryStream())
            {
                renderer.Render(barcode, stream);
                return stream.ToArray();
            }
        }
    }
}
