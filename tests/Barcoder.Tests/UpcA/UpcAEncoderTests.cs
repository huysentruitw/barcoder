using System;
using Barcoder.UpcA;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.UpcA
{
    public sealed class UpcAEncoderTests
    {
        [Theory]
        [InlineData("012345678912", "10100011010011001001001101111010100011011000101010101000010001001001000111010011001101101100101", BarcodeType.UPCA, true)]
        [InlineData("01234567891", "10100011010011001001001101111010100011011000101010101000010001001001000111010011001101101100101", BarcodeType.UPCA, false)]
        [InlineData("98765432109", "10100010110110111011101101011110110001010001101010100001011011001100110111001011101001001000101", BarcodeType.UPCA, false)]
        public void Encode(string testCode, string testResult, string kind, bool checkContent)
        {
            IBarcode code = UpcAEncoder.Encode(testCode);
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
            Action action = () => UpcAEncoder.Encode("012345678913");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Checksum mismatch");
        }

        [Fact]
        public void Encode_InvalidCode_ShouldThrowException()
        {
            Action action = () => UpcAEncoder.Encode("invalid");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only encode numerical digits (0-9)");
        }

        [Fact]
        public void Encode_InvalidContentLength_ShouldThrowException()
        {
            Action action = () => UpcAEncoder.Encode("123");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid content length. Should be 11 if the code does not include a checksum, 12 if the code already includes a checksum");
        }
    }
}
