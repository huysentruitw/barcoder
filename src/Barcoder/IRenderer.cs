using System.IO;

namespace Barcoder.Renderers
{
    public interface IRenderer
    {
        Stream Render(IBarcode barcode);
    }
}
