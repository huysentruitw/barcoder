namespace Barcoder.Aztec
{
    internal static class EncodingModeExtensions
    {
        public static byte BitCount(this EncodingMode mode)
            => mode == EncodingMode.Digit ? (byte)4 : (byte)5;
    }
}
