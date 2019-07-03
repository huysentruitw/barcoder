using System.Windows;
using System.Windows.Controls;
using Barcoder.Code128;

namespace Barcoder.WPF
{
    [TemplatePart(Name = CanvasElementName, Type = typeof(Canvas))]
    public class Code128 : Base1DCode
    {
        public static readonly DependencyProperty IncludeChecksumProperty = DependencyProperty.Register(nameof(IncludeChecksum), typeof(bool), typeof(Code128), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));

        static Code128()

        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Code128), new FrameworkPropertyMetadata(typeof(Code128)));
        }

        public bool IncludeChecksum
        {
            get => GetValue<bool>(IncludeChecksumProperty);
            set => SetValue(IncludeChecksumProperty, value);
        }

        protected override IBarcode GetBarcode()
        {
            return Code128Encoder.Encode(Value, IncludeChecksum);
        }
    }
}
