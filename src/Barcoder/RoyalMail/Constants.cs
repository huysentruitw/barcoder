namespace Barcoder.RoyalMail
{
    internal static class Constants
    {
        internal const int BARCODE_HEIGHT = 8;

        internal static readonly BarTypes[][] Symbols = new BarTypes[][]
        {
            new[] { BarTypes.Tracker, BarTypes.Tracker, BarTypes.FullHeight, BarTypes.FullHeight }, // 0
            new[] { BarTypes.Tracker, BarTypes.Descender, BarTypes.Ascender, BarTypes.FullHeight }, // 1
            new[] { BarTypes.Tracker, BarTypes.Descender, BarTypes.FullHeight, BarTypes.Ascender }, // 2
            new[] { BarTypes.Descender, BarTypes.Tracker, BarTypes.Ascender, BarTypes.FullHeight }, // 3
            new[] { BarTypes.Descender, BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Ascender }, // 4
            new[] { BarTypes.Descender, BarTypes.Descender, BarTypes.Ascender, BarTypes.Ascender }, // 5
            new[] { BarTypes.Tracker, BarTypes.Ascender, BarTypes.Descender, BarTypes.FullHeight }, // 6
            new[] { BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Tracker, BarTypes.FullHeight }, // 7
            new[] { BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Descender, BarTypes.Ascender }, // 8
            new[] { BarTypes.Descender, BarTypes.Ascender, BarTypes.Tracker, BarTypes.FullHeight }, // 9
            new[] { BarTypes.Descender, BarTypes.Ascender, BarTypes.Descender, BarTypes.Ascender }, // A
            new[] { BarTypes.Descender, BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Ascender }, // B
            new[] { BarTypes.Tracker, BarTypes.Ascender, BarTypes.FullHeight, BarTypes.Descender }, // C
            new[] { BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Ascender, BarTypes.Descender }, // D
            new[] { BarTypes.Tracker, BarTypes.FullHeight, BarTypes.FullHeight, BarTypes.Tracker }, // E
            new[] { BarTypes.Descender, BarTypes.Ascender, BarTypes.Ascender, BarTypes.Descender }, // F
            new[] { BarTypes.Descender, BarTypes.Ascender, BarTypes.FullHeight, BarTypes.Tracker }, // G
            new[] { BarTypes.Descender, BarTypes.FullHeight, BarTypes.Ascender, BarTypes.Tracker }, // H
            new[] { BarTypes.Ascender, BarTypes.Tracker, BarTypes.Descender, BarTypes.FullHeight }, // I
            new[] { BarTypes.Ascender, BarTypes.Descender, BarTypes.Tracker, BarTypes.FullHeight }, // J
            new[] { BarTypes.Ascender, BarTypes.Descender, BarTypes.Descender, BarTypes.Ascender }, // K
            new[] { BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Tracker, BarTypes.FullHeight }, // L
            new[] { BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Descender, BarTypes.Ascender }, // M
            new[] { BarTypes.FullHeight, BarTypes.Descender, BarTypes.Tracker, BarTypes.Ascender }, // N
            new[] { BarTypes.Ascender, BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Descender }, // O
            new[] { BarTypes.Ascender, BarTypes.Descender, BarTypes.Ascender, BarTypes.Descender }, // P
            new[] { BarTypes.Ascender, BarTypes.Descender, BarTypes.FullHeight, BarTypes.Tracker }, // Q
            new[] { BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Ascender, BarTypes.Descender }, // R
            new[] { BarTypes.FullHeight, BarTypes.Tracker, BarTypes.FullHeight, BarTypes.Tracker }, // S
            new[] { BarTypes.FullHeight, BarTypes.Descender, BarTypes.Ascender, BarTypes.Tracker }, // T
            new[] { BarTypes.Ascender, BarTypes.Ascender, BarTypes.Descender, BarTypes.Descender }, // U
            new[] { BarTypes.Ascender, BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Descender }, // V
            new[] { BarTypes.Ascender, BarTypes.FullHeight, BarTypes.Descender, BarTypes.Tracker }, // W
            new[] { BarTypes.FullHeight, BarTypes.Ascender, BarTypes.Tracker, BarTypes.Descender }, // X
            new[] { BarTypes.FullHeight, BarTypes.Ascender, BarTypes.Descender, BarTypes.Tracker }, // Y
            new[] { BarTypes.FullHeight, BarTypes.FullHeight, BarTypes.Tracker, BarTypes.Tracker }, // Z
        };
    }
}
