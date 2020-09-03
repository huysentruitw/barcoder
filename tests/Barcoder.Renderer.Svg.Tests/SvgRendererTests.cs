using System;
using System.IO;
using Barcoder.Code128;
using Barcoder.Ean;
using Barcoder.Qr;
using Barcoder.Renderers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Barcoder.Renderer.Svg.Tests
{
    public class SvgRendererTests
    {
        [Fact]
        public void Render_PassNullAsBarcode_ShouldThrowException()
        {
            // Arrange
            var renderer = new SvgRenderer();
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
            var renderer = new SvgRenderer();
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
            var renderer = new SvgRenderer();
            IBarcode barcode = Code128Encoder.Encode("Wikipedia");
            string expected = GetExpectedSvgOutput("Code128.ExpectedSvgOutput.txt");

            // Act
            string svg = RenderBarcodeToString(renderer, barcode);

            // Assert
            svg.Length.Should().BeGreaterOrEqualTo(0);
            svg.Should().Be(expected);
        }

        [Fact]
        public void Render_Barcode2D()
        {
            // Arrange
            var renderer = new SvgRenderer();
            IBarcode barcode = QrEncoder.Encode("Hello Unicode\nHave a nice day!", ErrorCorrectionLevel.L, Encoding.Unicode);
            string expected = GetExpectedSvgOutput("QrCode.ExpectedSvgOutput.txt");

            // Act
            string svg = RenderBarcodeToString(renderer, barcode);

            // Assert
            svg.Length.Should().BeGreaterOrEqualTo(0);
            svg.Should().Be(expected);
        }

        private static string RenderBarcodeToString(IRenderer renderer, IBarcode barcode)
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                renderer.Render(barcode, stream);
                stream.Position = 0;
                return reader.ReadToEnd().Replace("\r", "").Replace("\n", "");
            }
        }

        private static string GetExpectedSvgOutput(string resourceName)
        {
            using (Stream stream = typeof(SvgRendererTests).Assembly.GetManifestResourceStream($"Barcoder.Renderer.Svg.Tests.{resourceName}"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd().Replace("\r", "").Replace("\n", "");
        }

        [Fact(Skip = "Integration test")]
        public void Render_Ean8_IncludeContentAsText()
        {
            var renderer = new SvgRenderer(includeEanContentAsText: true);
            IBarcode barcode = EanEncoder.Encode("1234567");
            using Stream stream = File.OpenWrite(@"d:\temp\ean-test.svg");
            renderer.Render(barcode, stream);
        }

        [Fact(Skip = "Integration test")]
        public void Render_Ean13_IncludeContentAsText()
        {
            var renderer = new SvgRenderer(includeEanContentAsText: true);
            IBarcode barcode = EanEncoder.Encode("978020137962");
            using Stream stream = File.OpenWrite(@"d:\temp\ean-test.svg");
            renderer.Render(barcode, stream);
        }
    }
}
