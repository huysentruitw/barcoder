using System.Collections.Generic;

namespace Barcoder.UpcE
{
    internal static class Constants
    {
        public static readonly IReadOnlyDictionary<char, EncodedNumber> EncodingTable = new Dictionary<char, EncodedNumber>
        {
            { '0', new EncodedNumber
                {
                    Odd  = new[] {false, false, false, true, true, false, true},
                    Even = new[] {false, true, false, false, true, true, true},
                }
            },
            { '1', new EncodedNumber
                {
                    Odd  = new[] {false, false, true, true, false, false, true},
                    Even = new[] {false, true, true, false, false, true, true},
                }
            },
            { '2', new EncodedNumber
                {
                    Odd  = new[] {false, false, true, false, false, true, true},
                    Even = new[] {false, false, true, true, false, true, true},
                }
            },
            { '3', new EncodedNumber
                {
                    Odd  = new[] {false, true, true, true, true, false, true},
                    Even = new[] {false, true, false, false, false, false, true},
                }
            },
            { '4', new EncodedNumber
                {
                    Odd  = new[] {false, true, false, false, false, true, true},
                    Even = new[] {false, false, true, true, true, false, true},
                }
            },
            { '5', new EncodedNumber
                {
                    Odd  = new[] {false, true, true, false, false, false, true},
                    Even = new[] {false, true, true, true, false, false, true},
                }
            },
            { '6', new EncodedNumber
                {
                    Odd  = new[] {false, true, false, true, true, true, true},
                    Even = new[] {false, false, false, false, true, false, true},
                }
            },
            { '7', new EncodedNumber
                {
                    Odd  = new[] {false, true, true, true, false, true, true},
                    Even = new[] {false, false, true, false, false, false, true},
                }
            },
            { '8', new EncodedNumber
                {
                    Odd  = new[] {false, true, true, false, true, true, true},
                    Even = new[] {false, false, false, true, false, false, true},
                }
            },
            { '9', new EncodedNumber
                {
                    Odd  = new[] {false, false, false, true, false, true, true},
                    Even = new[] {false, false, true, false, true, true, true},
                }
            },
        };

        public static readonly IReadOnlyDictionary<char, ParityPatterns> ParityPatternTable = new Dictionary<char, ParityPatterns>
        {
            { '0', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Even, Parity.Even, Parity.Odd, Parity.Odd, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Odd, Parity.Odd, Parity.Even, Parity.Even, Parity.Even},
                }
            },
            { '1', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Even, Parity.Odd, Parity.Even, Parity.Odd, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Odd, Parity.Even, Parity.Odd, Parity.Even, Parity.Even},
                }
            },
            { '2', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Even, Parity.Odd, Parity.Odd, Parity.Even, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Odd, Parity.Even, Parity.Even, Parity.Odd, Parity.Even},
                }
            },
            { '3', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Even, Parity.Odd, Parity.Odd, Parity.Odd, Parity.Even},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Odd, Parity.Even, Parity.Even, Parity.Even, Parity.Odd},
                }
            },
            { '4', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Even, Parity.Even, Parity.Odd, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Odd, Parity.Odd, Parity.Even, Parity.Even},
                }
            },
            { '5', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Odd, Parity.Even, Parity.Even, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Even, Parity.Odd, Parity.Odd, Parity.Even},
                }
            },
            { '6', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Odd, Parity.Odd, Parity.Even, Parity.Even},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Even, Parity.Even, Parity.Odd, Parity.Odd},
                }
            },
            { '7', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Even, Parity.Odd, Parity.Even, Parity.Odd},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Odd, Parity.Even, Parity.Odd, Parity.Even},
                }
            },
            { '8', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Even, Parity.Odd, Parity.Odd, Parity.Even},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Odd, Parity.Even, Parity.Even, Parity.Odd},
                }
            },
            { '9', new ParityPatterns
                {
                    NumberSystemZero = new[] {Parity.Even, Parity.Odd, Parity.Odd, Parity.Even, Parity.Odd, Parity.Even},
                    NumberSystemOne  = new[] {Parity.Odd, Parity.Even, Parity.Even, Parity.Odd, Parity.Even, Parity.Odd},
                }
            },
        };

        public struct EncodedNumber
        {
            public bool[] Odd { get; set; }
            public bool[] Even { get; set; }
        }

        public struct ParityPatterns
        {
            public Parity[] NumberSystemZero { get; set; }
            public Parity[] NumberSystemOne { get; set; }
        }

        public enum Parity
        {
            Odd,
            Even
        }

        public const int Margin = 10;
    }

    public enum UpcENumberSystem
    {
        Zero,
        One
    }
}
