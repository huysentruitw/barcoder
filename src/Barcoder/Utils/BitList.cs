using System.Collections.Generic;
using System.Linq;

namespace Barcoder.Utils
{
    internal struct BitList
    {
#pragma warning disable IDE0032 // Use auto property
        private int _count;
#pragma warning restore IDE0032 // Use auto property
        private uint[] _data;

        /// <summary>
        /// Creates a <see cref="BitList"/> with the given bit capacity.
        /// All bits are initialized with false.
        /// </summary>
        /// <param name="capacity">The required capacity.</param>
        public BitList(int capacity)
        {
            _count = capacity;
            var x = 0;
            if ((capacity % 32) != 0)
                x = 1;
            _data = new uint[capacity / 32 + x];
        }

        /// <summary>
        /// Returns the number of contained bits.
        /// </summary>
        public int Length => _count;

        /// <summary>
        /// Appends the given bits to the end of the list.
        /// </summary>
        /// <param name="bits">The bits to append.</param>
        public void AddBit(params bool[] bits)
        {
            foreach (bool bit in bits)
            {
                var itemIndex = _count / 32;
                while (itemIndex >= (_data?.Length ?? 0))
                    Grow();
                SetBit(_count, bit);
                _count++;
            }
        }

        /// <summary>
        /// Sets the bit at the given index to the given value.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <param name="value">The given value.</param>
        public void SetBit(int index, bool value)
        {
            var itemIndex = index / 32;
            var itemBitShift = 31 - (index % 32);
            if (value)
                _data[itemIndex] |= (uint)1 << itemBitShift;
            else
                _data[itemIndex] &= ~((uint)1 << itemBitShift);
        }

        /// <summary>
        /// Returns the bit at the given index.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <returns>The bit.</returns>
        public bool GetBit(int index)
        {
            var itemIndex = index / 32;
            var itemBitShift = 31 - (index % 32);
            return ((_data[itemIndex] >> itemBitShift) & 1) == 1;
        }

        /// <summary>
        /// Appends all 8 bits of the given byte to the end of the list.
        /// MSb first.
        /// </summary>
        /// <param name="b">The byte containing the bits to add.</param>
        public void AddByte(byte b)
        {
            for (var i = 7; i >= 0; i--)
                AddBit(((b >> i) & 1) == 1);
        }

        /// <summary>
        /// Appends the last (LSB) count bits of b to the end of the list.
        /// </summary>
        /// <param name="b"></param>
        public void AddBits(uint b, byte count)
        {
            for (var i = count - 1; i >= 0; i--)
                AddBit(((b >> i) & 1) == 1);
        }

        /// <summary>
        /// Returns all bits of the <see cref="BitList"/> as a <see cref="byte[]"/>.
        /// </summary>
        /// <returns>A <see cref="byte[]"/>.</returns>
        public byte[] GetBytes()
        {
            var len = _count >> 3;
            if ((_count % 8) != 0)
                len++;
            var result = new byte[len];
            for (var i = 0; i < len; i++)
            {
                var shift = (3 - (i % 4)) * 8;
                result[i] = (byte)(_data[i / 4] >> shift);
            }
            return result;
        }

        /// <summary>
        /// Iterates through all bytes contained in the <see cref="BitList"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> IterateBytes()
        {
            var c = _count;
            var shift = 24;
            var i = 0;
            while (c > 0)
            {
                yield return (byte)(_data[i] >> shift);
                shift -= 8;
                if (shift < 0)
                {
                    shift = 24;
                    i++;
                }
                c -= 8;
            }
        }

        private void Grow()
        {
            var dataLength = _data?.Length ?? 0;
            var growBy = dataLength;
            if (growBy < 128)
                growBy = 128;
            else if (growBy >= 1024)
                growBy = 1024;

            var nd = new uint[dataLength + growBy];
            _data?.CopyTo(nd, 0);
            _data = nd;
        }

        public override string ToString()
        {
            BitList that = this;
            return new string(Enumerable.Range(0, Length).Select(i => that.GetBit(i) ? 'X' : '.').ToArray());
        }
    }
}
