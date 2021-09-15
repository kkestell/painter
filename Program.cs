using System;
using System.Diagnostics;

namespace Painter
{
    class Program
    {
        static void Main(string[] args)
        {
            Arguments arguments;
            try
            {
                arguments = new Arguments(
                    args[0],
                    args[1],
                    int.Parse(args[2]),
                    bool.Parse(args[3]));
            }
            catch (Exception)
            {
                Console.WriteLine("Usage: Painter <inputImagePath> <outputImagePath> <iterationCount> <saveIntermediateImages (true/false)>");
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            new TrianglePainter().Run(arguments);

            stopwatch.Stop();
            Console.WriteLine($"Operation completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
