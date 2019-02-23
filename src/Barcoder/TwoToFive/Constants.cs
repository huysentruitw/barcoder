using System.Collections.Generic;

namespace Barcoder.TwoToFive
{
    internal static class Constants
    {
        public const int PatternWidth = 5;

        public static readonly IReadOnlyDictionary<char, bool[]> EncodingTable = new Dictionary<char, bool[]>
        {
            { '0', new bool[PatternWidth] { false, false, true, true, false } },
            { '1', new bool[PatternWidth] { true, false, false, false, true } },
            { '2', new bool[PatternWidth] { false, true, false, false, true } },
            { '3', new bool[PatternWidth] { true, true, false, false, false } },
            { '4', new bool[PatternWidth] { false, false, true, false, true } },
            { '5', new bool[PatternWidth] { true, false, true, false, false } },
            { '6', new bool[PatternWidth] { false, true, true, false, false } },
            { '7', new bool[PatternWidth] { false, false, false, true, true } },
            { '8', new bool[PatternWidth] { true, false, false, true, false } },
            { '9', new bool[PatternWidth] { false, true, false, true, false } },
        };

        public static readonly IReadOnlyDictionary<bool, EncodeInfo> Modes = new Dictionary<bool, EncodeInfo>
        {
            {
                false, new EncodeInfo // non-interleaved
                {
                    Start  = new[] { true, true, false, true, true, false, true, false },
                    End    = new[] { true, true, false, true, false, true, true },
                    Widths = new Dictionary<bool, int> { { true, 3 }, { false, 1 } }
                }
            }, {
                true, new EncodeInfo // interleaved
                {
                    Start  = new[] { true, false, true, false },
                    End    = new[] { true, true, false, true },
                    Widths = new Dictionary<bool, int> { { true, 3 }, { false, 1 } }
                }
            }
        };

        public static readonly bool[] NonInterleavedSpace = new bool[PatternWidth] { false, false, false, false, false };

        public struct EncodeInfo
        {
            public bool[] Start { get; set; }
            public bool[] End { get; set; }
            public IReadOnlyDictionary<bool, int> Widths { get; set; }
        }

        public const int Margin = 10;
    }
}
