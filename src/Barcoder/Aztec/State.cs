using System.Collections.Generic;
using System.Linq;
using Barcoder.Utils;

namespace Barcoder.Aztec
{
    internal struct State
    {
        private static readonly IReadOnlyDictionary<EncodingMode, IReadOnlyDictionary<EncodingMode, uint>> LatchTable = new Dictionary<EncodingMode, IReadOnlyDictionary<EncodingMode, uint>>
        {
            {
                EncodingMode.Upper,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Upper, 0 },
                    { EncodingMode.Lower, (5 << 16) + 28 },
                    { EncodingMode.Digit, (5 << 16) + 30 },
                    { EncodingMode.Mixed, (5 << 16) + 29 },
                    { EncodingMode.Punct, (10 << 16) + (29 << 5) + 30 },
                }
            },
            {
                EncodingMode.Lower,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Upper, (9 << 16) + (30 << 4) + 14 },
                    { EncodingMode.Lower, 0 },
                    { EncodingMode.Digit, (5 << 16) + 30 },
                    { EncodingMode.Mixed, (5 << 16) + 29 },
                    { EncodingMode.Punct, (10 << 16) + (29 << 5) + 30 },
                }
            },
            {
                EncodingMode.Digit,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Upper, (4 << 16) + 14 },
                    { EncodingMode.Lower, (9 << 16) + (14 << 5) + 28 },
                    { EncodingMode.Digit, 0 },
                    { EncodingMode.Mixed, (9 << 16) + (14 << 5) + 29 },
                    { EncodingMode.Punct, (14 << 16) + (14 << 10) + (29 << 5) + 30 },
                }
            },
            {
                EncodingMode.Mixed,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Upper, (5 << 16) + 29 },
                    { EncodingMode.Lower, (5 << 16) + 28 },
                    { EncodingMode.Digit, (10 << 16) + (29 << 5) + 30 },
                    { EncodingMode.Mixed, 0 },
                    { EncodingMode.Punct, (5 << 16) + 30 },
                }
            },
            {
                EncodingMode.Punct,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Upper, (5 << 16) + 31 },
                    { EncodingMode.Lower, (10 << 16) + (31 << 5) + 28 },
                    { EncodingMode.Digit, (10 << 16) + (31 << 5) + 30 },
                    { EncodingMode.Mixed, (10 << 16) + (31 << 5) + 29 },
                    { EncodingMode.Punct, 0 },
                }
            },
        };

        public static readonly IReadOnlyDictionary<EncodingMode, IReadOnlyDictionary<EncodingMode, uint>> ShiftTable = new Dictionary<EncodingMode, IReadOnlyDictionary<EncodingMode, uint>>
        {
            {
                EncodingMode.Upper,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Punct, 0 },
                }
            },
            {
                EncodingMode.Lower,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Punct, 0 },
                    { EncodingMode.Upper, 28 },
                }
            },
            {
                EncodingMode.Mixed,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Punct, 0 },
                }
            },
            {
                EncodingMode.Digit,
                new Dictionary<EncodingMode, uint>
                {
                    { EncodingMode.Punct, 0 },
                    { EncodingMode.Upper, 15 },
                }
            },
        };

        public static readonly IReadOnlyDictionary<EncodingMode, uint[]> CharMap = new Dictionary<EncodingMode, uint[]>
        {
            {
                EncodingMode.Upper, new uint[256]
            },
            {
                EncodingMode.Lower, new uint[256]
            },
            {
                EncodingMode.Digit, new uint[256]
            },
            {
                EncodingMode.Mixed, new uint[256]
            },
            {
                EncodingMode.Punct, new uint[256]
            },
        };

        static State()
        {
            CharMap[EncodingMode.Upper][' '] = 1;
            for (char c = 'A'; c <= 'Z'; c++) CharMap[EncodingMode.Upper][c] = (uint)(c - 'A' + 2);

            CharMap[EncodingMode.Lower][' '] = 1;
            for (char c = 'a'; c <= 'z'; c++) CharMap[EncodingMode.Lower][c] = (uint)(c - 'a' + 2);

            CharMap[EncodingMode.Digit][' '] = 1;
            for (char c = '0'; c <= '9'; c++) CharMap[EncodingMode.Digit][c] = (uint)(c - '0' + 2);
            CharMap[EncodingMode.Digit][','] = 12;
            CharMap[EncodingMode.Digit]['.'] = 13;

            int[] mixedTable = new[]
            {
                0, ' ', 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 27, 28, 29, 30, 31, '@', '\\', '^', '_', '`', '|', '~', 127,
            };
            for (uint i = 0; i < mixedTable.Length; i++)
            {
                int value = mixedTable[i];
                CharMap[EncodingMode.Mixed][value] = i;
            }

            int[] punctTable = new[]
            {
                0, '\r', 0, 0, 0, 0, '!', '\'', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
                ':', ';', '<', '=', '>', '?', '[', ']', '{', '}',
            };
            for (uint i = 0; i < punctTable.Length; i++)
            {
                int value = punctTable[i];
                if (value > 0) CharMap[EncodingMode.Punct][value] = i;
            }
        }

        public State(EncodingMode mode, IToken tokens, int shiftByteCount, int bitCount)
        {
            Mode = mode;
            Tokens = tokens;
            ShiftByteCount = shiftByteCount;
            BitCount = bitCount;
        }

        public static State InitialState => new State(EncodingMode.Upper, null, 0, 0);

        public EncodingMode Mode { get; }

        public IToken Tokens { get; }

        public int ShiftByteCount { get; }

        public int BitCount { get; }

        // Create a new state representing this state with a latch to a (not
        // necessary different) mode, and then a code.
        public State LatchAndAppend(EncodingMode mode, uint value)
        {
            int bitCount = BitCount;
            IToken tokens = Tokens;

            if (mode != Mode)
            {
                uint latch = LatchTable[Mode][mode];
                tokens = new SimpleToken(tokens, latch & 0xFFFF, (byte)(latch >> 16));
                bitCount += (int)(latch >> 16);
            }

            tokens = new SimpleToken(tokens, value, mode.BitCount());

            return new State(mode, tokens, 0, bitCount + mode.BitCount());
        }

        // Create a new state representing this state, with a temporary shift
        // to a different mode to output a single value.
        public State ShiftAndAppend(EncodingMode mode, uint value)
        {
            IToken tokens = Tokens;

            // Shifts exist only to UPPER and PUNCT, both with tokens size 5.
            tokens = new SimpleToken(tokens, ShiftTable[Mode][mode], Mode.BitCount());
            tokens = new SimpleToken(tokens, value, 5);

            return new State(Mode, tokens, 0, BitCount + Mode.BitCount() + 5);
        }

        // Create a new state representing this state, but an additional character
        // output in Binary Shift mode.
        public State AddBinaryShiftChar(int index)
        {
            IToken tokens = Tokens;
            EncodingMode mode = Mode;
            int bitCount = BitCount;

            if (Mode == EncodingMode.Punct || Mode == EncodingMode.Digit)
            {
                var latch = LatchTable[Mode][EncodingMode.Upper];
                tokens = new SimpleToken(tokens, latch & 0xFFFF, (byte)(latch >> 16));
                bitCount += (int)(latch >> 16);
                mode = EncodingMode.Upper;
            }

            int deltaBitCount = 8;
            if (ShiftByteCount == 0 || ShiftByteCount == 31)
                deltaBitCount = 18;
            else if (ShiftByteCount == 62)
                deltaBitCount = 9;

            var result = new State(mode, tokens, ShiftByteCount + 1, bitCount + deltaBitCount);
            if (result.ShiftByteCount == 2047 + 31)
            {
                // The string is as long as it's allowed to be.  We should end it.
                result = result.EndBinaryShift(index + 1);
            }

            return result;
        }

        // Create the state identical to this one, but we are no longer in
        // Binary Shift mode.
        public State EndBinaryShift(int index)
        {
            if (ShiftByteCount == 0)
                return this;

            IToken tokens = new BinaryShiftToken(Tokens, index - ShiftByteCount, ShiftByteCount);
            return new State(Mode, tokens, 0, BitCount);
        }

        // Returns true if "this" state is better (or equal) to be in than "that"
        // state under all possible circumstances.
        public bool IsBetterThanOrEqualTo(State other)
        {
            var mySize = BitCount + (LatchTable[Mode][other.Mode] >> 16);
            if (other.ShiftByteCount > 0 && (ShiftByteCount == 0 || ShiftByteCount > other.ShiftByteCount))
                mySize += 10; // Cost of entering Binary Shift mode.
            return mySize <= other.BitCount;
        }

        public BitList ToBitList(byte[] data)
        {
            var tokens = new Stack<IToken>();
            State endState = EndBinaryShift(data.Length);

            for (IToken token = endState.Tokens; token != null; token = token.PreviousToken)
                tokens.Push(token);

            var result = new BitList();
            while (tokens.Any())
            {
                IToken token = tokens.Pop();
                token.AppendTo(ref result, data);
            }

            return result;
        }
    }
}
