using System;
using Barcoder.Qr;
using Barcoder.Qr.InternalEncoders;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr.InternalEncoders
{
    public sealed class NumericEncoderTests
    {
        [Theory]
        [InlineData("01234567", new byte[] { 16, 32, 12, 86, 97, 128, 236, 17, 236 })]
        [InlineData("0123456789012345", new byte[] { 16, 64, 12, 86, 106, 110, 20, 234, 80 })]
        public void Encode(string content, byte[] expectedBytes)
        {
            // Arrange
            var numericEncoder = new NumericEncoder();

            // Act
            (BitList bits, VersionInfo versionInfo) = numericEncoder.Encode(content, ErrorCorrectionLevel.H);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
            bits.GetBytes().Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void Encode_AlphaNumericContent_ShouldThrowException()
        {
            // Arrange
            var numericEncoder = new NumericEncoder();

            // Act
            Action action = () => numericEncoder.Encode("foo", ErrorCorrectionLevel.H);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("foo can not be ancoded as Numeric");
        }

        [Fact]
        public void Encode_NumericContentTooLong_ShouldThrowException()
        {
            // Arrange
            var numericEncoder = new NumericEncoder();

            // Act
            Action action = () => numericEncoder.Encode(new string('1', 14297), ErrorCorrectionLevel.H);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Too much data to encode");
        }
    }
}
