using System;
using SixLabors.Primitives;

namespace Painter
{
    public struct Vector2
    {
        // Constant Fields
        // Fields
        // Constructors

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Finalizers(Destructors)
        // Delegates
        // Events
        // Enums
        // Interfaces
        // Properties

        public float X { get; set; }

        public float Y { get; set; }

        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        // Indexers
        // Methods

        public static Vector2 Perturb(Vector2 v, int radius)
        {
            var angle = new Random().NextDouble() * Math.PI * 2;

            return new Vector2(
                v.X + ((float)Math.Cos(angle) * radius),
                v.Y + ((float)Math.Sin(angle) * radius));
        }

        public static Vector2 Random()
        {
            var rng = new Random();

            return new Vector2(
                rng.Next(0, Image.Size),
                rng.Next(0, Image.Size));
        }

        public static implicit operator PointF(Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        public void Clamp()
        {
            if (X > Image.Size)
                X = Image.Size;

            if (X < 0)
                X = 0;

            if (Y > Image.Size)
                Y = Image.Size;

            if (Y < 0)
                Y = 0;
        }

        // Structs
        // Classes
    }
}
