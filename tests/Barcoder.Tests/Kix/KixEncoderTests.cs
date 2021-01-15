using System;
using System.Linq;
using Barcoder.Kix;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Kix
{
    public sealed class KixEncoderTests
    {
        [Fact]
        public void Encode_Barcode_ShouldEncodeKixCorrectly()
        {
            // Arrange
            var content = "123ABC789XYZ"; // checksum character == 'K'
            var expectedDataBits = ImageStringToBools(@"
                ....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+....
                ....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+....
                ....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+....
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                ..+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+....
                ..+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+....
                ..+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+....
            ");

            // Act
            IBarcode kixCode = KixEncoder.Encode(content);

            // Assert
            kixCode.Should().NotBeNull();
            kixCode.Metadata.CodeKind.Should().Be(BarcodeType.KixCode);
            expectedDataBits.Length.Should().Be(kixCode.Bounds.X * kixCode.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % kixCode.Bounds.X;
                int y = i / kixCode.Bounds.X;
                kixCode.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        private static bool[] ImageStringToBools(string imageString)
        {
            string[] lines = imageString?
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            if ((lines?.Length ?? 0) <= 0)
                throw new InvalidOperationException($"No data in {nameof(imageString)}");
            var dimension = lines.First().Length;
            foreach (var line in lines)
                if (line.Length != dimension)
                    throw new InvalidOperationException("Not all lines have the same length");
            return lines.SelectMany(x => x).Select(x => x != '.').ToArray();
        }
    }
}
