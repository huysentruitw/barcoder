using Barcoder.Qr;
using Barcoder.Qr.InternalEncoders;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Qr
{
    public sealed class VersionInfoTests
    {
        [Theory]
        [InlineData(5, (EncodingMode)100, 0)]
        [InlineData(5, EncodingMode.Numeric, 10)]
        [InlineData(5, EncodingMode.AlphaNumeric, 9)]
        [InlineData(5, EncodingMode.Byte, 8)]
        [InlineData(5, EncodingMode.Kanji, 8)]
        [InlineData(15, EncodingMode.Numeric, 12)]
        [InlineData(15, EncodingMode.AlphaNumeric, 11)]
        [InlineData(15, EncodingMode.Byte, 16)]
        [InlineData(15, EncodingMode.Kanji, 10)]
        [InlineData(30, EncodingMode.Numeric, 14)]
        [InlineData(30, EncodingMode.AlphaNumeric, 13)]
        [InlineData(30, EncodingMode.Byte, 16)]
        [InlineData(30, EncodingMode.Kanji, 12)]
        internal void CharCountBits_ForDifferentVersionsAndEncodingModes_ShouldReturnCorrectAmountOfBits(byte version, EncodingMode encodingMode, byte expectedNumberOfBits)
        {
            // Arrange
            var versionInfo = new VersionInfo(version, ErrorCorrectionLevel.M, 0, 0, 0, 0, 0);

            // Act
            var numberOfBits = versionInfo.CharCountBits(encodingMode);

            // Assert
            numberOfBits.Should().Be(expectedNumberOfBits);
        }

        [Fact]
        public void TotalDataBytes()
        {
            // Arrange
            var versionInfo = new VersionInfo(7, ErrorCorrectionLevel.M, 0, 1, 10, 2, 5);

            // Act
            var totalDataBytes = versionInfo.TotalDataBytes();

            // Assert
            totalDataBytes.Should().Be(20);
        }

        [Fact]
        public void ModulWidth()
        {
            // Arrange
            var versionInfo = new VersionInfo(7, ErrorCorrectionLevel.M, 0, 1, 10, 2, 5);

            // Act
            var modulWidth = versionInfo.ModulWidth();

            // Assert
            modulWidth.Should().Be(45);
        }

        [Fact]
        public void FindSmallestVersionInfo_NumberOfDataBitsOutOfRange_ShouldReturnNull()
        {
            // Act
            var versionInfo = VersionInfo.FindSmallestVersionInfo(ErrorCorrectionLevel.H, EncodingMode.AlphaNumeric, 10208);

            // Assert
            versionInfo.Should().BeNull();
        }

        [Theory]
        [InlineData(10191, 40)]
        [InlineData(5591, 29)]
        [InlineData(5592, 30)]
        [InlineData(190, 3)]
        [InlineData(200, 4)]
        public void FindSmallestVersionInfo_ShouldReturnCorrectVersionForGivenNumberOfDataBits(int dataBits, byte expectedVersion)
        {
            // Act
            var versionInfo = VersionInfo.FindSmallestVersionInfo(ErrorCorrectionLevel.H, EncodingMode.AlphaNumeric, dataBits);

            // Assert
            versionInfo.Should().NotBeNull();
            versionInfo.Version.Should().Be(expectedVersion);
        }

        [Theory]
        [InlineData(1, new int[] { })]
        [InlineData(2, new int[] { 6, 18 })]
        [InlineData(3, new int[] { 6, 22 })]
        [InlineData(4, new int[] { 6, 26 })]
        [InlineData(5, new int[] { 6, 30 })]
        [InlineData(6, new int[] { 6, 34 })]
        [InlineData(7, new int[] { 6, 22, 38 })]
        [InlineData(8, new int[] { 6, 24, 42 })]
        [InlineData(9, new int[] { 6, 26, 46 })]
        [InlineData(10, new int[] { 6, 28, 50 })]
        [InlineData(11, new int[] { 6, 30, 54 })]
        [InlineData(12, new int[] { 6, 32, 58 })]
        [InlineData(13, new int[] { 6, 34, 62 })]
        [InlineData(14, new int[] { 6, 26, 46, 66 })]
        [InlineData(15, new int[] { 6, 26, 48, 70 })]
        [InlineData(16, new int[] { 6, 26, 50, 74 })]
        [InlineData(17, new int[] { 6, 30, 54, 78 })]
        [InlineData(18, new int[] { 6, 30, 56, 82 })]
        [InlineData(19, new int[] { 6, 30, 58, 86 })]
        [InlineData(20, new int[] { 6, 34, 62, 90 })]
        [InlineData(21, new int[] { 6, 28, 50, 72, 94 })]
        [InlineData(22, new int[] { 6, 26, 50, 74, 98 })]
        [InlineData(23, new int[] { 6, 30, 54, 78, 102 })]
        [InlineData(24, new int[] { 6, 28, 54, 80, 106 })]
        [InlineData(25, new int[] { 6, 32, 58, 84, 110 })]
        [InlineData(26, new int[] { 6, 30, 58, 86, 114 })]
        [InlineData(27, new int[] { 6, 34, 62, 90, 118 })]
        [InlineData(28, new int[] { 6, 26, 50, 74, 98, 122 })]
        [InlineData(29, new int[] { 6, 30, 54, 78, 102, 126 })]
        [InlineData(30, new int[] { 6, 26, 52, 78, 104, 130 })]
        [InlineData(31, new int[] { 6, 30, 56, 82, 108, 134 })]
        [InlineData(32, new int[] { 6, 34, 60, 86, 112, 138 })]
        [InlineData(33, new int[] { 6, 30, 58, 86, 114, 142 })]
        [InlineData(34, new int[] { 6, 34, 62, 90, 118, 146 })]
        [InlineData(35, new int[] { 6, 30, 54, 78, 102, 126, 150 })]
        [InlineData(36, new int[] { 6, 24, 50, 76, 102, 128, 154 })]
        [InlineData(37, new int[] { 6, 28, 54, 80, 106, 132, 158 })]
        [InlineData(38, new int[] { 6, 32, 58, 84, 110, 136, 162 })]
        [InlineData(39, new int[] { 6, 26, 54, 82, 110, 138, 166 })]
        [InlineData(40, new int[] { 6, 30, 58, 86, 114, 142, 170 })]
        public void AlignmentPatternPlacements_ShouldReturnCorrectPlacementsForGivenVersion(byte version, int[] expectedPlacements)
        {
            // Arrange
            var versionInfo = new VersionInfo(version, ErrorCorrectionLevel.M, 0, 0, 0, 0, 0);

            // Act
            var placements = versionInfo.AlignmentPatternPlacements();

            // Assert
            placements.Should().BeEquivalentTo(expectedPlacements);
        }
    }
}
