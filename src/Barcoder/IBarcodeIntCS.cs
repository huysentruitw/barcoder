namespace Barcoder
{
    public interface IBarcodeIntCS : IBarcode
    {
        int CheckSum { get; }
    }
}
