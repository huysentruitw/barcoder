using System.IO;

namespace Barcoder.Renderers
{
    public interface IRenderer
    {
        void Render(IBarcode barcode, Stream outputStream);
    }
}
