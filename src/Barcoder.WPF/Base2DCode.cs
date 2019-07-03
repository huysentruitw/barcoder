using System.Windows;
using System.Windows.Controls;

namespace Barcoder.WPF
{
    [TemplatePart(Name = CanvasElementName, Type = typeof(Canvas))]
    public abstract class Base2DCode : BaseBarcode
    {
        public static readonly DependencyProperty ModuleSizeProperty = DependencyProperty.Register(nameof(ModuleSize), typeof(double), typeof(Base2DCode), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.None));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(Base2DCode), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public new double Height
        {
            get => base.Height;
            protected set => base.Height = value;
        }

        public double ModuleSize
        {
            get => (double)GetValue(ModuleSizeProperty);
            set => SetValue(ModuleSizeProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public new double Width
        {
            get => base.Width;
            protected set => base.Width = value;
        }

        public override void Redraw()
        {
            if (_canvas == null)
                return;

            _canvas.Children.Clear();

            var barcode = GetBarcode();

            base.Width = base.Height = barcode.Bounds.X * ModuleSize;

            for (var y = 0; y < barcode.Bounds.Y; y++)
            {
                for (var x = 0; x < barcode.Bounds.X; x++)
                {
                    if (barcode.At(x, y))
                    {
                        AddRectangle(x * ModuleSize, y * ModuleSize, ModuleSize, ModuleSize);
                    }
                }
            }
        }
    }
}
