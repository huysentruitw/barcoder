using System;
using System.Linq;
using Barcoder.Pdf417;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Pdf417
{
    public class Pdf417EncoderTests
    {
        [Fact]
        public void Encode_Barcoder_ShouldEncodePdf417CodeCorrectly()
        {
            // Arrange
            var content = "Barcoder";
            var expectedDataBits = ImageStringToBools(@"
                ++++++++.+.+.+...+++.+.+.+++......+++.+.+...+++....+++.+..++...++++.++++.+.++..+++++.+++++.+.+.+++++..+++++++.+...+.+..+
                ++++++++.+.+.+...+++.+.+.+++......+++.+.+...+++....+++.+..++...++++.++++.+.++..+++++.+++++.+.+.+++++..+++++++.+...+.+..+
                ++++++++.+.+.+...++++++.+.+...+++.++++.+..+....+...+++.+..+++..++...+..++.+++++..+...++++++.+.+.+++...+++++++.+...+.+..+
                ++++++++.+.+.+...++++++.+.+...+++.++++.+..+....+...+++.+..+++..++...+..++.+++++..+...++++++.+.+.+++...+++++++.+...+.+..+
                ++++++++.+.+.+...+.+.+..++++......++...+++++..+..+.+...++..+....+++.+....++..++.++++.+++.+.+...++++++.+++++++.+...+.+..+
                ++++++++.+.+.+...+.+.+..++++......++...+++++..+..+.+...++..+....+++.+....++..++.++++.+++.+.+...++++++.+++++++.+...+.+..+
                ++++++++.+.+.+...++.+.++++.+++++..+.++....+++...++.+++.+..+..+++....++....++....+..+.++.+.++++..+++++.+++++++.+...+.+..+
                ++++++++.+.+.+...++.+.++++.+++++..+.++....+++...++.+++.+..+..+++....++....++....+..+.++.+.++++..+++++.+++++++.+...+.+..+
                ++++++++.+.+.+...+++.+.+++....++..+++..+.++..+.....+++.+...++.....+.+.++++++..+.++...+++.+.+++..++....+++++++.+...+.+..+
                ++++++++.+.+.+...+++.+.+++....++..+++..+.++..+.....+++.+...++.....+.+.++++++..+.++...+++.+.+++..++....+++++++.+...+.+..+
            ");

            // Act
            IBarcode pdf417 = Pdf417Encoder.Encode(content, 2);

            // Assert
            pdf417.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(pdf417.Bounds.X * pdf417.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % pdf417.Bounds.X;
                int y = i / pdf417.Bounds.X;
                pdf417.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
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
