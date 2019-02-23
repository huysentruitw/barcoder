using System.Collections.Generic;

namespace Barcoder.Code39
    {
    internal static class Constants
    {
        public static readonly IReadOnlyDictionary<char, (int value, bool[] data)> EncodingTable = new Dictionary<char, (int, bool[])>
        {
            { '0', (0,  new[] {true, false, true, false, false, true, true, false, true, true, false, true}) },
            { '1', (1,  new[] {true, true, false, true, false, false, true, false, true, false, true, true}) },
            { '2', (2,  new[] {true, false, true, true, false, false, true, false, true, false, true, true}) },
            { '3', (3,  new[] {true, true, false, true, true, false, false, true, false, true, false, true}) },
            { '4', (4,  new[] {true, false, true, false, false, true, true, false, true, false, true, true}) },
            { '5', (5,  new[] {true, true, false, true, false, false, true, true, false, true, false, true}) },
            { '6', (6,  new[] {true, false, true, true, false, false, true, true, false, true, false, true}) },
            { '7', (7,  new[] {true, false, true, false, false, true, false, true, true, false, true, true}) },
            { '8', (8,  new[] {true, true, false, true, false, false, true, false, true, true, false, true}) },
            { '9', (9,  new[] {true, false, true, true, false, false, true, false, true, true, false, true}) },
            { 'A', (10, new[] {true, true, false, true, false, true, false, false, true, false, true, true}) },
            { 'B', (11, new[] {true, false, true, true, false, true, false, false, true, false, true, true}) },
            { 'C', (12, new[] {true, true, false, true, true, false, true, false, false, true, false, true}) },
            { 'D', (13, new[] {true, false, true, false, true, true, false, false, true, false, true, true}) },
            { 'E', (14, new[] {true, true, false, true, false, true, true, false, false, true, false, true}) },
            { 'F', (15, new[] {true, false, true, true, false, true, true, false, false, true, false, true}) },
            { 'G', (16, new[] {true, false, true, false, true, false, false, true, true, false, true, true}) },
            { 'H', (17, new[] {true, true, false, true, false, true, false, false, true, true, false, true}) },
            { 'I', (18, new[] {true, false, true, true, false, true, false, false, true, true, false, true}) },
            { 'J', (19, new[] {true, false, true, false, true, true, false, false, true, true, false, true}) },
            { 'K', (20, new[] {true, true, false, true, false, true, false, true, false, false, true, true}) },
            { 'L', (21, new[] {true, false, true, true, false, true, false, true, false, false, true, true}) },
            { 'M', (22, new[] {true, true, false, true, true, false, true, false, true, false, false, true}) },
            { 'N', (23, new[] {true, false, true, false, true, true, false, true, false, false, true, true}) },
            { 'O', (24, new[] {true, true, false, true, false, true, true, false, true, false, false, true}) },
            { 'P', (25, new[] {true, false, true, true, false, true, true, false, true, false, false, true}) },
            { 'Q', (26, new[] {true, false, true, false, true, false, true, true, false, false, true, true}) },
            { 'R', (27, new[] {true, true, false, true, false, true, false, true, true, false, false, true}) },
            { 'S', (28, new[] {true, false, true, true, false, true, false, true, true, false, false, true}) },
            { 'T', (29, new[] {true, false, true, false, true, true, false, true, true, false, false, true}) },
            { 'U', (30, new[] {true, true, false, false, true, false, true, false, true, false, true, true}) },
            { 'V', (31, new[] {true, false, false, true, true, false, true, false, true, false, true, true}) },
            { 'W', (32, new[] {true, true, false, false, true, true, false, true, false, true, false, true}) },
            { 'X', (33, new[] {true, false, false, true, false, true, true, false, true, false, true, true}) },
            { 'Y', (34, new[] {true, true, false, false, true, false, true, true, false, true, false, true}) },
            { 'Z', (35, new[] {true, false, false, true, true, false, true, true, false, true, false, true}) },
            { '-', (36, new[] {true, false, false, true, false, true, false, true, true, false, true, true}) },
            { '.', (37, new[] {true, true, false, false, true, false, true, false, true, true, false, true}) },
            { ' ', (38, new[] {true, false, false, true, true, false, true, false, true, true, false, true}) },
            { '$', (39, new[] {true, false, false, true, false, false, true, false, false, true, false, true}) },
            { '/', (40, new[] {true, false, false, true, false, false, true, false, true, false, false, true}) },
            { '+', (41, new[] {true, false, false, true, false, true, false, false, true, false, false, true}) },
            { '%', (42, new[] {true, false, true, false, false, true, false, false, true, false, false, true}) },
            { '*', (-1, new[] {true, false, false, true, false, true, true, false, true, true, false, true}) },
        };

        public static readonly IReadOnlyDictionary<char, string> ExtendedTable = new Dictionary<char, string>
        {
            { (char)0, @"%U" }, { (char)1, @"$A" }, { (char)2, @"$B" }, { (char)3, @"$C" }, { (char)4, @"$D" }, { (char)5, @"$E" }, { (char)6, @"$F" }, { (char)7, @"$G" }, { (char)8, @"$H" }, { (char)9, @"$I" }, { (char)10, @"$J" },
            { (char)11, @"$K" }, { (char)12, @"$L" }, { (char)13, @"$M" }, { (char)14, @"$N" }, { (char)15, @"$O" }, { (char)16, @"$P" }, { (char)17, @"$Q" }, { (char)18, @"$R" }, { (char)19, @"$S" }, { (char)20, @"$T" },
            { (char)21, @"$U" }, { (char)22, @"$V" }, { (char)23, @"$W" }, { (char)24, @"$X" }, { (char)25, @"$Y" }, { (char)26, @"$Z" }, { (char)27, @"%A" }, { (char)28, @"%B" }, { (char)29, @"%C" }, { (char)30, @"%D" },
            { (char)31, @"%E" }, { (char)33, @"/A" }, { (char)34, @"/B" }, { (char)35, @"/C" }, { (char)36, @"/D" }, { (char)37, @"/E" }, { (char)38, @"/F" }, { (char)39, @"/G" }, { (char)40, @"/H" }, { (char)41, @"/I" },
            { (char)42, @"/J" }, { (char)43, @"/K" }, { (char)44, @"/L" }, { (char)47, @"/O" }, { (char)58, @"/Z" }, { (char)59, @"%F" }, { (char)60, @"%G" }, { (char)61, @"%H" }, { (char)62, @"%I" }, { (char)63, @"%J" },
            { (char)64, @"%V" }, { (char)91, @"%K" }, { (char)92, @"%L" }, { (char)93, @"%M" }, { (char)94, @"%N" }, { (char)95, @"%O" }, { (char)96, @"%W" }, { (char)97, @"+A" }, { (char)98, @"+B" }, { (char)99, @"+C" },
            { (char)100, @"+D" }, { (char)101, @"+E" }, { (char)102, @"+F" }, { (char)103, @"+G" }, { (char)104, @"+H" }, { (char)105, @"+I" }, { (char)106, @"+J" }, { (char)107, @"+K" }, { (char)108, @"+L" },
            { (char)109, @"+M" }, { (char)110, @"+N" }, { (char)111, @"+O" }, { (char)112, @"+P" }, { (char)113, @"+Q" }, { (char)114, @"+R" }, { (char)115, @"+S" }, { (char)116, @"+T" }, { (char)117, @"+U" },
            { (char)118, @"+V" }, { (char)119, @"+W" }, { (char)120, @"+X" }, { (char)121, @"+Y" }, { (char)122, @"+Z" }, { (char)123, @"%P" }, { (char)124, @"%Q" }, { (char)125, @"%R" }, { (char)126, @"%S" },
            { (char)127, @"%T" },
        };

        public const int Margin = 10;
    }
}
