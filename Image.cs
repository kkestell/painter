using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Painter
{
    internal class Image
    {
        // Constant Fields

        public const int Size = 256;

        // Fields

        private readonly Image<Rgba32> image;

        // Constructors

        public Image(string filename)
        {
            image = SixLabors.ImageSharp.Image.Load(filename);
            image.Mutate(i => i.Resize(Size, Size));
        }

        public Image()
        {
            image = new Image<Rgba32>(Size, Size);
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

        public Rgba32 this[int x, int y] => image[x, y];

        public int Width => image.Width;

        public int Height => image.Height;

        // Indexers
        // Methods

        public static double Similarity(Image image1, Image image2)
        {
            // Root-mean-square deviation

            long sum = 0;
            long n = 0;

            for (var y = 0; y < image1.Height; y++)
            for (var x = 0; x < image1.Width; x++)
            {
                var d =
                    (long) Math.Pow(image1[x, y].R - image2[x, y].R, 2) +
                    (long) Math.Pow(image1[x, y].G - image2[x, y].G, 2) +
                    (long) Math.Pow(image1[x, y].B - image2[x, y].B, 2);
                sum += d;
                n++;
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

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var color = image[x, y];
                r += color.R;
                g += color.G;
                b += color.B;
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

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
                if (region.Contains(new Vector2(x, y)))
                {
                    var color = image[x, y];
                    r += color.R;
                    g += color.G;
                    b += color.B;
                    cnt++;
                }

            if (cnt == 0) return Rgba32.Black;

            return new Rgba32(
                (byte) (r / cnt),
                (byte) (g / cnt),
                (byte) (b / cnt));
        }

        public void Save(string filename)
        {
            image.Save(filename);
        }

        public void DrawTriangle(Triangle triangle, Rgba32 color)
        {
            image.Mutate(
                i => i.FillPolygon(
                    color, triangle.V1, triangle.V2, triangle.V3));
        }

        public Image Clone()
        {
            return new Image(image.Clone());
        }

        // Structs
        // Classes
    }
}