using System;
using Barcoder.UpcE;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.UpcE
{
    public sealed class UpcEEncoderTests
    {
        [Theory]
        [InlineData("654321", UpcENumberSystem.Zero, "101000010101100010011101011110100110110011001010101", BarcodeType.UPCE)]
        [InlineData("654321", UpcENumberSystem.One, "101010111101110010100011011110100110110110011010101", BarcodeType.UPCE)]
        [InlineData("123456", UpcENumberSystem.Zero, "101011001100100110111101001110101110010101111010101", BarcodeType.UPCE)]
        public void Encode(string testCode, UpcENumberSystem numberSystem, string testResult, string kind)
        {
            IBarcode code = UpcEEncoder.Encode(testCode, numberSystem);
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
        public void Encode_InvalidCode_ShouldThrowException()
        {
            Action action = () => UpcEEncoder.Encode("invalid", UpcENumberSystem.Zero);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only encode numerical digits (0-9)");

            Action action2 = () => UpcEEncoder.Encode("invalid", UpcENumberSystem.One);
            action2.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only encode numerical digits (0-9)");
        }

        [Fact]
        public void Encode_InvalidNumberSystem_ShouldThrowException()
        {
            Action action = () => UpcEEncoder.Encode("654321", (UpcENumberSystem)2);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Only number systems 0 and 1 are supported by UPC E");
        }

        [Fact]
        public void Encode_InvalidContentLength_ShouldThrowException()
        {
            Action action = () => UpcEEncoder.Encode("123", UpcENumberSystem.Zero);
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid content length. Should be 6");

            Action action2 = () => UpcEEncoder.Encode("123", UpcENumberSystem.One);
            action2.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid content length. Should be 6");
        }
    }
}
