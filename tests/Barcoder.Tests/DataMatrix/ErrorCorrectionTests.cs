using System;
using System.Linq;
using Barcoder.DataMatrix;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.DataMatrix
{
    public sealed class ErrorCorrectionTests
    {
        [Theory]
        [InlineData(new byte[] { 142, 164, 186 }, new byte[] { 142, 164, 186, 114, 25, 5, 88, 102 })]
        [InlineData(new byte[] { 66, 129, 70 }, new byte[] { 66, 129, 70, 138, 234, 82, 82, 95 })]
        [InlineData(new byte[] { 124, 35, 113, 112, 35, 59, 142, 45, 35, 99, 98, 117, 100, 105, 66, 100, 117, 106, 112, 111, 35, 59, 35, 116, 117, 98, 115, 117, 96, 102, 111, 101, 35, 126, 129, 181 }, new byte[] { 124, 35, 113, 112, 35, 59, 142, 45, 35, 99, 98, 117, 100, 105, 66, 100, 117, 106, 112, 111, 35, 59, 35, 116, 117, 98, 115, 117, 96, 102, 111, 101, 35, 126, 129, 181, 196, 53, 147, 192, 151, 213, 107, 61, 98, 251, 50, 71, 186, 15, 43, 111, 165, 243, 209, 79, 128, 109, 251, 4 })]
        public void CalculateEccOfData_ShouldReturnCorrectEccForGivenData(byte[] data, byte[] expectedResult)
        {
            // Arrange
            CodeSize size = CodeSizes.All.FirstOrDefault(x => x.DataCodewords >= data.Length)
                ?? throw new InvalidOperationException("Size not found");

            // Act
            byte[] result = ErrorCorrection.CalculateEcc(data, size);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
