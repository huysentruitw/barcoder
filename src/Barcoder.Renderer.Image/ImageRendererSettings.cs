using SixLabors.ImageSharp.PixelFormats;

namespace Barcoder.Renderer.Image
{
    public class ImageRendererSettings<TPixel> where TPixel : struct, IPixel<TPixel>
    {
        public int PixelSize { get; set; } = 10;
        public int BarHeightFor1DBarcode { get; set; } = 40;
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;
        public int JpegQuality { get; set; } = 75;
        public bool IncludeEanContentAsText { get; set; }
        public string EanFontFamily { get; set; } = "Arial";
        public TPixel BackgroundColor { get; set; } = NamedColors<TPixel>.White;
        public TPixel ForegroundColor { get; set; } = NamedColors<TPixel>.Black;
    }
}
