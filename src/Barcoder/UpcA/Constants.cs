using System.Collections.Generic;

namespace Barcoder.UpcA
{
    internal static class Constants
    {
        public static readonly IReadOnlyDictionary<char, EncodedNumber> EncodingTable = new Dictionary<char, EncodedNumber>
        {
            { '0', new EncodedNumber
                {
                    Left  = new[] {false, false, false, true, true, false, true},
                    Right = new[] {true, true, true, false, false, true, false},
                }
            },
            { '1', new EncodedNumber
                {
                    Left  = new[] {false, false, true, true, false, false, true},
                    Right = new[] {true, true, false, false, true, true, false},
                }
            },
            { '2', new EncodedNumber
                {
                    Left  = new[] {false, false, true, false, false, true, true},
                    Right = new[] {true, true, false, true, true, false, false},
                }
            },
            { '3', new EncodedNumber
                {
                    Left  = new[] {false, true, true, true, true, false, true},
                    Right = new[] {true, false, false, false, false, true, false},
                }
            },
            { '4', new EncodedNumber
                {
                    Left  = new[] {false, true, false, false, false, true, true},
                    Right = new[] {true, false, true, true, true, false, false},
                }
            },
            { '5', new EncodedNumber
                {
                    Left  = new[] {false, true, true, false, false, false, true},
                    Right = new[] {true, false, false, true, true, true, false},
                }
            },
            { '6', new EncodedNumber
                {
                    Left  = new[] {false, true, false, true, true, true, true},
                    Right = new[] {true, false, true, false, false, false, false},
                }
            },
            { '7', new EncodedNumber
                {
                    Left  = new[] {false, true, true, true, false, true, true},
                    Right = new[] {true, false, false, false, true, false, false},
                }
            },
            { '8', new EncodedNumber
                {
                    Left  = new[] {false, true, true, false, true, true, true},
                    Right = new[] {true, false, false, true, false, false, false},
                }
            },
            { '9', new EncodedNumber
                {
                    Left  = new[] {false, false, false, true, false, true, true},
                    Right = new[] {true, true, true, false, true, false, false},
                }
            },
        };

        public struct EncodedNumber
        {
            public bool[] Left { get; set; }
            public bool[] Right { get; set; }
        }

        public const int Margin = 10;
    }
}
