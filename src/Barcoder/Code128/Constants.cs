namespace Barcoder.Code128
{
    internal static class Constants
    {
        public static readonly bool[][] EncodingTable = new bool[107][]
        {
            new[] {true, true, false, true, true, false, false, true, true, false, false},
	        new[] {true, true, false, false, true, true, false, true, true, false, false},
            new[] {true, true, false, false, true, true, false, false, true, true, false},
            new[] {true, false, false, true, false, false, true, true, false, false, false},
            new[] {true, false, false, true, false, false, false, true, true, false, false},
            new[] {true, false, false, false, true, false, false, true, true, false, false},
            new[] {true, false, false, true, true, false, false, true, false, false, false},
            new[] {true, false, false, true, true, false, false, false, true, false, false},
            new[] {true, false, false, false, true, true, false, false, true, false, false},
            new[] {true, true, false, false, true, false, false, true, false, false, false},
            new[] {true, true, false, false, true, false, false, false, true, false, false},
            new[] {true, true, false, false, false, true, false, false, true, false, false},
            new[] {true, false, true, true, false, false, true, true, true, false, false},
            new[] {true, false, false, true, true, false, true, true, true, false, false},
            new[] {true, false, false, true, true, false, false, true, true, true, false},
            new[] {true, false, true, true, true, false, false, true, true, false, false},
            new[] {true, false, false, true, true, true, false, true, true, false, false},
            new[] {true, false, false, true, true, true, false, false, true, true, false},
            new[] {true, true, false, false, true, true, true, false, false, true, false},
            new[] {true, true, false, false, true, false, true, true, true, false, false},
            new[] {true, true, false, false, true, false, false, true, true, true, false},
            new[] {true, true, false, true, true, true, false, false, true, false, false},
            new[] {true, true, false, false, true, true, true, false, true, false, false},
            new[] {true, true, true, false, true, true, false, true, true, true, false},
            new[] {true, true, true, false, true, false, false, true, true, false, false},
            new[] {true, true, true, false, false, true, false, true, true, false, false},
            new[] {true, true, true, false, false, true, false, false, true, true, false},
            new[] {true, true, true, false, true, true, false, false, true, false, false},
            new[] {true, true, true, false, false, true, true, false, true, false, false},
            new[] {true, true, true, false, false, true, true, false, false, true, false},
            new[] {true, true, false, true, true, false, true, true, false, false, false},
            new[] {true, true, false, true, true, false, false, false, true, true, false},
            new[] {true, true, false, false, false, true, true, false, true, true, false},
            new[] {true, false, true, false, false, false, true, true, false, false, false},
            new[] {true, false, false, false, true, false, true, true, false, false, false},
            new[] {true, false, false, false, true, false, false, false, true, true, false},
            new[] {true, false, true, true, false, false, false, true, false, false, false},
            new[] {true, false, false, false, true, true, false, true, false, false, false},
            new[] {true, false, false, false, true, true, false, false, false, true, false},
            new[] {true, true, false, true, false, false, false, true, false, false, false},
            new[] {true, true, false, false, false, true, false, true, false, false, false},
            new[] {true, true, false, false, false, true, false, false, false, true, false},
            new[] {true, false, true, true, false, true, true, true, false, false, false},
            new[] {true, false, true, true, false, false, false, true, true, true, false},
            new[] {true, false, false, false, true, true, false, true, true, true, false},
            new[] {true, false, true, true, true, false, true, true, false, false, false},
            new[] {true, false, true, true, true, false, false, false, true, true, false},
            new[] {true, false, false, false, true, true, true, false, true, true, false},
            new[] {true, true, true, false, true, true, true, false, true, true, false},
            new[] {true, true, false, true, false, false, false, true, true, true, false},
            new[] {true, true, false, false, false, true, false, true, true, true, false},
            new[] {true, true, false, true, true, true, false, true, false, false, false},
            new[] {true, true, false, true, true, true, false, false, false, true, false},
            new[] {true, true, false, true, true, true, false, true, true, true, false},
            new[] {true, true, true, false, true, false, true, true, false, false, false},
            new[] {true, true, true, false, true, false, false, false, true, true, false},
            new[] {true, true, true, false, false, false, true, false, true, true, false},
            new[] {true, true, true, false, true, true, false, true, false, false, false},
            new[] {true, true, true, false, true, true, false, false, false, true, false},
            new[] {true, true, true, false, false, false, true, true, false, true, false},
            new[] {true, true, true, false, true, true, true, true, false, true, false},
            new[] {true, true, false, false, true, false, false, false, false, true, false},
            new[] {true, true, true, true, false, false, false, true, false, true, false},
            new[] {true, false, true, false, false, true, true, false, false, false, false},
            new[] {true, false, true, false, false, false, false, true, true, false, false},
            new[] {true, false, false, true, false, true, true, false, false, false, false},
            new[] {true, false, false, true, false, false, false, false, true, true, false},
            new[] {true, false, false, false, false, true, false, true, true, false, false},
            new[] {true, false, false, false, false, true, false, false, true, true, false},
            new[] {true, false, true, true, false, false, true, false, false, false, false},
            new[] {true, false, true, true, false, false, false, false, true, false, false},
            new[] {true, false, false, true, true, false, true, false, false, false, false},
            new[] {true, false, false, true, true, false, false, false, false, true, false},
            new[] {true, false, false, false, false, true, true, false, true, false, false},
            new[] {true, false, false, false, false, true, true, false, false, true, false},
            new[] {true, true, false, false, false, false, true, false, false, true, false},
            new[] {true, true, false, false, true, false, true, false, false, false, false},
            new[] {true, true, true, true, false, true, true, true, false, true, false},
            new[] {true, true, false, false, false, false, true, false, true, false, false},
            new[] {true, false, false, false, true, true, true, true, false, true, false},
            new[] {true, false, true, false, false, true, true, true, true, false, false},
            new[] {true, false, false, true, false, true, true, true, true, false, false},
            new[] {true, false, false, true, false, false, true, true, true, true, false},
            new[] {true, false, true, true, true, true, false, false, true, false, false},
            new[] {true, false, false, true, true, true, true, false, true, false, false},
            new[] {true, false, false, true, true, true, true, false, false, true, false},
            new[] {true, true, true, true, false, true, false, false, true, false, false},
            new[] {true, true, true, true, false, false, true, false, true, false, false},
            new[] {true, true, true, true, false, false, true, false, false, true, false},
            new[] {true, true, false, true, true, false, true, true, true, true, false},
            new[] {true, true, false, true, true, true, true, false, true, true, false},
            new[] {true, true, true, true, false, true, true, false, true, true, false},
            new[] {true, false, true, false, true, true, true, true, false, false, false},
            new[] {true, false, true, false, false, false, true, true, true, true, false},
            new[] {true, false, false, false, true, false, true, true, true, true, false},
            new[] {true, false, true, true, true, true, false, true, false, false, false},
            new[] {true, false, true, true, true, true, false, false, false, true, false},
            new[] {true, true, true, true, false, true, false, true, false, false, false},
            new[] {true, true, true, true, false, true, false, false, false, true, false},
            new[] {true, false, true, true, true, false, true, true, true, true, false},
            new[] {true, false, true, true, true, true, false, true, true, true, false},
            new[] {true, true, true, false, true, false, true, true, true, true, false},
            new[] {true, true, true, true, false, true, false, true, true, true, false},
            new[] {true, true, false, true, false, false, false, false, true, false, false},
            new[] {true, true, false, true, false, false, true, false, false, false, false},
            new[] {true, true, false, true, false, false, true, true, true, false, false},
            new[] {true, true, false, false, false, true, true, true, false, true, false, true, true},
        };

        public const byte StartASymbol = 103;
        public const byte StartBSymbol = 104;
        public const byte StartCSymbol = 105;

        public const byte CodeASymbol = 101;
        public const byte CodeBSymbol = 100;
        public const byte CodeCSymbol = 99;

        public const byte StopSymbol = 106;

        /// <summary>
        /// Special Function 1
        /// </summary>
        public const char FNC1 = '\u00f1';

        /// <summary>
        /// Special Function 2
        /// </summary>
        public const char FNC2 = '\u00f2';

        /// <summary>
        /// Special Function 3
        /// </summary>
        public const char FNC3 = '\u00f3';

        /// <summary>
        /// Special Function 4
        /// </summary>
        public const char FNC4 = '\u00f4';

        public const string ABTable = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_";
        public const string BTable = ABTable + "`abcdefghijklmnopqrstuvwxyz{|}~\u007F";
        public const string AOnlyTable =
            "\u0000\u0001\u0002\u0003\u0004" + // NUL, SOH, STX, ETX, EOT
            "\u0005\u0006\u0007\u0008\u0009" + // ENQ, ACK, BEL, BS,  HT
            "\u000A\u000B\u000C\u000D\u000E" + // LF,  VT,  FF,  CR,  SO
            "\u000F\u0010\u0011\u0012\u0013" + // SI,  DLE, DC1, DC2, DC3
            "\u0014\u0015\u0016\u0017\u0018" + // DC4, NAK, SYN, ETB, CAN
            "\u0019\u001A\u001B\u001C\u001D" + // EM,  SUB, ESC, FS,  GS
            "\u001E\u001F"; // RS,  US
        public const string ATable = ABTable + AOnlyTable;

        public const int Margin = 10;
    }
}
