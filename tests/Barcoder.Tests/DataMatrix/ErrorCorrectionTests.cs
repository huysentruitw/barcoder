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
