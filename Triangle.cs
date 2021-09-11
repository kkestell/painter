using System;

namespace Painter
{
    internal class Triangle
    {
        // Constant Fields
        // Fields
        // Constructors

        public Triangle()
        {
            V1 = Vector2.Zero;
            V2 = Vector2.Zero;
            V3 = Vector2.Zero;
        }

        public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public Triangle(Triangle t)
        {
            V1 = t.V1;
            V2 = t.V2;
            V3 = t.V3;
        }

        // Finalizers (Destructors)
        // Delegates
        // Events
        // Enums
        // Interfaces
        // Properties

        public Vector2 V1 { get; }
        public Vector2 V2 { get; }
        public Vector2 V3 { get; }

        // Indexers
        // Methods

        public static Triangle Perturb(Triangle triangle, int radius, int width, int height)
        {
            var v1 = Vector2.Perturb(triangle.V1, radius);
            var v2 = Vector2.Perturb(triangle.V2, radius);
            var v3 = Vector2.Perturb(triangle.V3, radius);

            v1.Clamp(width, height);
            v2.Clamp(width, height);
            v3.Clamp(width, height);

            return new Triangle(v1, v2, v3);
        }

        public static Triangle Random(int size, int width, int height)
        {
            var rng = new Random();
            var position = Vector2.Random(width, height);

            var v1 = new Vector2(
                position.X + rng.Next(-size, size),
                position.Y + rng.Next(-size, size));
            var v2 = new Vector2(
                position.X + rng.Next(-size, size),
                position.Y + rng.Next(-size, size));
            var v3 = new Vector2(
                position.X + rng.Next(-size, size),
                position.Y + rng.Next(-size, size));

            v1.Clamp(width, height);
            v2.Clamp(width, height);
            v3.Clamp(width, height);

            return new Triangle(v1, v2, v3);
        }

        private static float Sign(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v1.X - v3.X) * (v2.Y - v3.Y) - (v2.X - v3.X) *
                   (v1.Y - v3.Y);
        }

        public bool Contains(Vector2 point)
        {
            var d1 = Sign(point, V1, V2);
            var d2 = Sign(point, V2, V3);
            var d3 = Sign(point, V3, V1);

            var hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
            var hasPos = d1 > 0 || d2 > 0 || d3 > 0;

            return !(hasNeg && hasPos);
        }

        public override string ToString()
        {
            return $"{V1.X},{V1.Y}, {V2.X},{V2.Y}, {V3.X},{V3.Y}";
        }

        // Structs
        // Classes
    }
}