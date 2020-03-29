using System.Collections.Generic;
using System.Linq;
using Barcoder.Pdf417;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Pdf417
{
    public sealed class HighLevelEncodingTests
    {
        [Theory]
        [InlineData("01234", 902, 112, 434)]
        [InlineData("Super !", 567, 615, 137, 809, 329)]
        [InlineData("Super ", 567, 615, 137, 809)]
        [InlineData("ABC123", 1, 88, 32, 119)]
        [InlineData("123ABC", 841, 63, 840, 32)]
        public void Encode_ShouldEncodeCorrectly(string msg, params int[] expected)
        {
            // Act
            int[] result = HighLevelEncoding.Encode(msg);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("alcool", 924, 163, 238, 432, 766, 244)]
        [InlineData("alcoolique", 901, 163, 238, 432, 766, 244, 105, 113, 117, 101)]
        public void EncodeBinary_ShouldEncodeCorrectly(string msg, params int[] expected)
        {
            // Act
            IEnumerable<int> result = HighLevelEncoding.EncodeBinary(msg, HighLevelEncoding.EncodingMode.Text);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
