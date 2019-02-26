using System;
using Barcoder.Utils;

namespace Barcoder.Qr
{
    public sealed class QrCode : IBarcode
    {
        private readonly BitList _data;

        internal QrCode(int dimension)
        {
            Dimension = dimension;
            Bounds = new Bounds(Dimension, Dimension);
            Metadata = new Metadata(BarcodeType.QR, 2);
            _data = new BitList(Dimension * Dimension);
        }

        internal void Set(int x, int y, bool value)
            => _data.SetBit(x * Dimension + y, value);

        internal bool Get(int x, int y)
            => _data.GetBit(x * Dimension + y);

        internal uint CalculatePenalty()
            => CalculatePenaltyRule1() + CalculatePenaltyRule2() + CalculatePenaltyRule3() + CalculatePenaltyRule4();

        internal uint CalculatePenaltyRule1()
        {
            uint result = 0;
            for (int x = 0; x < Dimension; x++)
            {
                bool checkForX = false;
                uint cntX = 0;
                bool checkForY = false;
                uint cntY = 0;
                for (int y = 0; y < Dimension; y++)
                {
                    if (Get(x, y) == checkForX)
                    {
                        cntX++;
                    }
                    else
                    {
                        checkForX = !checkForX;
                        if (cntX >= 5)
                        {
                            result += cntX - 2;
                        }
                        cntX = 1;
                    }
                    if (Get(y, x) == checkForY)
                    {
                        cntY++;
                    }
                    else
                    {
                        checkForY = !checkForY;
                        if (cntY >= 5)
                        {
                            result += cntY - 2;
                        }
                        cntY = 1;
                    }
                }
                if (cntX >= 5)
                {
                    result += cntX - 2;
                }
                if (cntY >= 5)
                {
                    result += cntY - 2;
                }
            }
            return result;
        }

        internal uint CalculatePenaltyRule2()
        {
            uint result = 0;
            for (int x = 0; x < Dimension - 1; x++)
            {
                for (int y = 0; y < Dimension - 1; y++)
                {
                    bool check = Get(x, y);
                    if (Get(x, y + 1) == check && Get(x + 1, y) == check && Get(x + 1, y + 1) == check)
                        result += 3;
                }
            }
            return result;
        }

        internal uint CalculatePenaltyRule3()
        {
            var pattern1 = new bool[] { true, false, true, true, true, false, true, false, false, false, false };
            var pattern2 = new bool[] { false, false, false, false, true, false, true, true, true, false, true };
            uint result = 0;
            for (int x = 0; x <= Dimension - pattern1.Length; x++)
            {
                for (int y = 0; y < Dimension; y++)
                {
                    bool pattern1XFound = true;
                    bool pattern2XFound = true;
                    bool pattern1YFound = true;
                    bool pattern2YFound = true;
                    for (int i = 0; i < pattern1.Length; i++)
                    {
                        bool iv = Get(x + i, y);
                        if (iv != pattern1[i])
                            pattern1XFound = false;
                        if (iv != pattern2[i])
                            pattern2XFound = false;
                        iv = Get(y, x + i);
                        if (iv != pattern1[i])
                            pattern1YFound = false;
                        if (iv != pattern2[i])
                            pattern2YFound = false;
                    }
                    if (pattern1XFound || pattern2XFound)
                        result += 40;
                    if (pattern1YFound || pattern2YFound)
                        result += 40;
                }
            }
            return result;
        }

        internal uint CalculatePenaltyRule4()
        {
            int totalNum = _data.Length;
            int trueCnt = 0;
            for (int i = 0; i < totalNum; i++)
                if (_data.GetBit(i))
                    trueCnt++;
            double percDark = (double)trueCnt * 100 / (double)totalNum;
            double floor = Math.Abs(Math.Floor(percDark / 5) - 10);
            double ceil = Math.Abs(Math.Ceiling(percDark / 5) - 10);
            return (uint)(Math.Min(floor, ceil) * 10);
        }

        internal int Dimension { get; }

        public string Content { get; internal set; }

        public Bounds Bounds { get; }

        public int Margin => 5;

        public Metadata Metadata { get; }

        public bool At(int x, int y) => Get(x, y);
    }
}
