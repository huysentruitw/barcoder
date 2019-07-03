using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Barcoder.WPF
{
    [TemplatePart(Name = CanvasElementName, Type = typeof(Canvas))]
    public abstract class BaseBarcode : Control
    {
        internal const string CanvasElementName = "PART_Canvas";
        protected Canvas _canvas;

        public override void OnApplyTemplate()
        {
            _canvas = GetTemplateChild(CanvasElementName) as Canvas;
            Redraw();
        }

        public abstract void Redraw();

        protected void AddRectangle(double x, double y, double width, double height)
        {
            var rect = new Rectangle()
            {
                Fill = Foreground,
                Stroke = Foreground,
                Width = width,
                Height = height,
                StrokeThickness = 0,
            };
            _canvas.Children.Add(rect);
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        protected abstract IBarcode GetBarcode();

        protected T GetValue<T>(DependencyProperty dependencyProperty)
        {
            return (T)GetValue(dependencyProperty);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            Redraw();
        }
    }
}
