using System;
using System.Linq;
using Barcoder.Qr.InternalEncoders;

namespace Barcoder.Qr
{
    internal sealed class VersionInfo
    {
        public VersionInfo(
            byte version,
            ErrorCorrectionLevel errorCorrectionLevel,
            byte errorCorrectionCodewordsPerBlock,
            byte numberOfBlocksInGroup1,
            byte dataCodewordsPerBlockInGroup1,
            byte numberOfBlocksInGroup2,
            byte dataCodewordsPerBlockInGroup2)
        {
            Version = version;
            ErrorCorrectionLevel = errorCorrectionLevel;
            ErrorCorrectionCodewordsPerBlock = errorCorrectionCodewordsPerBlock;
            NumberOfBlocksInGroup1 = numberOfBlocksInGroup1;
            DataCodewordsPerBlockInGroup1 = dataCodewordsPerBlockInGroup1;
            NumberOfBlocksInGroup2 = numberOfBlocksInGroup2;
            DataCodewordsPerBlockInGroup2 = dataCodewordsPerBlockInGroup2;
        }

        public byte Version { get; }
        public ErrorCorrectionLevel ErrorCorrectionLevel { get; }
        public byte ErrorCorrectionCodewordsPerBlock { get; }
        public byte NumberOfBlocksInGroup1 { get; }
        public byte DataCodewordsPerBlockInGroup1 { get; }
        public byte NumberOfBlocksInGroup2 { get; }
        public byte DataCodewordsPerBlockInGroup2 { get; }

        public int TotalDataBytes()
            => ((int)NumberOfBlocksInGroup1 * (int)DataCodewordsPerBlockInGroup1)
             + ((int)NumberOfBlocksInGroup2 * (int)DataCodewordsPerBlockInGroup2);

        public byte CharCountBits(EncodingMode encodingMode)
        {
            switch (encodingMode)
            {
            case EncodingMode.Numeric:
                if (Version < 10)
                    return 10;
                if (Version < 27)
                    return 12;
                return 14;

            case EncodingMode.AlphaNumeric:
                if (Version < 10)
                    return 9;
                if (Version < 27)
                    return 11;
                return 13;

            case EncodingMode.Byte:
                if (Version < 10)
                    return 8;
                return 16;

            case EncodingMode.Kanji:
                if (Version < 10)
                    return 8;
                if (Version < 27)
                    return 10;
                return 12;

            default:
                return 0;
            }
        }

        public int ModulWidth()
            => (((int)Version - 1) * 4) + 21;

        public int[] AlignmentPatternPlacements()
        {
            if (Version == 1)
                return Array.Empty<int>();

            int first = 6;
            int last = ModulWidth() - 7;
            double space = last - first;
            int count = (int)Math.Ceiling(space / 28.0) + 1;

            var result = new int[count];
            result[0] = first;
            result[result.Length - 1] = last;
            if (count > 2)
            {
                int step = (int)(Math.Ceiling(space / (count - 1)));
                if (step % 2 == 1)
                {
                    int frac = (int)Math.Round(space / (count - 1));
                    if (frac % 2 == 0)
                        step--;
                    else
                        step++;
                }

                for (var i = 1; i <= count-2; i++)
                    result[i] = last - (step * (count - 1 - i));
            }

            return result;
        }

        public static VersionInfo FindSmallestVersionInfo(ErrorCorrectionLevel errorCorrectionLevel, EncodingMode encodingMode, int dataBits)
        {
            dataBits += 4; // Mode indicator
            foreach (VersionInfo versionInfo in VersionInfos.Where(x => x.ErrorCorrectionLevel == errorCorrectionLevel))
                if ((versionInfo.TotalDataBytes() * 8) >= (dataBits + (int)versionInfo.CharCountBits(encodingMode)))
                    return versionInfo;
            return null;
        }

        private static readonly VersionInfo[] VersionInfos = new[]
        {
            new VersionInfo(1, ErrorCorrectionLevel.L, 7, 1, 19, 0, 0),
            new VersionInfo(1, ErrorCorrectionLevel.M, 10, 1, 16, 0, 0),
            new VersionInfo(1, ErrorCorrectionLevel.Q, 13, 1, 13, 0, 0),
            new VersionInfo(1, ErrorCorrectionLevel.H, 17, 1, 9, 0, 0),
            new VersionInfo(2, ErrorCorrectionLevel.L, 10, 1, 34, 0, 0),
            new VersionInfo(2, ErrorCorrectionLevel.M, 16, 1, 28, 0, 0),
            new VersionInfo(2, ErrorCorrectionLevel.Q, 22, 1, 22, 0, 0),
            new VersionInfo(2, ErrorCorrectionLevel.H, 28, 1, 16, 0, 0),
            new VersionInfo(3, ErrorCorrectionLevel.L, 15, 1, 55, 0, 0),
            new VersionInfo(3, ErrorCorrectionLevel.M, 26, 1, 44, 0, 0),
            new VersionInfo(3, ErrorCorrectionLevel.Q, 18, 2, 17, 0, 0),
            new VersionInfo(3, ErrorCorrectionLevel.H, 22, 2, 13, 0, 0),
            new VersionInfo(4, ErrorCorrectionLevel.L, 20, 1, 80, 0, 0),
            new VersionInfo(4, ErrorCorrectionLevel.M, 18, 2, 32, 0, 0),
            new VersionInfo(4, ErrorCorrectionLevel.Q, 26, 2, 24, 0, 0),
            new VersionInfo(4, ErrorCorrectionLevel.H, 16, 4, 9, 0, 0),
            new VersionInfo(5, ErrorCorrectionLevel.L, 26, 1, 108, 0, 0),
            new VersionInfo(5, ErrorCorrectionLevel.M, 24, 2, 43, 0, 0),
            new VersionInfo(5, ErrorCorrectionLevel.Q, 18, 2, 15, 2, 16),
            new VersionInfo(5, ErrorCorrectionLevel.H, 22, 2, 11, 2, 12),
            new VersionInfo(6, ErrorCorrectionLevel.L, 18, 2, 68, 0, 0),
            new VersionInfo(6, ErrorCorrectionLevel.M, 16, 4, 27, 0, 0),
            new VersionInfo(6, ErrorCorrectionLevel.Q, 24, 4, 19, 0, 0),
            new VersionInfo(6, ErrorCorrectionLevel.H, 28, 4, 15, 0, 0),
            new VersionInfo(7, ErrorCorrectionLevel.L, 20, 2, 78, 0, 0),
            new VersionInfo(7, ErrorCorrectionLevel.M, 18, 4, 31, 0, 0),
            new VersionInfo(7, ErrorCorrectionLevel.Q, 18, 2, 14, 4, 15),
            new VersionInfo(7, ErrorCorrectionLevel.H, 26, 4, 13, 1, 14),
            new VersionInfo(8, ErrorCorrectionLevel.L, 24, 2, 97, 0, 0),
            new VersionInfo(8, ErrorCorrectionLevel.M, 22, 2, 38, 2, 39),
            new VersionInfo(8, ErrorCorrectionLevel.Q, 22, 4, 18, 2, 19),
            new VersionInfo(8, ErrorCorrectionLevel.H, 26, 4, 14, 2, 15),
            new VersionInfo(9, ErrorCorrectionLevel.L, 30, 2, 116, 0, 0),
            new VersionInfo(9, ErrorCorrectionLevel.M, 22, 3, 36, 2, 37),
            new VersionInfo(9, ErrorCorrectionLevel.Q, 20, 4, 16, 4, 17),
            new VersionInfo(9, ErrorCorrectionLevel.H, 24, 4, 12, 4, 13),
            new VersionInfo(10, ErrorCorrectionLevel.L, 18, 2, 68, 2, 69),
            new VersionInfo(10, ErrorCorrectionLevel.M, 26, 4, 43, 1, 44),
            new VersionInfo(10, ErrorCorrectionLevel.Q, 24, 6, 19, 2, 20),
            new VersionInfo(10, ErrorCorrectionLevel.H, 28, 6, 15, 2, 16),
            new VersionInfo(11, ErrorCorrectionLevel.L, 20, 4, 81, 0, 0),
            new VersionInfo(11, ErrorCorrectionLevel.M, 30, 1, 50, 4, 51),
            new VersionInfo(11, ErrorCorrectionLevel.Q, 28, 4, 22, 4, 23),
            new VersionInfo(11, ErrorCorrectionLevel.H, 24, 3, 12, 8, 13),
            new VersionInfo(12, ErrorCorrectionLevel.L, 24, 2, 92, 2, 93),
            new VersionInfo(12, ErrorCorrectionLevel.M, 22, 6, 36, 2, 37),
            new VersionInfo(12, ErrorCorrectionLevel.Q, 26, 4, 20, 6, 21),
            new VersionInfo(12, ErrorCorrectionLevel.H, 28, 7, 14, 4, 15),
            new VersionInfo(13, ErrorCorrectionLevel.L, 26, 4, 107, 0, 0),
            new VersionInfo(13, ErrorCorrectionLevel.M, 22, 8, 37, 1, 38),
            new VersionInfo(13, ErrorCorrectionLevel.Q, 24, 8, 20, 4, 21),
            new VersionInfo(13, ErrorCorrectionLevel.H, 22, 12, 11, 4, 12),
            new VersionInfo(14, ErrorCorrectionLevel.L, 30, 3, 115, 1, 116),
            new VersionInfo(14, ErrorCorrectionLevel.M, 24, 4, 40, 5, 41),
            new VersionInfo(14, ErrorCorrectionLevel.Q, 20, 11, 16, 5, 17),
            new VersionInfo(14, ErrorCorrectionLevel.H, 24, 11, 12, 5, 13),
            new VersionInfo(15, ErrorCorrectionLevel.L, 22, 5, 87, 1, 88),
            new VersionInfo(15, ErrorCorrectionLevel.M, 24, 5, 41, 5, 42),
            new VersionInfo(15, ErrorCorrectionLevel.Q, 30, 5, 24, 7, 25),
            new VersionInfo(15, ErrorCorrectionLevel.H, 24, 11, 12, 7, 13),
            new VersionInfo(16, ErrorCorrectionLevel.L, 24, 5, 98, 1, 99),
            new VersionInfo(16, ErrorCorrectionLevel.M, 28, 7, 45, 3, 46),
            new VersionInfo(16, ErrorCorrectionLevel.Q, 24, 15, 19, 2, 20),
            new VersionInfo(16, ErrorCorrectionLevel.H, 30, 3, 15, 13, 16),
            new VersionInfo(17, ErrorCorrectionLevel.L, 28, 1, 107, 5, 108),
            new VersionInfo(17, ErrorCorrectionLevel.M, 28, 10, 46, 1, 47),
            new VersionInfo(17, ErrorCorrectionLevel.Q, 28, 1, 22, 15, 23),
            new VersionInfo(17, ErrorCorrectionLevel.H, 28, 2, 14, 17, 15),
            new VersionInfo(18, ErrorCorrectionLevel.L, 30, 5, 120, 1, 121),
            new VersionInfo(18, ErrorCorrectionLevel.M, 26, 9, 43, 4, 44),
            new VersionInfo(18, ErrorCorrectionLevel.Q, 28, 17, 22, 1, 23),
            new VersionInfo(18, ErrorCorrectionLevel.H, 28, 2, 14, 19, 15),
            new VersionInfo(19, ErrorCorrectionLevel.L, 28, 3, 113, 4, 114),
            new VersionInfo(19, ErrorCorrectionLevel.M, 26, 3, 44, 11, 45),
            new VersionInfo(19, ErrorCorrectionLevel.Q, 26, 17, 21, 4, 22),
            new VersionInfo(19, ErrorCorrectionLevel.H, 26, 9, 13, 16, 14),
            new VersionInfo(20, ErrorCorrectionLevel.L, 28, 3, 107, 5, 108),
            new VersionInfo(20, ErrorCorrectionLevel.M, 26, 3, 41, 13, 42),
            new VersionInfo(20, ErrorCorrectionLevel.Q, 30, 15, 24, 5, 25),
            new VersionInfo(20, ErrorCorrectionLevel.H, 28, 15, 15, 10, 16),
            new VersionInfo(21, ErrorCorrectionLevel.L, 28, 4, 116, 4, 117),
            new VersionInfo(21, ErrorCorrectionLevel.M, 26, 17, 42, 0, 0),
            new VersionInfo(21, ErrorCorrectionLevel.Q, 28, 17, 22, 6, 23),
            new VersionInfo(21, ErrorCorrectionLevel.H, 30, 19, 16, 6, 17),
            new VersionInfo(22, ErrorCorrectionLevel.L, 28, 2, 111, 7, 112),
            new VersionInfo(22, ErrorCorrectionLevel.M, 28, 17, 46, 0, 0),
            new VersionInfo(22, ErrorCorrectionLevel.Q, 30, 7, 24, 16, 25),
            new VersionInfo(22, ErrorCorrectionLevel.H, 24, 34, 13, 0, 0),
            new VersionInfo(23, ErrorCorrectionLevel.L, 30, 4, 121, 5, 122),
            new VersionInfo(23, ErrorCorrectionLevel.M, 28, 4, 47, 14, 48),
            new VersionInfo(23, ErrorCorrectionLevel.Q, 30, 11, 24, 14, 25),
            new VersionInfo(23, ErrorCorrectionLevel.H, 30, 16, 15, 14, 16),
            new VersionInfo(24, ErrorCorrectionLevel.L, 30, 6, 117, 4, 118),
            new VersionInfo(24, ErrorCorrectionLevel.M, 28, 6, 45, 14, 46),
            new VersionInfo(24, ErrorCorrectionLevel.Q, 30, 11, 24, 16, 25),
            new VersionInfo(24, ErrorCorrectionLevel.H, 30, 30, 16, 2, 17),
            new VersionInfo(25, ErrorCorrectionLevel.L, 26, 8, 106, 4, 107),
            new VersionInfo(25, ErrorCorrectionLevel.M, 28, 8, 47, 13, 48),
            new VersionInfo(25, ErrorCorrectionLevel.Q, 30, 7, 24, 22, 25),
            new VersionInfo(25, ErrorCorrectionLevel.H, 30, 22, 15, 13, 16),
            new VersionInfo(26, ErrorCorrectionLevel.L, 28, 10, 114, 2, 115),
            new VersionInfo(26, ErrorCorrectionLevel.M, 28, 19, 46, 4, 47),
            new VersionInfo(26, ErrorCorrectionLevel.Q, 28, 28, 22, 6, 23),
            new VersionInfo(26, ErrorCorrectionLevel.H, 30, 33, 16, 4, 17),
            new VersionInfo(27, ErrorCorrectionLevel.L, 30, 8, 122, 4, 123),
            new VersionInfo(27, ErrorCorrectionLevel.M, 28, 22, 45, 3, 46),
            new VersionInfo(27, ErrorCorrectionLevel.Q, 30, 8, 23, 26, 24),
            new VersionInfo(27, ErrorCorrectionLevel.H, 30, 12, 15, 28, 16),
            new VersionInfo(28, ErrorCorrectionLevel.L, 30, 3, 117, 10, 118),
            new VersionInfo(28, ErrorCorrectionLevel.M, 28, 3, 45, 23, 46),
            new VersionInfo(28, ErrorCorrectionLevel.Q, 30, 4, 24, 31, 25),
            new VersionInfo(28, ErrorCorrectionLevel.H, 30, 11, 15, 31, 16),
            new VersionInfo(29, ErrorCorrectionLevel.L, 30, 7, 116, 7, 117),
            new VersionInfo(29, ErrorCorrectionLevel.M, 28, 21, 45, 7, 46),
            new VersionInfo(29, ErrorCorrectionLevel.Q, 30, 1, 23, 37, 24),
            new VersionInfo(29, ErrorCorrectionLevel.H, 30, 19, 15, 26, 16),
            new VersionInfo(30, ErrorCorrectionLevel.L, 30, 5, 115, 10, 116),
            new VersionInfo(30, ErrorCorrectionLevel.M, 28, 19, 47, 10, 48),
            new VersionInfo(30, ErrorCorrectionLevel.Q, 30, 15, 24, 25, 25),
            new VersionInfo(30, ErrorCorrectionLevel.H, 30, 23, 15, 25, 16),
            new VersionInfo(31, ErrorCorrectionLevel.L, 30, 13, 115, 3, 116),
            new VersionInfo(31, ErrorCorrectionLevel.M, 28, 2, 46, 29, 47),
            new VersionInfo(31, ErrorCorrectionLevel.Q, 30, 42, 24, 1, 25),
            new VersionInfo(31, ErrorCorrectionLevel.H, 30, 23, 15, 28, 16),
            new VersionInfo(32, ErrorCorrectionLevel.L, 30, 17, 115, 0, 0),
            new VersionInfo(32, ErrorCorrectionLevel.M, 28, 10, 46, 23, 47),
            new VersionInfo(32, ErrorCorrectionLevel.Q, 30, 10, 24, 35, 25),
            new VersionInfo(32, ErrorCorrectionLevel.H, 30, 19, 15, 35, 16),
            new VersionInfo(33, ErrorCorrectionLevel.L, 30, 17, 115, 1, 116),
            new VersionInfo(33, ErrorCorrectionLevel.M, 28, 14, 46, 21, 47),
            new VersionInfo(33, ErrorCorrectionLevel.Q, 30, 29, 24, 19, 25),
            new VersionInfo(33, ErrorCorrectionLevel.H, 30, 11, 15, 46, 16),
            new VersionInfo(34, ErrorCorrectionLevel.L, 30, 13, 115, 6, 116),
            new VersionInfo(34, ErrorCorrectionLevel.M, 28, 14, 46, 23, 47),
            new VersionInfo(34, ErrorCorrectionLevel.Q, 30, 44, 24, 7, 25),
            new VersionInfo(34, ErrorCorrectionLevel.H, 30, 59, 16, 1, 17),
            new VersionInfo(35, ErrorCorrectionLevel.L, 30, 12, 121, 7, 122),
            new VersionInfo(35, ErrorCorrectionLevel.M, 28, 12, 47, 26, 48),
            new VersionInfo(35, ErrorCorrectionLevel.Q, 30, 39, 24, 14, 25),
            new VersionInfo(35, ErrorCorrectionLevel.H, 30, 22, 15, 41, 16),
            new VersionInfo(36, ErrorCorrectionLevel.L, 30, 6, 121, 14, 122),
            new VersionInfo(36, ErrorCorrectionLevel.M, 28, 6, 47, 34, 48),
            new VersionInfo(36, ErrorCorrectionLevel.Q, 30, 46, 24, 10, 25),
            new VersionInfo(36, ErrorCorrectionLevel.H, 30, 2, 15, 64, 16),
            new VersionInfo(37, ErrorCorrectionLevel.L, 30, 17, 122, 4, 123),
            new VersionInfo(37, ErrorCorrectionLevel.M, 28, 29, 46, 14, 47),
            new VersionInfo(37, ErrorCorrectionLevel.Q, 30, 49, 24, 10, 25),
            new VersionInfo(37, ErrorCorrectionLevel.H, 30, 24, 15, 46, 16),
            new VersionInfo(38, ErrorCorrectionLevel.L, 30, 4, 122, 18, 123),
            new VersionInfo(38, ErrorCorrectionLevel.M, 28, 13, 46, 32, 47),
            new VersionInfo(38, ErrorCorrectionLevel.Q, 30, 48, 24, 14, 25),
            new VersionInfo(38, ErrorCorrectionLevel.H, 30, 42, 15, 32, 16),
            new VersionInfo(39, ErrorCorrectionLevel.L, 30, 20, 117, 4, 118),
            new VersionInfo(39, ErrorCorrectionLevel.M, 28, 40, 47, 7, 48),
            new VersionInfo(39, ErrorCorrectionLevel.Q, 30, 43, 24, 22, 25),
            new VersionInfo(39, ErrorCorrectionLevel.H, 30, 10, 15, 67, 16),
            new VersionInfo(40, ErrorCorrectionLevel.L, 30, 19, 118, 6, 119),
            new VersionInfo(40, ErrorCorrectionLevel.M, 28, 18, 47, 31, 48),
            new VersionInfo(40, ErrorCorrectionLevel.Q, 30, 34, 24, 34, 25),
            new VersionInfo(40, ErrorCorrectionLevel.H, 30, 20, 15, 61, 16),
        };
    }
}
