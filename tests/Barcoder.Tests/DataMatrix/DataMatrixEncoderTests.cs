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
