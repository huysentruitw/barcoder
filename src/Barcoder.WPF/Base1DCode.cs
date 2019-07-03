using System.Windows;
using System.Windows.Controls;

namespace Barcoder.WPF
{
    [TemplatePart(Name = CanvasElementName, Type = typeof(Canvas))]
    public abstract class Base1DCode : BaseBarcode
    {
        public static readonly DependencyProperty ModuleWidthProperty = DependencyProperty.Register(nameof(ModuleWidth), typeof(double), typeof(Base1DCode), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.None));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(Base1DCode), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public double ModuleWidth
        {
            get => (double)GetValue(ModuleWidthProperty);
            set => SetValue(ModuleWidthProperty, value);
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
            Width = barcode.Bounds.X * ModuleWidth;

            double posX = 0;
            const double posY = 0;

            double startX = -1;
            for (int i = 0; i < barcode.Bounds.X; i++)
            {
                if (barcode.At(i, 0))
                {
                    if (startX < 0)
                        startX = posX;
                }
                else
                {
                    if (startX >= 0)
                    {
                        AddRectangle(startX, posY, posX - startX, Height);
                        startX = -1;
                    }
                }
                posX += ModuleWidth;
            }
            if (startX >= 0)
            {
                AddRectangle(startX, posY, posX - startX, Height);
            }
        }
    }
}
