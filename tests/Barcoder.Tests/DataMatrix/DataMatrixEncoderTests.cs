using System;
using System.Linq;
using Barcoder.DataMatrix;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests.DataMatrix
{
    public sealed class DataMatrixEncoderTests
    {
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
            expectedDataBits.Length.Should().Be(dataMatrix.Bounds.X * dataMatrix.Bounds.Y);
            for (int i = 0; i < expectedDataBits.Length; i++)
            {
                int x = i % dataMatrix.Bounds.X;
                int y = i / dataMatrix.Bounds.X;
                dataMatrix.Get(x, y).Should().Be(expectedDataBits[i], $"of expected bit on index {i}");
            }
        }

        [Fact]
        public void Encode_ValidContent_ShouldEncodeDataMatrixCodeCorrectly_FixedSize()
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
            var dataMatrix = DataMatrixEncoder.Encode(content, 14) as DataMatrixCode;
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
    }
}
