using System;
using System.Linq;
using Barcoder.Qr;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr
{
    public sealed class QrEncoderTests
    {
        [Fact]
        public void Encode_InvalidEncoding_ShouldThrowException()
        {
            // Act
            Action action = () => QrEncoder.Encode("test", ErrorCorrectionLevel.H, (Encoding)255);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Unknown encoding type 255");
        }

        [Fact]
        public void Encode_ValidUnicodeContent_ShouldEncodeQrCodeCorrectly()
        {
            // Arrange
            var content = "hello world";
            var expectedDataBits = ImageStringToBools(@"
                +++++++.+.+.+...+.+++++++
                +.....+.++...+++..+.....+
                +.+++.+.+.+.++.++.+.+++.+
                +.+++.+....++.++..+.+++.+
                +.+++.+..+...++.+.+.+++.+
                +.....+.+..+..+++.+.....+
                +++++++.+.+.+.+.+.+++++++
                ........++..+..+.........
                ..+++.+.+++.+.++++++..+++
                +++..+..+...++.+...+..+..
                +...+.++++....++.+..++.++
                ++.+.+.++...+...+.+....++
                ..+..+++.+.+++++.++++++++
                +.+++...+..++..++..+..+..
                +.....+..+.+.....+++++.++
                +.+++.....+...+.+.+++...+
                +.+..+++...++.+.+++++++..
                ........+....++.+...+.+..
                +++++++......++++.+.+.+++
                +.....+....+...++...++.+.
                +.+++.+.+.+...+++++++++..
                +.+++.+.++...++...+.++..+
                +.+++.+.++.+++++..++.+..+
                +.....+..+++..++.+.++...+
                +++++++....+..+.+..+..+++
            ");

            // Act
            IBarcode qr = QrEncoder.Encode(content, ErrorCorrectionLevel.H, Encoding.Unicode);

            // Assert
            qr.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(qr.Bounds.X * qr.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % qr.Bounds.X;
                int y = i / qr.Bounds.X;
                qr.At(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        private static bool[] ImageStringToBools(string imageString)
        {
            var lines = imageString?
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            if (lines.Length <= 0) throw new InvalidOperationException($"No data in {nameof(imageString)}");
            var dimension = lines.First().Length;
            if (lines.Length != dimension) throw new InvalidOperationException("Not a square QR code");
            foreach (var line in lines)
                if (line.Length != dimension)
                    throw new InvalidOperationException("Not all lines have the same length");
            return lines.SelectMany(x => x).Select(x => x != '.').ToArray();
        }
    }
}
