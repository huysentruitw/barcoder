namespace Barcoder
{
    public struct Metadata
    {
        internal Metadata(string codeKind, byte dimensions)
        {
            CodeKind = codeKind;
            Dimensions = dimensions;
        }

        /// <summary>
        /// The name of the barcode kind.
        /// </summary>
        public string CodeKind { get; }
        /// <summary>
        /// Contains 1 for 1D or 2 for 2D barcodes.
        /// </summary>
        public byte Dimensions { get; }
    }
}
