using System;
using System.Linq;
using Barcoder.RoyalMail;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.RoyalMail
{
    public sealed class RoyalMailFourStateCodeEncoderTests
    {
        [Fact]
        public void Encode_Barcode_ShouldEncodeRm4ScCorrectly()
        {
            // Arrange
            var content = "123ABC789XYZ"; // checksum character == 'K'
            var expectedDataBits = ImageStringToBools(@"
                +.....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+.....+.....+.+
                +.....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+.....+.....+.+
                +.....+.+.....+.+.....+.+...+...+...+...+...+.+.....+...+...+...+...+...+.+.+.....+.+.....+.+.....+.....+.+
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                ....+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+.......+.+...+
                ....+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+.......+.+...+
                ....+...+...+.+...+.....+.+...+...+.+.........+.+...+...+...+.+...+.....+.+.....+.+...+...+.+.......+.+...+
            ");

            // Act
            IBarcode royalMailCode = RoyalMailFourStateCodeEncoder.Encode(content);

            // Assert
            royalMailCode.Should().NotBeNull();
            royalMailCode.Metadata.CodeKind.Should().Be(BarcodeType.RM4SC);
            expectedDataBits.Length.Should().Be(royalMailCode.Bounds.X * royalMailCode.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % royalMailCode.Bounds.X;
                int y = i / royalMailCode.Bounds.X;
                royalMailCode.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
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
