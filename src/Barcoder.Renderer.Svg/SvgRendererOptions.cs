namespace Barcoder.Renderer.Svg
{
    public sealed class SvgRendererOptions
    {
        public bool IncludeEanContentAsText { get; set; } = false;
        
        public int? CustomMargin { get; set; } = null;
    }
}
