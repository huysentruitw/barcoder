using Barcoder.Qr;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr
{
    public sealed class QrCodeTests
    {
        [Fact]
        public void Constructor()
        {
            // Act
            var qr = new QrCode(2);

            // Assert
            qr.Dimension.Should().Be(2);
            qr.Bounds.Should().BeEquivalentTo(new Bounds(2, 2));
        }

        [Fact]
        public void CalculatePenaltyRule1()
        {
            var qr = new QrCode(7);
            qr.CalculatePenaltyRule1().Should().Be(70);
            qr.Set(0, 0, true);
            qr.CalculatePenaltyRule1().Should().Be(68);
            qr.Set(0, 6, true);
            qr.CalculatePenaltyRule1().Should().Be(66);
        }

        [Fact]
        public void CalculatePenaltyRule2()
        {
            var qr = new QrCode(3);
            qr.CalculatePenaltyRule2().Should().Be(12);
            qr.Set(0, 0, true);
            qr.Set(1, 1, true);
            qr.Set(2, 0, true);
            qr.CalculatePenaltyRule2().Should().Be(0);
            qr.Set(1, 1, false);
            qr.CalculatePenaltyRule2().Should().Be(6);
        }

        [Theory]
        [InlineData("A", 80)]
        [InlineData("FOO", 40)]
        [InlineData("0815", 0)]
        public void CalculatePenaltyRule3(string content, uint expectedPenalty)
        {
            // Arrange
            var qr = QrEncoder.Encode(content, ErrorCorrectionLevel.L, Encoding.AlphaNumeric) as QrCode;

            // Act
            var penalty = qr.CalculatePenaltyRule3();

            // Assert
            penalty.Should().Be(expectedPenalty);
        }

        [Fact]
        public void CalculatePenaltyRule4()
        {
            var qr = new QrCode(3);
            qr.CalculatePenaltyRule4().Should().Be(100);
            qr.Set(0, 0, true);
            qr.CalculatePenaltyRule4().Should().Be(70);
            qr.Set(0, 1, true);
            qr.CalculatePenaltyRule4().Should().Be(50);
            qr.Set(0, 2, true);
            qr.CalculatePenaltyRule4().Should().Be(30);
            qr.Set(1, 0, true);
            qr.CalculatePenaltyRule4().Should().Be(10);
            qr.Set(1, 1, true);
            qr.CalculatePenaltyRule4().Should().Be(10);
        }

        [Fact]
        public void CalculatePenaltyRule4_ZeroCase()
        {
            var qr = new QrCode(2);
            qr.CalculatePenaltyRule4().Should().Be(100);
            qr.Set(0, 0, true);
            qr.Set(1, 0, true);
            qr.CalculatePenaltyRule4().Should().Be(0);
        }
    }
}
