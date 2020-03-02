using System.Linq;
using Barcoder.Aztec;
using Barcoder.Utils;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.Aztec
{
    public sealed class HighLevelEncodingTests
    {
        [Theory]
        //                      'A'  P/S   '. ' L/L    b    D/L    '.'
        [InlineData("A. b.", "...X. ..... ...XX XXX.. ...XX XXXX. XX.X")]
        //                             'L'  L/L   'o'   'r'   'e'   'm'   ' '   'i'   'p'   's'   'u'   'm'   D/L   '.'
        [InlineData("Lorem ipsum.", ".XX.X XXX.. X.... X..XX ..XX. .XXX. ....X .X.X. X...X X.X.. X.XX. .XXX. XXXX. XX.X")]
        //                              'L'  L/L   'o'   P/S   '. '  U/S   'T'   'e'   's'   't'    D/L   ' '  '1'  '2'  '3'  '.'
        [InlineData("Lo. Test 123.", ".XX.X XXX.. X.... ..... ...XX XXX.. X.X.X ..XX. X.X.. X.X.X  XXXX. ...X ..XX .X.. .X.X XX.X")]
        //                       'L'  L/L   'o'   D/L   '.'  '.'  '.'  U/L  L/L   'x'
        [InlineData("Lo...x", ".XX.X XXX.. X.... XXXX. XX.X XX.X XX.X XXX. XXX.. XX..X")]
        // Uses Binary/Shift rather than Lower/Shift to save two bits.
        //                       'A'   'B'   'C'   B/S    =1    'd'     'E'   'F'   'G'
        [InlineData("ABCdEFG", "...X. ...XX ..X.. XXXXX ....X .XX..X.. ..XX. ..XXX .X...")]
        public void Encode_Text_ShouldEncodeCorrectly(string content, string expectedBitPattern)
        {
            // Arrange
            byte[] data = content.Select(x => (byte)x).ToArray();
            expectedBitPattern = expectedBitPattern.Replace(" ", string.Empty);

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.ToString().Should().Be(expectedBitPattern);
        }

        [Fact]
        public void Encode_ShouldKeepBitCountLow()
        {
            // Found on an airline boarding pass.  Several stretches of Binary shift are
            // necessary to keep the bit-count so low.

            // Arrange
            const string content = "09  UAG    ^160MEUCIQC0sYS/HpKxnBELR1uB85R20OoqqwFGa0q2uEiYgh6utAIgLl1aBVM4EOTQtMQQYH9M2Z3Dp4qnA/fwWuQ+M8L3V8U=";
            byte[] data = content.Select(x => (byte)x).ToArray();

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.Length.Should().Be(823);
        }

        [Theory]
        //                         'N'  B/S    =1   '\0'      N
        [InlineData("N\u0000N", ".XXXX XXXXX ....X ........ .XXXX")] // Encode "N" in UPPER
        //                         'N'  B/S    =2   '\0'       'n'
        [InlineData("N\u0000n", ".XXXX XXXXX ...X. ........ .XX.XXX.")] // Encode "n" in BINARY
        // Binary short form consecutive bytes
        //                            'N'  B/S    =2    '\0'    \u0080   ' '  'A'
        [InlineData("N\x00\x80 A", ".XXXX XXXXX ...X. ........ X....... ....X ...X.")]
        // Binary skipping over single character
        //                                B/S  =4    '\0'      'a'     '\3ff'   '\200'   ' '   'A'
        [InlineData("\x00"+"a\xFF\x80 A", "XXXXX ..X.. ........ .XX....X XXXXXXXX X....... ....X ...X.")]
        // Getting into binary mode from digit mode
        //                          D/L   '1'  '2'  '3'  '4'  U/L  B/S    =1    \0
        [InlineData("1234\u0000", "XXXX. ..XX .X.. .X.X .XX. XXX. XXXXX ....X ........")]
        public void Encode_MixedBinary_ShouldEncodeCorrectly(string content, string expectedBitPattern)
        {
            // Arrange
            expectedBitPattern = expectedBitPattern.Replace(" ", string.Empty);
            byte[] data = content.Select(x => (byte)x).ToArray();

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.ToString().Should().Be(expectedBitPattern);
        }

        [Theory]
        [InlineData(1, 8 * 1 + 10)]
        [InlineData(2, 8 * 2 + 10)]
        [InlineData(3, 8 * 3 + 10)]
        [InlineData(10, 8 * 10 + 10)]
        [InlineData(29, 8 * 29 + 10)]
        [InlineData(30, 8 * 30 + 10)]
        [InlineData(31, 8 * 31 + 10)]
        [InlineData(32, 8 * 32 + 20)]
        [InlineData(33, 8 * 33 + 20)]
        [InlineData(60, 8 * 60 + 20)]
        [InlineData(61, 8 * 61 + 20)]
        [InlineData(62, 8 * 62 + 20)]
        [InlineData(63, 8 * 63 + 21)]
        [InlineData(64, 8 * 64 + 21)]
        [InlineData(2076, 8 * 2076 + 21)]
        [InlineData(2077, 8 * 2077 + 21)]
        [InlineData(2078, 8 * 2078 + 21)]
        [InlineData(2079, 8 * 2079 + 31)]
        [InlineData(2080, 8 * 2080 + 31)]
        [InlineData(2100, 8 * 2100 + 31)]
        public void Encode_Binary_ShouldEncodeCorrectly(int contentLength, int expectedLength)
        {
            // Arrange
            byte[] data = Enumerable.Range(0, contentLength).Select(i => (byte)(128 + (i % 30))).ToArray();

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.Length.Should().Be(expectedLength);
        }

        // The addition of an 'a' at the beginning or end gets merged into the binary code
        // in those cases where adding another binary character only adds 8 or 9 bits to the result.
        // So we exclude the border cases i=1,32,2079
        // A lower case letter at the beginning will be merged into binary mode
        [Theory]
        [InlineData(2, 8 * 2 + 10)]
        [InlineData(3, 8 * 3 + 10)]
        [InlineData(10, 8 * 10 + 10)]
        [InlineData(29, 8 * 29 + 10)]
        [InlineData(30, 8 * 30 + 10)]
        [InlineData(31, 8 * 31 + 10)]
        [InlineData(33, 8 * 33 + 20)]
        [InlineData(60, 8 * 60 + 20)]
        [InlineData(61, 8 * 61 + 20)]
        [InlineData(62, 8 * 62 + 20)]
        [InlineData(63, 8 * 63 + 21)]
        [InlineData(64, 8 * 64 + 21)]
        [InlineData(2076, 8 * 2076 + 21)]
        [InlineData(2077, 8 * 2077 + 21)]
        [InlineData(2078, 8 * 2078 + 21)]
        [InlineData(2080, 8 * 2080 + 31)]
        [InlineData(2100, 8 * 2100 + 31)]
        public void Encode_Binary_PrecededByLowerCaseChar_ShouldEncodeCorrectly(int contentLength, int expectedLength)
        {
            // Arrange
            byte[] data = new[] { (byte)'a' }
                .Concat(Enumerable.Range(0, contentLength - 1).Select(i => (byte)(128 + (i % 30))))
                .ToArray();

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.Length.Should().Be(expectedLength);
        }

        // The addition of an 'a' at the beginning or end gets merged into the binary code
        // in those cases where adding another binary character only adds 8 or 9 bits to the result.
        // So we exclude the border cases i=1,32,2079
        // A lower case letter at the end will also be merged into binary mode
        [Theory]
        [InlineData(2, 8 * 2 + 10)]
        [InlineData(3, 8 * 3 + 10)]
        [InlineData(10, 8 * 10 + 10)]
        [InlineData(29, 8 * 29 + 10)]
        [InlineData(30, 8 * 30 + 10)]
        [InlineData(31, 8 * 31 + 10)]
        [InlineData(33, 8 * 33 + 20)]
        [InlineData(60, 8 * 60 + 20)]
        [InlineData(61, 8 * 61 + 20)]
        [InlineData(62, 8 * 62 + 20)]
        [InlineData(63, 8 * 63 + 21)]
        [InlineData(64, 8 * 64 + 21)]
        [InlineData(2076, 8 * 2076 + 21)]
        [InlineData(2077, 8 * 2077 + 21)]
        [InlineData(2078, 8 * 2078 + 21)]
        [InlineData(2080, 8 * 2080 + 31)]
        [InlineData(2100, 8 * 2100 + 31)]
        public void Encode_Binary_FollowedByLowerCaseChar_ShouldEncodeCorrectly(int contentLength, int expectedLength)
        {
            // Arrange
            byte[] data = Enumerable.Range(0, contentLength - 1).Select(i => (byte)(128 + (i % 30)))
                .Append((byte)'a')
                .ToArray();

            // Act
            BitList result = HighLevelEncoding.Encode(data);

            // Assert
            result.Length.Should().Be(expectedLength);
        }
    }
}
