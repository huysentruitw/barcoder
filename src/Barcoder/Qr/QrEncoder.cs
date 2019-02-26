using System;
using System.Collections.Generic;
using System.Linq;
using Barcoder.Qr.InternalEncoders;
using Barcoder.Utils;

namespace Barcoder.Qr
{
    public static class QrEncoder
    {
        public static IBarcode Encode(string content, ErrorCorrectionLevel errorCorrectionLevel, Encoding encoding)
        {
            InternalEncoderBase encoder = GetEncoder(encoding);
            (BitList bits, VersionInfo versionInfo) = encoder.Encode(content, errorCorrectionLevel);

            var blocks = BlockList.SplitToBlocks(new Queue<byte>(bits.IterateBytes()), versionInfo);
            byte[] data = blocks.Interleave(versionInfo);
            QrCode result = Render(data, versionInfo);
            result.Content = content;
            return result;
        }

        private static InternalEncoderBase GetEncoder(Encoding encoding)
        {
            switch (encoding)
            {
            case Encoding.Auto:
                return new AutoEncoder();
            case Encoding.Numeric:
                return new NumericEncoder();
            case Encoding.AlphaNumeric:
                return new AlphaNumericEncoder();
            case Encoding.Unicode:
                return new UnicodeEncoder();
            default:
                throw new InvalidOperationException($"Unknown encoding type {encoding}");
            }
        }

        private static QrCode Render(byte[] data, VersionInfo versionInfo)
        {
            int dimension = versionInfo.ModulWidth();

            QrCode[] results = Enumerable.Range(0, 8).Select(_ => new QrCode(dimension)).ToArray();

            QrCode occupied = new QrCode(dimension);

            void setAll(int x, int y, bool value)
            {
                occupied.Set(x, y, true);
                foreach (QrCode result in results)
                    result.Set(x, y, value);
            }

            DrawFinderPatterns(versionInfo, setAll);
            DrawAlignmentPatterns(occupied, versionInfo, setAll);

            // Timing pattern
            for (int i = 0; i < dimension; i++)
            {
                if (!occupied.Get(i, 6))
                    setAll(i, 6, i % 2 == 0);
                if (!occupied.Get(6, i))
                    setAll(6, i, i % 2 == 0);
            }

            // Dark module
            setAll(8, dimension - 8, true);

            DrawVersionInfo(versionInfo, setAll);
            DrawFormatInfo(versionInfo, -1, occupied.Set);
            for (int i = 0; i < results.Length; i++)
                DrawFormatInfo(versionInfo, i, results[i].Set);

            // Write the data
            int curBitNo = 0;
            foreach ((int X, int Y) pos in IterateModules(occupied))
            {
                bool curBit = curBitNo < data.Length * 8
                    ? ((data[curBitNo / 8] >> (7 - (curBitNo % 8))) & 1) == 1
                    : false;

                for (int i = 0; i < results.Length; i++)
                    SetMasked(pos.X, pos.Y, curBit, i, results[i].Set);

                curBitNo++;
            }

            uint lowestPenalty = uint.MaxValue;
            int lowestPenaltyIndex = -1;
            for (int i = 0; i < results.Length; i++)
            {
                uint penalty = results[i].CalculatePenalty();
                if (penalty < lowestPenalty)
                {
                    lowestPenalty = penalty;
                    lowestPenaltyIndex = i;
                }
            }
            return results[lowestPenaltyIndex];
        }

        private static void DrawFinderPatterns(VersionInfo versionInfo, Action<int, int, bool> set)
        {
            int dim = versionInfo.ModulWidth();
            void drawPattern(int xoff, int yoff)
            {
                for (int x = -1; x < 8; x++)
                {
                    for (int y = -1; y < 8; y++)
                    {
                        var val = (x == 0 || x == 6 || y == 0 || y == 6 || (x > 1 && x < 5 && y > 1 && y < 5)) && (x <= 6 && y <= 6 && x >= 0 && y >= 0);
                        if (x + xoff >= 0 && x + xoff < dim && y + yoff >= 0 && y + yoff < dim)
                            set(x + xoff, y + yoff, val);
                    }
                }
            }
            drawPattern(0, 0);
            drawPattern(0, dim - 7);
            drawPattern(dim - 7, 0);
        }

        private static void DrawAlignmentPatterns(QrCode occupied, VersionInfo versionInfo, Action<int, int, bool> set)
        {
            void drawPattern(int xoff, int yoff)
            {
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        var val = x == -2 || x == 2 || y == -2 || y == 2 || (x == 0 && y == 0);
                        set(x + xoff, y + yoff, val);
                    }
                }
            }
            int[] positions = versionInfo.AlignmentPatternPlacements();
            foreach (int x in positions)
            {
                foreach (int y in positions)
                {
                    if (occupied.Get(x, y))
                        continue;
                    drawPattern(x, y);
                }
            }
        }

        private static readonly Dictionary<byte, bool[]> VersionInfoBitsByVersion = new Dictionary<byte, bool[]>
        {
            { 7, new [] {false, false, false, true, true, true, true, true, false, false, true, false, false, true, false, true, false, false} },
            { 8, new [] {false, false, true, false, false, false, false, true, false, true, true, false, true, true, true, true, false, false} },
            { 9, new [] {false, false, true, false, false, true, true, false, true, false, true, false, false, true, true, false, false, true} },
            { 10, new [] {false, false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, true} },
            { 11, new [] {false, false, true, false, true, true, true, false, true, true, true, true, true, true, false, true, true, false} },
            { 12, new [] {false, false, true, true, false, false, false, true, true, true, false, true, true, false, false, false, true, false} },
            { 13, new [] {false, false, true, true, false, true, true, false, false, false, false, true, false, false, false, true, true, true} },
            { 14, new [] {false, false, true, true, true, false, false, true, true, false, false, false, false, false, true, true, false, true} },
            { 15, new [] {false, false, true, true, true, true, true, false, false, true, false, false, true, false, true, false, false, false} },
            { 16, new [] {false, true, false, false, false, false, true, false, true, true, false, true, true, true, true, false, false, false} },
            { 17, new [] {false, true, false, false, false, true, false, true, false, false, false, true, false, true, true, true, false, true} },
            { 18, new [] {false, true, false, false, true, false, true, false, true, false, false, false, false, true, false, true, true, true} },
            { 19, new [] {false, true, false, false, true, true, false, true, false, true, false, false, true, true, false, false, true, false} },
            { 20, new [] {false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, true, false} },
            { 21, new [] {false, true, false, true, false, true, false, true, true, false, true, false, false, false, false, false, true, true} },
            { 22, new [] {false, true, false, true, true, false, true, false, false, false, true, true, false, false, true, false, false, true} },
            { 23, new [] {false, true, false, true, true, true, false, true, true, true, true, true, true, false, true, true, false, false} },
            { 24, new [] {false, true, true, false, false, false, true, true, true, false, true, true, false, false, false, true, false, false} },
            { 25, new [] {false, true, true, false, false, true, false, false, false, true, true, true, true, false, false, false, false, true} },
            { 26, new [] {false, true, true, false, true, false, true, true, true, true, true, false, true, false, true, false, true, true} },
            { 27, new [] {false, true, true, false, true, true, false, false, false, false, true, false, false, false, true, true, true, false} },
            { 28, new [] {false, true, true, true, false, false, true, true, false, false, false, false, false, true, true, false, true, false} },
            { 29, new [] {false, true, true, true, false, true, false, false, true, true, false, false, true, true, true, true, true, true} },
            { 30, new [] {false, true, true, true, true, false, true, true, false, true, false, true, true, true, false, true, false, true} },
            { 31, new [] {false, true, true, true, true, true, false, false, true, false, false, true, false, true, false, false, false, false} },
            { 32, new [] {true, false, false, false, false, false, true, false, false, true, true, true, false, true, false, true, false, true} },
            { 33, new [] {true, false, false, false, false, true, false, true, true, false, true, true, true, true, false, false, false, false} },
            { 34, new [] {true, false, false, false, true, false, true, false, false, false, true, false, true, true, true, false, true, false} },
            { 35, new [] {true, false, false, false, true, true, false, true, true, true, true, false, false, true, true, true, true, true} },
            { 36, new [] {true, false, false, true, false, false, true, false, true, true, false, false, false, false, true, false, true, true} },
            { 37, new [] {true, false, false, true, false, true, false, true, false, false, false, false, true, false, true, true, true, false} },
            { 38, new [] {true, false, false, true, true, false, true, false, true, false, false, true, true, false, false, true, false, false} },
            { 39, new [] {true, false, false, true, true, true, false, true, false, true, false, true, false, false, false, false, false, true} },
            { 40, new [] {true, false, true, false, false, false, true, true, false, false, false, true, true, false, true, false, false, true} },
        };

        private static void DrawVersionInfo(VersionInfo versionInfo, Action<int, int, bool> set)
        {
            if (VersionInfoBitsByVersion.TryGetValue(versionInfo.Version, out bool[] versionInfoBits))
            {
                for (int i = 0; i < versionInfoBits.Length; i++)
                {
                    int x = (versionInfo.ModulWidth() - 11) + i % 3;
                    int y = i / 3;
                    set(x, y, versionInfoBits[versionInfoBits.Length - i - 1]);
                    set(y, x, versionInfoBits[versionInfoBits.Length - i - 1]);
                }
            }
        }

        private static readonly Dictionary<ErrorCorrectionLevel, Dictionary<int, bool[]>> FormatInfos = new Dictionary<ErrorCorrectionLevel, Dictionary<int, bool[]>>
        {
            { ErrorCorrectionLevel.L, new Dictionary<int, bool[]> {
                { 0, new [] {true, true, true, false, true, true, true, true, true, false, false, false, true, false, false} },
                { 1, new [] {true, true, true, false, false, true, false, true, true, true, true, false, false, true, true} },
                { 2, new [] {true, true, true, true, true, false, true, true, false, true, false, true, false, true, false} },
                { 3, new [] {true, true, true, true, false, false, false, true, false, false, true, true, true, false, true} },
                { 4, new [] {true, true, false, false, true, true, false, false, false, true, false, true, true, true, true} },
                { 5, new [] {true, true, false, false, false, true, true, false, false, false, true, true, false, false, false} },
                { 6, new [] {true, true, false, true, true, false, false, false, true, false, false, false, false, false, true} },
                { 7, new [] {true, true, false, true, false, false, true, false, true, true, true, false, true, true, false} },
            } },
            { ErrorCorrectionLevel.M, new Dictionary<int, bool[]> {
                { 0, new [] {true, false, true, false, true, false, false, false, false, false, true, false, false, true, false} },
                { 1, new [] {true, false, true, false, false, false, true, false, false, true, false, false, true, false, true} },
                { 2, new [] {true, false, true, true, true, true, false, false, true, true, true, true, true, false, false} },
                { 3, new [] {true, false, true, true, false, true, true, false, true, false, false, true, false, true, true} },
                { 4, new [] {true, false, false, false, true, false, true, true, true, true, true, true, false, false, true} },
                { 5, new [] {true, false, false, false, false, false, false, true, true, false, false, true, true, true, false} },
                { 6, new [] {true, false, false, true, true, true, true, true, false, false, true, false, true, true, true} },
                { 7, new [] {true, false, false, true, false, true, false, true, false, true, false, false, false, false, false} },
            } },
            { ErrorCorrectionLevel.Q, new Dictionary<int, bool[]> {
                { 0, new [] {false, true, true, false, true, false, true, false, true, false, true, true, true, true, true} },
                { 1, new [] {false, true, true, false, false, false, false, false, true, true, false, true, false, false, false} },
                { 2, new [] {false, true, true, true, true, true, true, false, false, true, true, false, false, false, true} },
                { 3, new [] {false, true, true, true, false, true, false, false, false, false, false, false, true, true, false} },
                { 4, new [] {false, true, false, false, true, false, false, true, false, true, true, false, true, false, false} },
                { 5, new [] {false, true, false, false, false, false, true, true, false, false, false, false, false, true, true} },
                { 6, new [] {false, true, false, true, true, true, false, true, true, false, true, true, false, true, false} },
                { 7, new [] {false, true, false, true, false, true, true, true, true, true, false, true, true, false, true} },
            } },
            { ErrorCorrectionLevel.H, new Dictionary<int, bool[]> {
                { 0, new [] {false, false, true, false, true, true, false, true, false, false, false, true, false, false, true} },
                { 1, new [] {false, false, true, false, false, true, true, true, false, true, true, true, true, true, false} },
                { 2, new [] {false, false, true, true, true, false, false, true, true, true, false, false, true, true, true} },
                { 3, new [] {false, false, true, true, false, false, true, true, true, false, true, false, false, false, false} },
                { 4, new [] {false, false, false, false, true, true, true, false, true, true, false, false, false, true, false} },
                { 5, new [] {false, false, false, false, false, true, false, false, true, false, true, false, true, false, true} },
                { 6, new [] {false, false, false, true, true, false, true, false, false, false, false, true, true, false, false} },
                { 7, new [] {false, false, false, true, false, false, false, false, false, true, true, true, false, true, true} },
            } },
        };

        private static void DrawFormatInfo(VersionInfo versionInfo, int usedMask, Action<int, int, bool> set)
        {
            bool[] formatInfo = usedMask == -1
                ? new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true } // Set all to true cause -1 --> occupied mask.
                : FormatInfos[versionInfo.ErrorCorrectionLevel][usedMask];

            if (formatInfo.Length == 15)
            {
                int dim = versionInfo.ModulWidth();
                set(0, 8, formatInfo[0]);
                set(1, 8, formatInfo[1]);
                set(2, 8, formatInfo[2]);
                set(3, 8, formatInfo[3]);
                set(4, 8, formatInfo[4]);
                set(5, 8, formatInfo[5]);
                set(7, 8, formatInfo[6]);
                set(8, 8, formatInfo[7]);
                set(8, 7, formatInfo[8]);
                set(8, 5, formatInfo[9]);
                set(8, 4, formatInfo[10]);
                set(8, 3, formatInfo[11]);
                set(8, 2, formatInfo[12]);
                set(8, 1, formatInfo[13]);
                set(8, 0, formatInfo[14]);

                set(8, dim - 1, formatInfo[0]);
                set(8, dim - 2, formatInfo[1]);
                set(8, dim - 3, formatInfo[2]);
                set(8, dim - 4, formatInfo[3]);
                set(8, dim - 5, formatInfo[4]);
                set(8, dim - 6, formatInfo[5]);
                set(8, dim - 7, formatInfo[6]);
                set(dim - 8, 8, formatInfo[7]);
                set(dim - 7, 8, formatInfo[8]);
                set(dim - 6, 8, formatInfo[9]);
                set(dim - 5, 8, formatInfo[10]);
                set(dim - 4, 8, formatInfo[11]);
                set(dim - 3, 8, formatInfo[12]);
                set(dim - 2, 8, formatInfo[13]);
                set(dim - 1, 8, formatInfo[14]);
            }
        }

        private static IEnumerable<(int X, int Y)> IterateModules(QrCode occupied)
        {
            foreach ((int X, int Y) pt in IterateAllPoints(occupied))
                if (!occupied.Get(pt.X, pt.Y))
                    yield return pt;
        }

        private static IEnumerable<(int X, int Y)> IterateAllPoints(QrCode occupied)
        {
            int curX = occupied.Dimension - 1;
            int curY = occupied.Dimension - 1;
            bool isUpward = true;
            while (true)
            {
                yield return (curX, curY);
                yield return (curX - 1, curY);
                if (isUpward)
                {
                    curY--;
                    if (curY < 0)
                    {
                        curY = 0;
                        curX -= 2;
                        if (curX == 6)
                            curX--;
                        if (curX < 0)
                            break;
                        isUpward = false;
                    }
                }
                else
                {
                    curY++;
                    if (curY >= occupied.Dimension)
                    {
                        curY = occupied.Dimension - 1;
                        curX -= 2;
                        if (curX == 6)
                            curX--;
                        if (curX < 0)
                            break;
                        isUpward = true;
                    }
                }
            }
        }

        private static void SetMasked(int x, int y, bool value, int mask, Action<int, int, bool> set)
        {
            switch (mask)
            {
            case 0:
                value = value != (((y + x) % 2) == 0);
                break;
            case 1:
                value = value != ((y % 2) == 0);
                break;
            case 2:
                value = value != ((x % 3) == 0);
                break;
            case 3:
                value = value != (((y + x) % 3) == 0);
                break;
            case 4:
                value = value != (((y / 2 + x / 3) % 2) == 0);
                break;
            case 5:
                value = value != (((y * x) % 2) + ((y * x) % 3) == 0);
                break;
            case 6:
                value = value != ((((y * x) % 2) + ((y * x) % 3)) % 2 == 0);
                break;
            case 7:
                value = value != ((((y + x) % 2) + ((y * x) % 3)) % 2 == 0);
                break;
            }
            set(x, y, value);
        }
    }
}
