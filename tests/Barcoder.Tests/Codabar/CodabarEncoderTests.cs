using System;
using Barcoder.Codebar;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Codebar
{
    public sealed class CodabarEncoderTests
    {
        [Theory]
        [InlineData("A40156B", "10110010010101101001010101001101010110010110101001010010101101001001011")]
        public void Encode(string txt, string testResult)
        {
            IBarcode code = CodabarEncoder.Encode(txt);
            code.Should().NotBeNull();
            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);
            code.Metadata.CodeKind.Should().Be(BarcodeType.Codabar);
            code.Metadata.Dimensions.Should().Be(1);
            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode_InvalidFormat_ShouldThrowException()
        {
            Action action = () => CodabarEncoder.Encode("C156Z");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("C156Z could not be encoded");
        }
    }
}
