using System;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Utils
{
    public sealed class Gs1EncoderTests
    {
        [Theory]
        [InlineData("(242) 123 (3124) 123456 (15) 20 01 01", "/242123/312412345615200101")]
        [InlineData("(15)123456(15)123456", "/1512345615123456")]
        public void Encode_ValidContent_ShouldEncodeCorrectly(string content, string expectedResult)
        {
            // Act
            string result = Gs1Encoder.Encode(content, '/');

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("(15)1234(15)123456", @"Application identifier '151234' does not match '^15(\d{6})$'")]
        [InlineData("(11)ABCDEF", @"Application identifier '11ABCDEF' does not match '^11(\d{6})$'")]
        public void Encode_InvalidContent_ShouldThrowException(string content, string expectedExceptionMessage)
        {
            // Act
            Action action = () => Gs1Encoder.Encode(content, '/');

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"*{expectedExceptionMessage}*")
                .Which.ParamName.Should().Be("content");
        }
    }
}
