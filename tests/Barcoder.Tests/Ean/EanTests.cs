using System;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests
{
    public sealed class EanTests
    {
        [Theory]
        [InlineData("5901234123457", "10100010110100111011001100100110111101001110101010110011011011001000010101110010011101000100101", "EAN 13", true)]
        [InlineData("55123457", "1010110001011000100110010010011010101000010101110010011101000100101", "EAN 8", true)]
        [InlineData("5512345", "1010110001011000100110010010011010101000010101110010011101000100101", "EAN 8", false)]
        public void Encode(string testCode, string testResult, string kind, bool checkMetadata)
        {
            IBarcodeIntCS code = Ean.Encode(testCode);
            if (checkMetadata)
            {
                code.Content.Should().Be(testCode);
                code.Metadata.Dimensions.Should().Be(1);
                code.Metadata.CodeKind.Should().Be(kind);
            }

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode_InvalidChecksum_ShouldThrowException()
        {
            Action action = () => Ean.Encode("55123458");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Checksum mismatch");
        }

        [Fact]
        public void Encode_InvalidCode_ShouldThrowException()
        {
            Action action = () => Ean.Encode("invalid");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only encode numerical digits (0-9)");
        }

        [Fact]
        public void Encode_InvalidContentLength_ShouldThrowException()
        {
            Action action = () => Ean.Encode("123");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid content length. Should be 7 or 12 if the code does not include a checksum, 8 or 8 if the code already includes a checksum");
        }
    }
}
