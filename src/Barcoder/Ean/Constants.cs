using System.Collections.Generic;

namespace Barcoder.Ean
{
    internal static class Constants
    {
        public static readonly IReadOnlyDictionary<char, EncodedNumber> EncodingTable = new Dictionary<char, EncodedNumber>
        {
            { '0', new EncodedNumber
                {
                    LeftOdd  = new[] {false, false, false, true, true, false, true},
                    LeftEven = new[] {false, true, false, false, true, true, true},
                    Right    = new[] {true, true, true, false, false, true, false},
                    Checksum = new[] {false, false, false, false, false, false},
                }
            },
            { '1', new EncodedNumber
                {
                    LeftOdd  = new[] {false, false, true, true, false, false, true},
                    LeftEven = new[] {false, true, true, false, false, true, true},
                    Right    = new[] {true, true, false, false, true, true, false},
                    Checksum = new[] {false, false, true, false, true, true},
                }
            },
            { '2', new EncodedNumber
                {
                    LeftOdd  = new[] {false, false, true, false, false, true, true},
                    LeftEven = new[] {false, false, true, true, false, true, true},
                    Right    = new[] {true, true, false, true, true, false, false},
                    Checksum = new[] {false, false, true, true, false, true},
                }
            },
            { '3', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, true, true, true, false, true},
                    LeftEven = new[] {false, true, false, false, false, false, true},
                    Right    = new[] {true, false, false, false, false, true, false},
                    Checksum = new[] {false, false, true, true, true, false},
                }
            },
            { '4', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, false, false, false, true, true},
                    LeftEven = new[] {false, false, true, true, true, false, true},
                    Right    = new[] {true, false, true, true, true, false, false},
                    Checksum = new[] {false, true, false, false, true, true},
                }
            },
            { '5', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, true, false, false, false, true},
                    LeftEven = new[] {false, true, true, true, false, false, true},
                    Right    = new[] {true, false, false, true, true, true, false},
                    Checksum = new[] {false, true, true, false, false, true},
                }
            },
            { '6', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, false, true, true, true, true},
                    LeftEven = new[] {false, false, false, false, true, false, true},
                    Right    = new[] {true, false, true, false, false, false, false},
                    Checksum = new[] {false, true, true, true, false, false},
                }
            },
            { '7', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, true, true, false, true, true},
                    LeftEven = new[] {false, false, true, false, false, false, true},
                    Right    = new[] {true, false, false, false, true, false, false},
                    Checksum = new[] {false, true, false, true, false, true},
                }
            },
            { '8', new EncodedNumber
                {
                    LeftOdd  = new[] {false, true, true, false, true, true, true},
                    LeftEven = new[] {false, false, false, true, false, false, true},
                    Right    = new[] {true, false, false, true, false, false, false},
                    Checksum = new[] {false, true, false, true, true, false},
                }
            },
            { '9', new EncodedNumber
                {
                    LeftOdd  = new[] {false, false, false, true, false, true, true},
                    LeftEven = new[] {false, false, true, false, true, true, true},
                    Right    = new[] {true, true, true, false, true, false, false},
                    Checksum = new[] {false, true, true, false, true, false},
                }
            },
        };

        public struct EncodedNumber
        {
            public bool[] LeftOdd { get; set; }
            public bool[] LeftEven { get; set; }
            public bool[] Right { get; set; }
            public bool[] Checksum { get; set; }
        }

        public const int Margin = 10;
    }
}
