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
    }
}
