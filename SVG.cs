using SixLabors.ImageSharp.PixelFormats;
using Svg;
using System.Drawing;

namespace Painter
{
    internal class SVG
    {
        private readonly SvgDocument svg;
        private readonly SvgGroup svgGroup;
        private readonly int inputWidth;
        private readonly int inputHeight;

        internal SVG(int inputWidth, int inputHeight)
        {
            this.inputWidth = inputWidth;
            this.inputHeight = inputHeight;

            svg = new SvgDocument
            {
                Width = inputWidth,
                Height = inputHeight
            };

            svgGroup = new SvgGroup();
            svg.Children.Add(svgGroup);
        }

        internal void DrawTriangle(Triangle triangle, Rgba32 color)
        {
            svgGroup.Children.Add(new SvgPolygon
            {
                Points = new SvgPointCollection
                {
                    new SvgUnit(triangle.V1.X), new SvgUnit(triangle.V1.Y),
                    new SvgUnit(triangle.V2.X), new SvgUnit(triangle.V2.Y),
                    new SvgUnit(triangle.V3.X), new SvgUnit(triangle.V3.Y),
                    new SvgUnit(triangle.V1.X), new SvgUnit(triangle.V1.Y),
                },
                Fill = new SvgColourServer(Color.FromArgb(color.A, color.R, color.G, color.B))
            });
        }

        internal void Fill(Rgba32 color)
        {
            svgGroup.Children.Add(new SvgPolygon
            {
                Points = new SvgPointCollection
                {
                    new SvgUnit(0), new SvgUnit(0),
                    new SvgUnit(inputWidth), new SvgUnit(0),
                    new SvgUnit(inputWidth), new SvgUnit(inputHeight),
                    new SvgUnit(0), new SvgUnit(inputHeight),
                    new SvgUnit(0), new SvgUnit(0),
                },
                Fill = new SvgColourServer(Color.FromArgb(color.A, color.R, color.G, color.B))
            });
        }

        internal void Save(string filePath)
        {
            svg.Write(filePath);
        }
    }
}
