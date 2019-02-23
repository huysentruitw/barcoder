using System.Collections.Generic;

namespace Barcoder.Code93
{
    internal static class Constants
    {
        public const char FNC1 = '\u00f1';
        public const char FNC2 = '\u00f2';
        public const char FNC3 = '\u00f3';
        public const char FNC4 = '\u00f4';

        public static readonly IReadOnlyDictionary<char, (int value, uint data)> EncodingTable = new Dictionary<char, (int, uint)>
        {
            { '0',  (0, 0x114) },  { '1',  (1, 0x148) },  { '2', (2, 0x144) },
            { '3',  (3, 0x142) },  { '4',  (4, 0x128) },  { '5', (5, 0x124) },
            { '6',  (6, 0x122) },  { '7',  (7, 0x150) },  { '8', (8, 0x112) },
            { '9',  (9, 0x10A) },  { 'A',  (10, 0x1A8) }, { 'B', (11, 0x1A4) },
            { 'C',  (12, 0x1A2) }, { 'D',  (13, 0x194) }, { 'E', (14, 0x192) },
            { 'F',  (15, 0x18A) }, { 'G',  (16, 0x168) }, { 'H', (17, 0x164) },
            { 'I',  (18, 0x162) }, { 'J',  (19, 0x134) }, { 'K', (20, 0x11A) },
            { 'L',  (21, 0x158) }, { 'M',  (22, 0x14C) }, { 'N', (23, 0x146) },
            { 'O',  (24, 0x12C) }, { 'P',  (25, 0x116) }, { 'Q', (26, 0x1B4) },
            { 'R',  (27, 0x1B2) }, { 'S',  (28, 0x1AC) }, { 'T', (29, 0x1A6) },
            { 'U',  (30, 0x196) }, { 'V',  (31, 0x19A) }, { 'W', (32, 0x16C) },
            { 'X',  (33, 0x166) }, { 'Y',  (34, 0x136) }, { 'Z', (35, 0x13A) },
            { '-',  (36, 0x12E) }, { '.',  (37, 0x1D4) }, { ' ', (38, 0x1D2) },
            { '$',  (39, 0x1CA) }, { '/',  (40, 0x16E) }, { '+', (41, 0x176) },
            { '%',  (42, 0x1AE) }, { FNC1, (43, 0x126) }, { FNC2, (44, 0x1DA) },
            { FNC3, (45, 0x1D6) }, { FNC4, (46, 0x132) }, { '*', (47, 0x15E) },
        };

        public static readonly string[] ExtendedTable = new[]
        {
            "\u00f2U", "\u00f1A", "\u00f1B", "\u00f1C", "\u00f1D", "\u00f1E", "\u00f1F", "\u00f1G",
            "\u00f1H", "\u00f1I", "\u00f1J", "\u00f1K", "\u00f1L", "\u00f1M", "\u00f1N", "\u00f1O",
            "\u00f1P", "\u00f1Q", "\u00f1R", "\u00f1S", "\u00f1T", "\u00f1U", "\u00f1V", "\u00f1W",
            "\u00f1X", "\u00f1Y", "\u00f1Z", "\u00f2A", "\u00f2B", "\u00f2C", "\u00f2D", "\u00f2E",
            " ", "\u00f3A", "\u00f3B", "\u00f3C", "\u00f3D", "\u00f3E", "\u00f3F", "\u00f3G",
            "\u00f3H", "\u00f3I", "\u00f3J", "\u00f3K", "\u00f3L", "-", ".", "\u00f3O",
            "0", "1", "2", "3", "4", "5", "6", "7",
            "8", "9", "\u00f3Z", "\u00f2F", "\u00f2G", "\u00f2H", "\u00f2I", "\u00f2J",
            "\u00f2V", "A", "B", "C", "D", "E", "F", "G",
            "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W",
            "X", "Y", "Z", "\u00f2K", "\u00f2L", "\u00f2M", "\u00f2N", "\u00f2O",
            "\u00f2W", "\u00f4A", "\u00f4B", "\u00f4C", "\u00f4D", "\u00f4E", "\u00f4F", "\u00f4G",
            "\u00f4H", "\u00f4I", "\u00f4J", "\u00f4K", "\u00f4L", "\u00f4M", "\u00f4N", "\u00f4O",
            "\u00f4P", "\u00f4Q", "\u00f4R", "\u00f4S", "\u00f4T", "\u00f4U", "\u00f4V", "\u00f4W",
            "\u00f4X", "\u00f4Y", "\u00f4Z", "\u00f2P", "\u00f2Q", "\u00f2R", "\u00f2S", "\u00f2T",
        };

        public const int Margin = 10;
    }
}
