using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace Painter
{
    internal class Image
    {
        // Constant Fields
        const int ResizeHeight = 256;

        // Fields

        private readonly Image<Rgba32> image;

        // Constructors

        public Image(string filename)
        {
            image = (Image<Rgba32>)SixLabors.ImageSharp.Image.Load(filename);

            var ratio = ResizeHeight / image.Height;
            var newWidth = image.Width * ratio;

            image.Mutate(i => i.Resize(newWidth, ResizeHeight));
        }

        public Image(int inputWidth, int inputHeight)
        {
            image = new Image<Rgba32>(inputWidth, inputHeight);
        }

        public Image(Image<Rgba32> image)
        {
            this.image = image;
        }

        // Finalizers (Destructors)
        // Delegates
        // Events
        // Enums
        // Interfaces
        // Properties

        public int Width => image.Width;

        public int Height => image.Height;

        // Indexers
        // Methods

        public static double Similarity(Image image1, Image image2)
        {
            // Root-mean-square deviation

            long sum = 0;
            long n = 0;

            for (int y = 0; y < image1.Height; y++)
            {
                Span<Rgba32> image1PixelRowSpan = image1.image.GetPixelRowSpan(y);
                Span<Rgba32> image2PixelRowSpan = image2.image.GetPixelRowSpan(y);

                for (int x = 0; x < image1.Width; x++)
                {
                    var d =
                        (long)Math.Pow(image1PixelRowSpan[x].R - image2PixelRowSpan[x].R, 2) +
                        (long)Math.Pow(image1PixelRowSpan[x].G - image2PixelRowSpan[x].G, 2) +
                        (long)Math.Pow(image1PixelRowSpan[x].B - image2PixelRowSpan[x].B, 2);
                    sum += d;
                    n++;
                }
            }

            return Math.Sqrt(sum / n);
        }

        public void Fill(Rgba32 color)
        {
            image.Mutate(
                i => i.Fill(
                    color,
                    new Rectangle(
                        0,
                        0,
                        image.Width,
                        image.Height)));
        }

        public Rgba32 AverageColor()
        {
            long r = 0, g = 0, b = 0;

            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> imagePixelRowSpan = image.GetPixelRowSpan(y);

                for (int x = 0; x < image.Width; x++)
                {
                    var color = imagePixelRowSpan[x];
                    r += color.R;
                    g += color.G;
                    b += color.B;
                }
            }

            var cnt = image.Width * image.Height;
            return new Rgba32(
                (byte) (r / cnt),
                (byte) (g / cnt),
                (byte) (b / cnt));
        }

        public Rgba32 AverageColor(Triangle region)
        {
            long r = 0, g = 0, b = 0, cnt = 0;

            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> imagePixelRowSpan = image.GetPixelRowSpan(y);

                for (int x = 0; x < image.Width; x++)
                {
                    if (region.Contains(new Vector2(x, y)))
                    {
                        var color = imagePixelRowSpan[x];
                        r += color.R;
                        g += color.G;
                        b += color.B;
                        cnt++;
                    }
                }
            }

            if (cnt == 0) return Color.Black;

            return new Rgba32(
                (byte) (r / cnt),
                (byte) (g / cnt),
                (byte) (b / cnt));
        }

        public void Save(string filename)
        {
            image.Save(filename);
        }

        public void DrawTriangle(Triangle triangle, Color color)
        {
            image.Mutate(
                i => i.FillPolygon(
                    color,
                    triangle.V1, triangle.V2, triangle.V3));
        }

        public Image Clone()
        {
            return new Image(image.Clone());
        }

        // Structs
        // Classes
    }
}