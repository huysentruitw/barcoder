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

            Action<int, int, bool> setAll = (int x, int y, bool value) =>
            {
                occupied.Set(x, y, true);
                foreach (QrCode result in results)
                    result.Set(x, y, value);
            };

            DrawFinderPatterns(versionInfo, setAll);
            DrawAlignmentPatterns(occupied, versionInfo, setAll);

            // Timing pattern
            for (int i = 0; i < dimension; i++)
            {
                if (!occupied.At(i, 6))
                    setAll(i, 6, i % 2 == 0);
                else
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
            throw new NotImplementedException();
        }

        private static void DrawAlignmentPatterns(QrCode qrCode, VersionInfo versionInfo, Action<int, int, bool> set)
        {
            throw new NotImplementedException();
        }

        private static void DrawVersionInfo(VersionInfo versionInfo, Action<int, int, bool> set)
        {
            throw new NotImplementedException();
        }

        private static void DrawFormatInfo(VersionInfo versionInfo, int usedMask, Action<int, int, bool> set)
        {
            throw new NotImplementedException();
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
