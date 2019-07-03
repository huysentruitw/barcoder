using System.Windows;
using System.Windows.Controls;
using Barcoder.DataMatrix;

namespace Barcoder.WPF
{
    [TemplatePart(Name = CanvasElementName, Type = typeof(Canvas))]
    public class DataMatrix : Base2DCode
    {
        static DataMatrix()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataMatrix), new FrameworkPropertyMetadata(typeof(DataMatrix)));
        }

        protected override IBarcode GetBarcode()
        {
            return DataMatrixEncoder.Encode(Value);
        }
    }
}
