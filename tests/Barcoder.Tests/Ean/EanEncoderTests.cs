using System;
using Barcoder.Ean;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Ean
{
    public sealed class EanEncoderTests
    {
        [Theory]
        [InlineData("5901234123457", "10100010110100111011001100100110111101001110101010110011011011001000010101110010011101000100101", BarcodeType.EAN13, true)]
        [InlineData("55123457", "1010110001011000100110010010011010101000010101110010011101000100101", BarcodeType.EAN8, true)]
        [InlineData("5512345", "1010110001011000100110010010011010101000010101110010011101000100101", BarcodeType.EAN8, false)]
        public void Encode(string testCode, string testResult, string kind, bool checkContent)
        {
            IBarcodeIntCS code = EanEncoder.Encode(testCode);
            if (checkContent)
                code.Content.Should().Be(testCode);

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);
            code.Metadata.CodeKind.Should().Be(kind);
            code.Metadata.Dimensions.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode_InvalidChecksum_ShouldThrowException()
        {
            Action action = () => EanEncoder.Encode("55123458");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Checksum mismatch");
        }

        [Fact]
        public void Encode_InvalidCode_ShouldThrowException()
        {
            Action action = () => EanEncoder.Encode("invalid");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only encode numerical digits (0-9)");
        }

        [Fact]
        public void Encode_InvalidContentLength_ShouldThrowException()
        {
            Action action = () => EanEncoder.Encode("123");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid content length. Should be 7 or 12 if the code does not include a checksum, 8 or 13 if the code already includes a checksum");
        }
    }
}
