using System;

namespace Painter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Painter <image>");
                return;
            }

            new TrianglePainter().Run(args[0]);
        }
    }
}
