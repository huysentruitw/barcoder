using System;
using Barcoder.Code39;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Code39
{
    public sealed class Code39EncoderTests
    {
        [Theory]
        [InlineData(false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            "1001011011010110101001011010110100101101101101001010101011001011011010110010101" +
            "011011001010101010011011011010100110101011010011010101011001101011010101001101011010" +
            "100110110110101001010101101001101101011010010101101101001010101011001101101010110010" +
            "101101011001010101101100101100101010110100110101011011001101010101001011010110110010" +
            "110101010011011010101010011011010110100101011010110010101101101100101010101001101011" +
            "011010011010101011001101010101001011011011010010110101011001011010100101101101")]
        public void Encode(bool includeChecksum, bool fullAsciiMode, string data, string testResult)
        {
            IBarcodeIntCS code = Code39Encoder.Encode(data, includeChecksum, fullAsciiMode);

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);
            code.Metadata.CodeKind.Should().Be(BarcodeType.Code39);
            code.Metadata.Dimensions.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode_Checksum()
        {
            IBarcodeIntCS code = Code39Encoder.Encode("5B79AN", true, true);
            code.Checksum.Should().Be('M');
        }

        [Fact]
        public void Encode_ContainsStarInNonAsciiMode_ShouldThrowException()
        {
            Action action = () => Code39Encoder.Encode("01*", false, false);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid data! Try full ASCII mode");
        }

        [Fact]
        public void Encode_UnderscoreInNonAsciiMode_ShouldThrowException()
        {
            Action action = () => Code39Encoder.Encode("_", false, false);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid data! Try full ASCII mode");
        }

        [Fact]
        public void Encode_UnderscoreInAsciiMode_ShouldNotThrowException()
        {
            Action action = () => Code39Encoder.Encode("_", false, true);
            action.Should().NotThrow();
        }

        [Fact]
        public void Encode_NonAsciiCharacterInAsciiMode_ShouldThrowException()
        {
            Action action = () => Code39Encoder.Encode("ù", false, true);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Only ASCII strings can be encoded");
        }
    }
}
