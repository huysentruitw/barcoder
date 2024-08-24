namespace Barcoder.Renderer.Svg
{
    public sealed class SvgRendererOptions
    {
        public bool IncludeEanContentAsText { get; set; } = false;

        public int? CustomMargin { get; set; } = null;

        public int BarHeightFor1DBarcode { get; set; } = 50;

        public string EanFontFamily { get; set; } = "arial";
    }
}
