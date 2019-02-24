using System;
using Barcoder.Qr;
using Barcoder.Qr.InternalEncoders;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr.InternalEncoders
{
    public sealed class AutoEncoderTests
    {
        [Theory]
        [InlineData("0123456789", Encoding.Numeric)]
        [InlineData("ALPHA NUMERIC", Encoding.AlphaNumeric)]
        [InlineData("unicode encoing", Encoding.Unicode)]
        public void Encode_ShouldUseCorrectEncoder(string content, Encoding expectedEncoding)
        {
            // Arrange
            var autoEncoder = new AutoEncoder();
            byte[] expectedBytes = Encode(content, expectedEncoding);

            // Act
            (BitList bits, VersionInfo versionInfo) = autoEncoder.Encode(content, ErrorCorrectionLevel.M);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
            bits.GetBytes().Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public void Encode_UnicodeContentTooLong_ShouldThrowException()
        {
            // Arrange
            var autoEncoder = new AutoEncoder();

            // Act
            Action action = () => autoEncoder.Encode(new string('a', 3000), ErrorCorrectionLevel.M);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"No encoding found to encode*");
        }

        private static byte[] Encode(string content, Encoding encoding)
        {
            if (encoding == Encoding.Numeric)
                return new NumericEncoder().Encode(content, ErrorCorrectionLevel.M).Item1.GetBytes();

            if (encoding == Encoding.AlphaNumeric)
                return new AlphaNumericEncoder().Encode(content, ErrorCorrectionLevel.M).Item1.GetBytes();

            if (encoding == Encoding.Unicode)
                return new UnicodeEncoder().Encode(content, ErrorCorrectionLevel.M).Item1.GetBytes();

            return null;
        }
    }
}
