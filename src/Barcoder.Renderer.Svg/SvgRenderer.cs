using System;
using System.IO;
using Barcoder.Renderers;
using SvgLib;

namespace Barcoder.Renderer.Svg
{
    public sealed class SvgRenderer : IRenderer
    {
        public void Render(IBarcode barcode, Stream outputStream)
        {
            barcode = barcode ?? throw new ArgumentNullException(nameof(barcode));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
            if (barcode.Bounds.Y == 1)
                Render1D(barcode, outputStream);
            else if (barcode.Bounds.Y > 1)
                Render2D(barcode, outputStream);
            else
                throw new NotSupportedException($"Y value of {barcode.Bounds.Y} is invalid");
        }

        private static void Render1D(IBarcode barcode, Stream outputStream)
        {
            var document = SvgDocument.Create();
            document.ViewBox = new SvgViewBox
            {
                Left = 0,
                Top = 0,
                Width = barcode.Bounds.X + 2 * barcode.Margin,
                Height = 50
            };
            document.Fill = "#FFFFFF";
            document.Stroke = "#000000";
            document.StrokeWidth = 1;
            document.StrokeLineCap = SvgStrokeLineCap.Butt;

            var prevBar = false;
            for (var x = 0; x < barcode.Bounds.X; x++)
            {
                if (!barcode.At(x, 0))
                {
                    prevBar = false;
                    continue;
                }

                SvgLine line;
                if (prevBar)
                {
                    line = document.AddLine();
                    line.StrokeWidth = 1.5;
                    line.X1 = line.X2 = x + barcode.Margin - 0.25;
                    line.Y1 = 0;
                    line.Y2 = 50;
                }
                else
                {
                    line = document.AddLine();
                    line.X1 = line.X2 = x + barcode.Margin;
                    line.Y1 = 0;
                    line.Y2 = 50;
                }

                prevBar = true;
            }

            document.Save(outputStream);
        }

        private static void Render2D(IBarcode barcode, Stream outputStream)
        {
            var document = SvgDocument.Create();
            document.ViewBox = new SvgViewBox
            {
                Left = 0,
                Top = 0,
                Width = barcode.Bounds.X + 2 * barcode.Margin,
                Height = barcode.Bounds.Y + 2 * barcode.Margin
            };
            document.Fill = "#FFFFFF";
            document.Stroke = "#000000";
            document.StrokeWidth = .05;
            document.StrokeLineCap = SvgStrokeLineCap.Butt;

            SvgGroup group = document.AddGroup();
            group.Fill = "#000000";
            for (int y = 0; y < barcode.Bounds.Y; y++)
            {
                for (int x = 0; x < barcode.Bounds.X; x++)
                {
                    if (barcode.At(x, y))
                    {
                        SvgRect rect = group.AddRect();
                        rect.X = x + barcode.Margin;
                        rect.Y = y + barcode.Margin;
                        rect.Width = 1;
                        rect.Height = 1;
                    }
                }
            }

            document.Save(outputStream);
        }
    }
}
