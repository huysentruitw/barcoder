namespace Barcoder.DataMatrix
{
    internal static class DataMatrixSpecialCodewords
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
