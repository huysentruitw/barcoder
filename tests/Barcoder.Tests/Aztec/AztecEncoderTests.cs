using System;
using System.Linq;
using Barcoder.Aztec;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Aztec
{
    public sealed class AztecEncoderTests
    {
        [Fact]
        public void Encode_WikipediaSample_ShouldEncodeAztecCodeCorrectly()
        {
            // Arrange
            var content = "This is an example Aztec symbol for Wikipedia.";
            var expectedDataBits = ImageStringToBools(@"
                +..++...+..++..+..+....
                +....+..++..+.++.++...+
                ++.+++++.+++........+..
                ++........++.+...++++++
                ..+++.+.+..++++....++..
                .+++.++++.+..+.+..++.+.
                ....+++++..++++.+.+..+.
                +...+.+++++++++++..+.++
                +.+..+++.......++++.++.
                +..++.++.+++++.++.+.+++
                +.+....+.+...+.++++...+
                +...+..+.+.+.+.+.++.+..
                ...+.+++.+...+.+..+++..
                ..++++++.+++++.++++++.+
                .++.+.++.......+++.++++
                .+.+...++++++++++++.++.
                .++.+...+++.+++...++...
                .+.......+.++..+..+++..
                .+.+++.++.+.++++.+.++++
                ..+.+.+++.+.+.++++..+..
                ....+.......+........+.
                ....++..+.++.+.+.+...++
                .+.+.++...+.+....+++..+
            ");

            // Act
            var aztec = AztecEncoder.Encode(content) as AztecCode;

            // Assert
            aztec.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(aztec.Dimension * aztec.Dimension);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % aztec.Dimension;
                int y = i / aztec.Dimension;
                aztec.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_ValidContent_ShouldEncodeAztecCodeCorrectly()
        {
            // Arrange
            var content = "Aztec Code is a public domain 2D matrix barcode symbology of nominally square symbols built on a square grid with a distinctive square bullseye pattern at their center.";
            bool[] expectedDataBits = ImageStringToBools(@"
                ....++..++..+..+..+.+++....+.+....+.++...
                .+...++..+.++.++...+......+..+.++.+.....+
                .+.+++..+.+.++..+++.+.++.......++...++..+
                +++......+.+....+....+..+..+.+..++...+.+.
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                ..++.+.+.+++.......+...+...++..++.++...+.
                ++..+...+...++++.+.++...+.++.+...++.++.+.
                .+...+.+..++.+.++.++.++++++.++.....+.+.++
                ++.++.+.++++.++++++++.+.+...++.++++.+++..
                .+...+.+..+...++..++.+.+.+..++.+++.+..+++
                .+.+++.++...+++....++.....+.+.+.+++.++..+
                ..+..+.++..++++..+.+..++++.++.++.+++..+.+
                +++.+......+....+++++.+.++.+.+.++.+.+.+.+
                .....+...+++.++..+.+.....+.++++.++.......
                .+..++.+.+...+++++++++++++++.+.++.+.+++..
                ..++........+.+...........++.+...+....+++
                ....+.++.++++++.+++++++++.+..++.....++++.
                .....+++.+..+.+.+.......+.++..+++.++.....
                ++..+..+.+.+.++.+.+++++.+.+++++++...+.+++
                ++++...+...+.++.+.+...+.+.+..+++..++...++
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                ..+..+...+....+.+.+...+.+.+..+.++........
                ....++..+++++.+.+.+++++.+.+++..++++.+....
                +..+.+.+....+.+.+.......+.++.++.+++..+.+.
                .+.+++.+.++.+++.+++++++++.++....++..++++.
                ..++.+.+.+++..+...........+++.++.+.+..+..
                ..++++.+....+.++++++++++++++.+...++.++.++
                ......+.+.++...+++++..+++...+...+++....+.
                +...+....+.++++.+..++..++..++.....+.+...+
                +..+...+++++..+.++++.+++..+...++++.+.++.+
                .+++++.......+..+++.+...++.++.++++..++...
                +......+....+.++.++..+..+..+.+.++++......
                ..+.++...+..+...+.++++++.++.+++++++++.+.+
                ..+....++.+...+..+.+.+...+..+++..+...++++
                +..++..++++++......+++.+.......+.+..+..++
                +.++..+.......+++++..++..++++++++..+.+.++
                +.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+.+
                +.....+..++++..+..+....+....+.+...++.+++.
                +.+.++.+++.+....++..++++..++.+.+..+.+...+
                ...+..+..+..++..+.++.++.+....++...+...+.+
                +...+.....+.+.+..++.+.......+..+..+++....
            ");

            // Act
            var aztec = AztecEncoder.Encode(content) as AztecCode;

            // Assert
            aztec.Should().NotBeNull();
            expectedDataBits.Length.Should().Be(aztec.Dimension * aztec.Dimension);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % aztec.Dimension;
                int y = i / aztec.Dimension;
                aztec.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
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
            if (lines.Length != dimension)
                throw new InvalidOperationException("Not a square Aztec code");
            foreach (var line in lines)
                if (line.Length != dimension)
                    throw new InvalidOperationException("Not all lines have the same length");
            return lines.SelectMany(x => x).Select(x => x != '.').ToArray();
        }
    }
}
