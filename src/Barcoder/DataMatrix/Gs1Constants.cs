using System.Collections.Generic;

namespace Barcoder.DataMatrix
{
    internal static class Gs1Constants
    {
        public static readonly IReadOnlyDictionary<string, int> PreDefinedApplicationIdentifierLengths = new Dictionary<string, int>
        {
            { "00", 20 },
            { "01", 16 },
            { "02", 16 },
            { "03", 16 },
            { "04", 18 },
            { "11", 8 },
            { "12", 8 },
            { "13", 8 },
            { "14", 8 },
            { "15", 8 },
            { "16", 8 },
            { "17", 8 },
            { "18", 8 },
            { "19", 8 },
            { "20", 4 },
            { "31", 10 },
            { "32", 10 },
            { "33", 10 },
            { "34", 10 },
            { "35", 10 },
            { "36", 10 },
            { "41", 16 },
        };

        public static class SpecialCodewords
        {
            public const byte Pad = 129;
            public const byte LatchToC40Encodation = 230;
            public const byte LatchToBase256Encodation = 231;
            public const byte FNC1 = 232;
            public const byte StructuredAppend = 233;
            public const byte ReaderProgramming = 234;
            public const byte UpperShiftToExtendedAscii = 235;
            public const byte Macro05 = 236;
            public const byte Macro06 = 237;
            public const byte LatchToAnsiX12Encodation = 238;
            public const byte LatchToTextEncodation = 239;
            public const byte LatchToEdifactEncodation = 240;
            public const byte ECI = 241;
        }
    }
}
