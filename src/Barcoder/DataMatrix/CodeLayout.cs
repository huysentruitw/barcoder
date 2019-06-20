using System;
using Barcoder.Utils;

namespace Barcoder.DataMatrix
{
    internal sealed class CodeLayout
    {
        public CodeLayout(CodeSize size)
        {
            Size = size;
            Matrix = new BitList(size.MatrixColumns * size.MatrixRows);
            Occupy = new BitList(size.MatrixColumns * size.MatrixRows);
        }

        public BitList Matrix { get; }

        public BitList Occupy { get; }

        public CodeSize Size { get; }

        public bool Occupied(int row, int col) => Occupy.GetBit(col + row * Size.MatrixColumns);

        public void Set(int row, int col, byte value, byte bitNum)
        {
            if (row < 0)
            {
                row += Size.MatrixRows;
                col += 4 - ((Size.MatrixRows + 4) % 8);
            }

            if (col < 0)
            {
                col += Size.MatrixColumns;
                row += 4 - ((Size.MatrixColumns + 4) % 8);
            }

            if (Occupied(row, col))
                throw new InvalidOperationException($"Field already occupied row: {row} col: {col}");

            Occupy.SetBit(col + row * Size.MatrixColumns, true);

            bool val = ((value >> (7 - bitNum)) & 1) == 1;
            Matrix.SetBit(col + row * Size.MatrixColumns, val);
        }

        public void SetSimple(int row, int col, byte value)
        {
            Set(row - 2, col - 2, value, 0);
            Set(row - 2, col - 1, value, 1);
            Set(row - 1, col - 2, value, 2);
            Set(row - 1, col - 1, value, 3);
            Set(row - 1, col - 0, value, 4);
            Set(row - 0, col - 2, value, 5);
            Set(row - 0, col - 1, value, 6);
            Set(row - 0, col - 0, value, 7);
        }

        public void Corner1(byte value)
        {
            Set(Size.MatrixRows - 1, 0, value, 0);
            Set(Size.MatrixRows - 1, 1, value, 1);
            Set(Size.MatrixRows - 1, 2, value, 2);
            Set(0, Size.MatrixColumns - 2, value, 3);
            Set(0, Size.MatrixColumns - 1, value, 4);
            Set(1, Size.MatrixColumns - 1, value, 5);
            Set(2, Size.MatrixColumns - 1, value, 6);
            Set(3, Size.MatrixColumns - 1, value, 7);
        }

        public void Corner2(byte value)
        {
            Set(Size.MatrixRows - 3, 0, value, 0);
            Set(Size.MatrixRows - 2, 0, value, 1);
            Set(Size.MatrixRows - 1, 0, value, 2);
            Set(0, Size.MatrixColumns - 4, value, 3);
            Set(0, Size.MatrixColumns - 3, value, 4);
            Set(0, Size.MatrixColumns - 2, value, 5);
            Set(0, Size.MatrixColumns - 1, value, 6);
            Set(1, Size.MatrixColumns - 1, value, 7);
        }

        public void Corner3(byte value)
        {
            Set(Size.MatrixRows - 3, 0, value, 0);
            Set(Size.MatrixRows - 2, 0, value, 1);
            Set(Size.MatrixRows - 1, 0, value, 2);
            Set(0, Size.MatrixColumns - 2, value, 3);
            Set(0, Size.MatrixColumns - 1, value, 4);
            Set(1, Size.MatrixColumns - 1, value, 5);
            Set(2, Size.MatrixColumns - 1, value, 6);
            Set(3, Size.MatrixColumns - 1, value, 7);
        }

        public void Corner4(byte value)
        {
            Set(Size.MatrixRows - 1, 0, value, 0);
            Set(Size.MatrixRows - 1, Size.MatrixColumns - 1, value, 1);
            Set(0, Size.MatrixColumns - 3, value, 2);
            Set(0, Size.MatrixColumns - 2, value, 3);
            Set(0, Size.MatrixColumns - 1, value, 4);
            Set(1, Size.MatrixColumns - 3, value, 5);
            Set(1, Size.MatrixColumns - 2, value, 6);
            Set(1, Size.MatrixColumns - 1, value, 7);
        }

        public void SetValues(byte[] data)
        {
            var idx = 0;
            var row = 4;
            var col = 0;

            while (row < Size.MatrixRows || col < Size.MatrixColumns)
            {
                if (row == Size.MatrixRows && col == 0)
                {
                    Corner1(data[idx]);
                    idx++;
                }

                if (row == Size.MatrixRows - 2 && col == 0 && (Size.MatrixColumns % 4) != 0)
                {
                    Corner2(data[idx]);
                    idx++;
                }

                if (row == Size.MatrixRows - 2 && col == 0 && (Size.MatrixColumns % 8) == 4)
                {
                    Corner3(data[idx]);
                    idx++;
                }

                if (row == Size.MatrixRows + 4 && col == 2 && (Size.MatrixColumns % 8) == 0)
                {
                    Corner4(data[idx]);
                    idx++;
                }

                while (true)
                {
                    if (row < Size.MatrixRows && col >= 0 && !Occupied(row, col))
                    {
                        SetSimple(row, col, data[idx]);
                        idx++;
                    }

                    row -= 2;
                    col += 2;
        
                    if (row < 0 || col >= Size.MatrixColumns)
                        break;
                }

                row += 1;
                col += 3;

                while (true)
                {
                    if (row >= 0 && col < Size.MatrixColumns && !Occupied(row, col))
                    {
                        SetSimple(row, col, data[idx]);
                        idx++;
                    }

                    row += 2;
                    col -= 2;

                    if (row >= Size.MatrixRows || col < 0)
                        break;
                }

                row += 3;
                col += 1;
            }

            if (!Occupied(Size.MatrixRows - 1, Size.MatrixColumns - 1))
            {
                Set(Size.MatrixRows - 1, Size.MatrixColumns - 1, 255, 0);
                Set(Size.MatrixRows - 2, Size.MatrixColumns - 2, 255, 0);
            }
        }

        public DataMatrixCode Merge()
        {
            var result = new DataMatrixCode(Size);

            //dotted horizontal lines
            for (int r = 0; r < Size.Rows; r += (Size.RegionRows + 2))
            {
                for (int c = 0; c < Size.Columns; c += 2)
                {
                    result.Set(c, r, true);
                }
            }

            //solid horizontal line
            for (int r = Size.RegionRows + 1; r < Size.Rows; r += (Size.RegionRows + 2))
            {
                for (int c = 0; c < Size.Columns; c++)
                {
                    result.Set(c, r, true);
                }
            }

            //dotted vertical lines
            for (int c = Size.RegionColumns + 1; c < Size.Columns; c += (Size.RegionColumns + 2))
            {
                for (int r = 1; r < Size.Rows; r += 2)
                {
                    result.Set(c, r, true);
                }
            }

            //solid vertical line
            for (int c = 0; c < Size.Columns; c += (Size.RegionColumns + 2))
            {
                for (int r = 0; r < Size.Rows; r++)
                {
                    result.Set(c, r, true);
                }
            }

            var count = 0;

            for (int hRegion = 0; hRegion < Size.RegionCountHorizontal; hRegion++)
            {
                for (int vRegion = 0; vRegion < Size.RegionCountVertical; vRegion++)
                {
                    for (int x = 0; x < Size.RegionColumns; x++)
                    {
                        int colMatrix = (Size.RegionColumns * hRegion) + x;
                        int colResult = ((2 + Size.RegionColumns) * hRegion) + x + 1;

                        for (int y = 0; y < Size.RegionRows; y++)
                        {
                            int rowMatrix = (Size.RegionRows * vRegion) + y;
                            int rowResult = ((2 + Size.RegionRows) * vRegion) + y + 1;

                            bool val = Matrix.GetBit(colMatrix + rowMatrix * Size.MatrixColumns);

                            if (val)
                            {
                                count++;
                            }

                            result.Set(colResult, rowResult, val);
                        }
                    }
                }
            }

            return result;
        }
    }
}
