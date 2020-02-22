using Barcoder.Utils;

namespace Barcoder.Aztec
{
    internal interface IToken
    {
        IToken PreviousToken { get; }

        void AppendTo(ref BitList bits, byte[] text);
    }

    internal struct SimpleToken : IToken
    {
        private readonly uint _value;
        private readonly byte _bitCount;

        public SimpleToken(IToken prev, uint value, byte bitCount)
        {
            PreviousToken = prev;
            _value = value;
            _bitCount = bitCount;
        }

        public IToken PreviousToken { get; }

        public void AppendTo(ref BitList bits, byte[] text)
            => bits.AddBits(_value, _bitCount);

        public override string ToString()
        {
            uint value = (uint)(_value & ((1 << _bitCount) - 1));
            value |= (uint)(1 << _bitCount);
            return $"<{value:X}>";
        }
    }

    internal struct BinaryShiftToken : IToken
    {
        private readonly int _shiftStart;
        private readonly int _shiftByteCount;

        public BinaryShiftToken(IToken prev, int shiftStart, int shiftByteCount)
        {
            PreviousToken = prev;
            _shiftStart = shiftStart;
            _shiftByteCount = shiftByteCount;
        }

        public IToken PreviousToken { get; }

        public void AppendTo(ref BitList bits, byte[] text)
        {
            for (var i = 0; i < _shiftByteCount; i++)
            {
                if (i == 0 || (i == 31 && _shiftByteCount <= 62))
                {
                    // We need a header before the first character, and before
                    // character 31 when the total byte code is <= 62
                    bits.AddBits(31, 5); // BINARY_SHIFT

                    if (_shiftByteCount > 62)
                    {
                        bits.AddBits((uint)_shiftByteCount - 31, 16);
                    }
                    else if (i == 0)
                    {
                        // 1 <= binaryShiftByteCode <= 62
                        if (_shiftByteCount < 31)
                        {
                            bits.AddBits((uint)_shiftByteCount, 5);
                        }
                        else
                        {
                            bits.AddBits(31, 5);
                        }
                    }
                    else
                    {
                        // 32 <= binaryShiftCount <= 62 and i == 31
                        bits.AddBits((uint)_shiftByteCount - 31, 5);
                    }
                }
                bits.AddByte(text[_shiftStart + i]);
            }
        }

        public override string ToString()
            => $"<{_shiftStart}::{_shiftStart + _shiftByteCount - 1}>";
    }
}
