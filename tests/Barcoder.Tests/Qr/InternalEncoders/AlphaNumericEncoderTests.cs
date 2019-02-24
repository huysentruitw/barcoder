using System;
using Barcoder.Qr;
using Barcoder.Qr.InternalEncoders;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr.InternalEncoders
{
    public sealed class AlphaNumericEncoderTests
    {
        [Fact]
        public void Encode_AlphaNumericContent_ShouldEncodeCorrectly()
        {
            // Arrange
            var alphaNumericEncoder = new AlphaNumericEncoder();

            // Act
            (BitList bits, VersionInfo versionInfo) = alphaNumericEncoder.Encode("HELLO WORLD", ErrorCorrectionLevel.M);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
            versionInfo.Version.Should().Be(1);
            bits.GetBytes().Should().BeEquivalentTo(new byte[] { 32, 91, 11, 120, 209, 114, 220, 77, 67, 64, 236, 17, 236, 17, 236, 17 });
        }

        [Fact]
        public void Encode_MaximumContentLength_ShouldSucceed()
        {
            // Arrange
            var alphaNumericEncoder = new AlphaNumericEncoder();

            // Act
            (BitList bits, VersionInfo versionInfo) = alphaNumericEncoder.Encode(new string('A', 4296), ErrorCorrectionLevel.L);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
        }

        [Fact]
        public void Encode_ExceedMaximumContentLength_ShouldThrowException()
        {
            // Arrange
            var alphaNumericEncoder = new AlphaNumericEncoder();

            // Act
            Action action = () => alphaNumericEncoder.Encode(new string('A', 4297), ErrorCorrectionLevel.L);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Too much data to encode");
        }

        [Theory]
        [InlineData("ABc", ErrorCorrectionLevel.L)]
        [InlineData("hello world", ErrorCorrectionLevel.M)]
        public void Encode_UnknownCharacterInContent_ShouldThrowException(string content, ErrorCorrectionLevel errorCorrectionLevel)
        {
            // Arrange
            var alphaNumericEncoder = new AlphaNumericEncoder();

            // Act
            Action action = () => alphaNumericEncoder.Encode(content, errorCorrectionLevel);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{content} can not be ancoded as AlphaNumeric");
        }
    }
}
