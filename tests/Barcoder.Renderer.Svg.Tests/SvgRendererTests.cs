using System.IO;
using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.Renderers;
using FluentAssertions;
using Xunit;

namespace Barcoder.Renderer.Svg.Tests
{
    public class SvgRendererTests
    {
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
    }
}
