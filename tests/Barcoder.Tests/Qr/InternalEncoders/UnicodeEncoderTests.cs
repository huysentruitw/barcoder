using System;
using Barcoder.Qr;
using Barcoder.Qr.InternalEncoders;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr.InternalEncoders
{
    public sealed class UnicodeEncoderTests
    {
        [Fact]
        public void Encode_NonUnicodeContent_ShouldEncodeCorrectly()
        {
            // Arrange
            var unicodeEncoder = new UnicodeEncoder();

            // Act
            (BitList bits, VersionInfo versionInfo) = unicodeEncoder.Encode("A", ErrorCorrectionLevel.H);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
            versionInfo.Version.Should().Be(1);
            bits.GetBytes().Should().BeEquivalentTo(new byte[] { 64, 20, 16, 236, 17, 236, 17, 236, 17 });
        }

        [Fact]
        public void Encode_UnicodeContent_ShouldEncodeCorrectly()
        {
            // Arrange
            var unicodeEncoder = new UnicodeEncoder();

            // Act
            (BitList bits, VersionInfo versionInfo) = unicodeEncoder.Encode("💩", ErrorCorrectionLevel.H);

            // Assert
            bits.Should().NotBeNull();
            versionInfo.Should().NotBeNull();
            versionInfo.Version.Should().Be(1);
            bits.GetBytes().Should().BeEquivalentTo(new byte[] { 64, 126, 251, 187, 255, 9, 249, 42, 144 });
        }

        [Fact]
        public void Encode_ExceedMaximumContentLength_ShouldThrowException()
        {
            // Arrange
            var unicodeEncoder = new UnicodeEncoder();

            // Act
            Action action = () => unicodeEncoder.Encode(new string('A', 3000), ErrorCorrectionLevel.H);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Too much data to encode");
        }
    }
}
