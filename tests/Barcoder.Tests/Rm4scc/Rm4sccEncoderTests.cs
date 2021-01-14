using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barcoder.Rm4scc;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Rm4scc
{
    public sealed class Rm4sccEncoderTests
    {
        [Fact]
        public void Encode_Barcoder_ShouldEncodeRm4sccCorrectly()
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
            var kixBits = ImageStringToBools(@"
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
            IBarcode encoder = Rm4sccEncoder.Encode(content, encodeAsKix: false);

            // Assert
            encoder.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(encoder.Bounds.X * encoder.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % encoder.Bounds.X;
                int y = i / encoder.Bounds.X;
                encoder.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_Barcoder_ShouldEncodeKixCorrectly()
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
            IBarcode encoder = Rm4sccEncoder.Encode(content, encodeAsKix: true);

            // Assert
            encoder.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(encoder.Bounds.X * encoder.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % encoder.Bounds.X;
                int y = i / encoder.Bounds.X;
                encoder.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
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
