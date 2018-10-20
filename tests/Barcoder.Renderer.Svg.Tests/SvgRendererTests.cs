using System.IO;
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
            IBarcodeIntCS barcode = Code128.Encode("Wikipedia");
            var renderer = new SvgRenderer();

            // Act
            string svg;
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                renderer.Render(barcode, stream);
                stream.Position = 0;
                svg = reader.ReadToEnd();
            }

            // Assert
            svg.Length.Should().BeGreaterOrEqualTo(0);

            string expected;
            using (Stream stream = typeof(SvgRendererTests).Assembly.GetManifestResourceStream("Barcoder.Renderer.Svg.Tests.ExpectedSvgOutput.txt"))
            using (var reader = new StreamReader(stream))
                expected = reader.ReadToEnd();

            svg.Should().Be(expected);
        }
    }
}
