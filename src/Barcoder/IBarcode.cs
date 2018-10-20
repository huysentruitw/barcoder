namespace Barcoder
{
    public interface IBarcode
    {
        string Content { get; }
        Bounds Bounds { get; }
        bool At(int x, int y);
        Metadata Metadata { get; }
    }
}
