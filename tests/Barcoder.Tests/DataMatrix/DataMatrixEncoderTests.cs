using System;
using System.Linq;
using Barcoder.DataMatrix;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.DataMatrix
{
    /// <summary>
    /// https://www.bcgen.com/datamatrix-barcode-creator.html
    /// X Dimension: 0.03
    ///
    /// </summary>
    public sealed class DataMatrixEncoderTests
    {
        [Theory]
        [InlineData("GZhdz9hk", @"      #.#.#.#.#.#.#.#.#.#.#.#.#.
                                        #.#..##..##.###.####..##.#
                                        #.##.##.######.#...##.###.
                                        #.#####.#.##....#.#.#.#.##
                                        ##...###...#......#.......
                                        #.####.#.#.#.###.##..##..#
                                        ##.#....######.#......##..
                                        #.#.#..##.#.###.#..####.##
                                        #.##.#.##.##...####...##..
                                        #.##..#..#..#.###..#.##..#
                                        ##.#.#....#...#...###...#.
                                        ##########################")]
        [InlineData("Abc12345Gtzw", @"  #.#.#.#.#.#.#.#.#.#.#.#.#.
                                        #.##....##..###.###.###.##
                                        ##..#.##....##.#.#.#.##...
                                        #.###...#..#...#.#.#.##.##
                                        #...#..###.#...#.#.#..#...
                                        #.##..#....#..###.#.....##
                                        ####.########.#.##.###....
                                        #.#.#.###.#.###.#.#...##.#
                                        #..##.###.##.###.##..#....
                                        #.##.#####.##..#.#..###.##
                                        ##..#...#.#...#.#......#..
                                        ##########################")]
        public void Encode_12x26(string content, string imageStr)
        {
            var expectedDataBits = ImageStringToBools(imageStr);

            TestEncode_FixedSize(12, 26, content, expectedDataBits);
        }

        [Fact]
        public void Encode_12x36()
        {
            var content = "Abc12345Hij!klm";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.
                #.##....#.##.##.#######......###...#
                ##..#.##.#.##.##..#.##.#..#...###...
                #.###....#......########..###..##.##
                #...#..#...#..#...###.#.....##......
                #.##..#.#..#.#...####...#.....##..##
                ####.#.########.#.######..##.###.#..
                #.#.#.###.#...#.###...##.###.#..#..#
                #.##.##.#.#.##....#.#....#####...#..
                #.#.#.....##..#.###.#.#.##..##.#..##
                ##..#.#.##.#.#....###...####...#.#..
                ####################################
            ");

            TestEncode_FixedSize(12, 36, content, expectedDataBits);
        }

        [Fact]
        public void Encode_16x36()
        {
            var content = "AbcdF12345ZY#123";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.
                #.##..#......#.#.######.#.###...##.#
                ##...##.###.#####.#######.##..####..
                #.##...#.#..#.#.#####....#.####...##
                #.#.###..##.#.##..##.#.###.#.##.....
                #...#..##....##..##..###.#.#...##.##
                #.##.#....#.#.#...#..#..###.....#.#.
                #...##...##....####...#######.###.##
                #...###.#..####.#.#.#.#...#..##...#.
                #.#.#.##.#..##...##.###..#..#.#...##
                #.##.#.#.#..#.#...##......#.#..#..#.
                #.##..#..#..#.#.####.#.##..#.#.#...#
                #.##..#..###.####.######.....#..###.
                #..##.###.##.#.####.####.#...#..####
                #.####...##.#.....#####.#......##.#.
                ####################################
            ");

            TestEncode_FixedSize(16, 36, content, expectedDataBits);
        }

        [Fact]
        public void Encode_16x48()
        {
            var content = "AbcDef1234567Abc1";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.
                #.##......#.#.....###..###.#.##..#..##..##.##..#
                ##...##.##..##..##.#..#.##.#...##..#.###.####.#.
                #.###...##..#.###...#.###..####.#.#...#.#...##.#
                #..###.#..#.#.#.#.###.#.#.#....###.##.##..#.##..
                #.#...#.##.......#....###..#.....###.#.#####..##
                #.###.....#.####.#.####.###..##..#..##.#.#.##.#.
                #.#...#..###..##.####..###.#########.#.....#..##
                ##.####.#..#..##.#.#....#...#..##.#.#..####.#...
                ##....##...#..##..#.#.####..##.#.#.#.###..##..##
                ####.#.#...##......###..#..#####.##.#.#..##...#.
                #.#...#.###...##.#.....##.#.#.#.#.#...##.####..#
                ####..###......##..###..###.#.#...#.#.#..##...#.
                #..##.###..##..#.#.######..#......#....##...##.#
                ######...######.#.#.###.###.########......#..#..
                ################################################
            ");

            TestEncode_FixedSize(16, 48, content, expectedDataBits);
        }

        [Theory]
        [InlineData("123456", @"#.#.#.#.#.#.#.#.#.
                                ##..#.....##.....#
                                ##...#..##.####.#.
                                ##..##...#...###.#
                                ####.##..###..#...
                                #.####...#...#.###
                                #....####.##.##.#.
                                ##################")]
        [InlineData("Gulz7", @" #.#.#.#.#.#.#.#.#.
                                #.##.######.#..#.#
                                ###....##.........
                                ###.###.....#...##
                                ###......#..##....
                                #.##..##.######..#
                                #...##.#.#......#.
                                ##################")]
        public void Encode_8x18(string content, string imageStr)
        {
            var expectedDataBits = ImageStringToBools(imageStr);

            TestEncode_FixedSize(8, 18, content, expectedDataBits);
        }

        [Fact]
        public void Encode_8x32()
        {
            var content = "Abc12345";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.
                #.##....#..##.###..##..#..###..#
                ##..#.##.####.#.#.#####..#####..
                #.###..#.......#####....#.###.##
                #...#.....#####.#.###..##..###..
                ####..#..#.#.####.###.####.#...#
                #.##.#..#..#....##.###.#...##...
                ################################
            ");

            TestEncode_FixedSize(8, 32, content, expectedDataBits);
        }

        [Fact]
        public void Encode_FixedNumberOfRows_ValidContent_ShouldEncodeDataMatrixCodeCorrectly()
        {
            // Arrange
            var content = "1234567890";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.
                ##..#..#...###
                ##..##...###..
                ##...##....#.#
                ###.#......#..
                #.......##...#
                #...##.####...
                #..........#.#
                #.###..##.....
                ###.##.#.#####
                #.####...#..#.
                #.###..#.##.##
                #..##.######..
                ##############
            ");

            // Act
            var dataMatrix = DataMatrixEncoder.Encode(content, fixedNumberOfRows: 14) as DataMatrixCode;

            // Assert
            dataMatrix.Should().NotBeNull();
            dataMatrix.Bounds.X.Should().Be(14);
            dataMatrix.Bounds.Y.Should().Be(14);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_Gs1ModeEnabled_ValidGs1Content_NonPreDefinedApplicationIdentifiers_ShouldEncodeDataMatrixGs1Correctly()
        {
            // Arrange
            var content = "(10) 12 (22) 34";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.
                ##.##.#.##.###
                #..##....#.#..
                ##...##...#..#
                ###....#####..
                #.#..#.##.##.#
                #.#..#..###.#.
                #..#.#...#.###
                #.##.#######..
                #..#.#.#.#...#
                ###.###...#.#.
                #.#...##..##.#
                #..#.##..###..
                ##############
            ");

            // Act
            var dataMatrix = DataMatrixEncoder.Encode(content, gs1ModeEnabled: true) as DataMatrixCode;

            // Assert
            dataMatrix.Should().NotBeNull();
            (dataMatrix.Bounds.X * dataMatrix.Bounds.Y).Should().Be(expectedDataBits.Length);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_Gs1ModeEnabled_ValidGs1Content_ShouldEncodeDataMatrixGs1Correctly()
        {
            // Arrange
            var content = "(01) 0 0614141 99999 6 (90) 1234567890";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.
                ##..#.####.....###
                #...#.#######.#...
                #.###.##..#....###
                ###..###..#...##..
                #.#.##.#.....##..#
                ##.##..#..#.###.#.
                #..####..##.....##
                #.#.###.#..######.
                ####....##.#..#..#
                ##..####.#..####..
                #.#.......###.####
                #.##.#....##..##..
                ###.#####.##..##.#
                ##...#.#.###.#....
                #..#...#..###..#.#
                ##....#.....#.###.
                ##################
            ");

            // Act
            var dataMatrix = DataMatrixEncoder.Encode(content, gs1ModeEnabled: true) as DataMatrixCode;

            // Assert
            dataMatrix.Should().NotBeNull();
            (dataMatrix.Bounds.X * dataMatrix.Bounds.Y).Should().Be(expectedDataBits.Length);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_ValidContent_ShouldEncodeDataMatrixCodeCorrectly()
        {
            // Arrange
            var content = "{\"po\":12,\"batchAction\":\"start_end\"}";
            var expectedDataBits = ImageStringToBools(@"
                #.#.#.#.#.#.#.#.#.#.#.#.
                #....###..#..#....#...##
                ##.......#...#.#.#....#.
                #.###...##..#...##.##..#
                ##...####..##..#.#.#.##.
                #.###.##.###..#######.##
                #..###...##.##..#.##.##.
                #.#.#.#.#.#.###....#.#.#
                ##.#...#.#.#..#...#####.
                #...####..#...##..#.#..#
                ##...#...##.###.#.....#.
                #.###.#.##.#.....###..##
                ##..#####...#..##...###.
                ###...#.####.##.#.#.#..#
                #..###..#.#.####.#.###..
                ###.#.#..#..#.###.#.##.#
                #####.##.###..#.####.#..
                #.##.#......#.#..#.#.###
                ###.#....######.#...##..
                ##...#..##.###..#...####
                #.######.###.##..#...##.
                #..#..#.##.#..####...#.#
                ###.###..#..##.#.##...#.
                ########################
            ");

            // Act
            var dataMatrix = DataMatrixEncoder.Encode(content) as DataMatrixCode;

            // Assert
            dataMatrix.Should().NotBeNull();
            (dataMatrix.Bounds.X * dataMatrix.Bounds.Y).Should().Be(expectedDataBits.Length);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}. x:{x}, y:{y}");
            }
        }

        [Fact]
        public void TextEncodingWithPadding()
        {
            // Arrange
            var content = "{\"po\":12,\"batchAction\":\"start_end\"}";

            // Act
            var result = DataMatrixEncoder.AddPadding(DataMatrixEncoder.EncodeText(content), 36);

            // Assert
            result.Should().BeEquivalentTo(new byte[] { 124, 35, 113, 112, 35, 59, 142, 45, 35, 99, 98, 117, 100, 105, 66, 100, 117, 106, 112, 111, 35, 59, 35, 116, 117, 98, 115, 117, 96, 102, 111, 101, 35, 126, 129, 181 });
        }

        private static bool[] ImageStringToBools(string imageString)
        {
            var lines = imageString?
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            if (lines.Length <= 0)
                throw new InvalidOperationException($"No data in {nameof(imageString)}");
            var cols = lines.First().Length;
            var rows = lines.Length;
            foreach (var line in lines)
                if (line.Length != cols)
                    throw new InvalidOperationException("Not all lines have the same length");
            return lines.SelectMany(x => x).Select(x => x != '.').ToArray();
        }

        private static void TestEncode_FixedSize(int rows, int columns, string content, bool[] expectedDataBits)
        {
            var dataMatrix = DataMatrixEncoder.Encode(content, fixedNumberOfRows: rows, fixedNumberOfColumns: columns) as DataMatrixCode;
            dataMatrix.Should().NotBeNull();
            dataMatrix.Bounds.X.Should().Be(columns);
            dataMatrix.Bounds.Y.Should().Be(rows);

            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}. x:{x}, y:{y}");
            }
        }
    }
}
