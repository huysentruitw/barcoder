using System;
using System.IO;
using FluentAssertions;
using Moq;
using Xunit;

namespace Barcoder.Renderer.Image.Tests
{
    public sealed class ImageRendererTests
    {
        [Fact]
        public void Render_PassNullAsBarcode_ShouldThrowException()
        {
            // Arrange
            var renderer = new ImageRenderer();
            var stream = new MemoryStream();

            // Act
            Action action = () => renderer.Render(null, stream);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("barcode");
        }

        [Fact]
        public void Render_PassNullAsOutputStream_ShouldThrowException()
        {
            // Arrange
            var renderer = new ImageRenderer();
            var barcodeMock = new Mock<IBarcode>();

            // Act
            Action action = () => renderer.Render(barcodeMock.Object, null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("outputStream");
        }
    }
}
