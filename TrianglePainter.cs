using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Painter
{
    internal class TrianglePainter
    {
        // Constant Fields
        // Fields
        // Constructors
        // Finalizers (Destructors)
        // Delegates
        // Events
        // Enums
        // Interfaces
        // Properties
        // Indexers
        // Methods

        private static void FindTriangle(object stateObject)
        {
            if (!(stateObject is State state)) return;

            var width = ((State)stateObject).ImageWidth;
            var height = ((State)stateObject).ImageHeight;

            var bestTriangle = Triangle.Random(64, width, height);
            var bestScore = double.MaxValue;

            for (var t = 0; t < 128; t++)
            {
                // FIXME
                var size = (128 - t) / 2 + 64;
                var newTriangle = Triangle.Random(size, width, height);

                var newImage = state.CanvasImage.Clone();

                newImage.DrawTriangle(
                    newTriangle,
                    state.SourceImage.AverageColor(newTriangle));

                var newScore = Image.Similarity(
                    state.SourceImage,
                    newImage);

                if (!(newScore < bestScore)) continue;

                bestScore = newScore;
                bestTriangle = newTriangle;
            }

            state.BestScore = bestScore;
            state.BestTriangle = bestTriangle;

            state.EventWaitHandle.Set();
        }

        private static void PerformPerturbation(object stateObject)
        {
            if (!(stateObject is State state)) return;

            var newTriangle = state.InitialState;
            var bestTriangle = newTriangle;
            var bestScore = double.MaxValue;

            for (var p = 0; p < 64; p++)
            {
                var newImage = state.CanvasImage.Clone();

                var width = ((State)stateObject).ImageWidth;
                var height = ((State)stateObject).ImageHeight;

                var perturbedTriangle = Triangle.Perturb(bestTriangle, 1, width, height);

                newImage.DrawTriangle(
                    perturbedTriangle,
                    state.SourceImage.AverageColor(newTriangle));

                var newScore = Image.Similarity(
                    state.SourceImage,
                    newImage);

                if (!(newScore < bestScore)) continue;

                bestScore = newScore;
                bestTriangle = perturbedTriangle;
            }

            state.BestScore = bestScore;
            state.BestTriangle = bestTriangle;

            state.EventWaitHandle.Set();
        }

        public void Run(Arguments args)
        {
            var sourceImage = new Image(args.InputFilePath);

            var canvasImage = new Image(sourceImage.Width, sourceImage.Height);
            canvasImage.Fill(sourceImage.AverageColor());

            var svg = new SVG(sourceImage.Width, sourceImage.Height);
            svg.Fill(sourceImage.AverageColor());

            var bestScore = double.MaxValue;

            for (var t1 = 0; t1 < args.IterationCount; t1++)
            {
                // Find a triangle

                var triangleThreads = new List<State>();

                for (var t2 = 0; t2 < 4; t2++)
                {
                    var state = new State
                    {
                        SourceImage = sourceImage,
                        CanvasImage = canvasImage,
                        ImageWidth = sourceImage.Width,
                        ImageHeight = sourceImage.Height
                    };

                    ThreadPool.QueueUserWorkItem(FindTriangle, state);
                    triangleThreads.Add(state);
                }

                WaitHandle.WaitAll(triangleThreads.Select(t =>
                    t.EventWaitHandle).ToArray());

                var bestTriangleMatch = triangleThreads.OrderBy(t =>
                    t.BestScore).First();

                // Perturb triangle

                var pertubationThreads = new List<State>();

                for (var t2 = 0; t2 < 4; t2++)
                {
                    var state = new State
                    {
                        SourceImage = sourceImage,
                        CanvasImage = canvasImage,
                        InitialState = bestTriangleMatch.BestTriangle,
                        ImageWidth = sourceImage.Width,
                        ImageHeight = sourceImage.Height
                    };

                    ThreadPool.QueueUserWorkItem(PerformPerturbation, state);
                    pertubationThreads.Add(state);
                }

                WaitHandle.WaitAll(pertubationThreads.Select(t =>
                    t.EventWaitHandle).ToArray());

                // Get winning pertubation

                var bestPertubationMatch = pertubationThreads.OrderBy(t =>
                    t.BestScore).First();

                if (bestPertubationMatch.BestScore < bestScore)
                {
                    bestScore = bestPertubationMatch.BestScore;

                    canvasImage.DrawTriangle(
                        bestPertubationMatch.BestTriangle,
                        sourceImage.AverageColor(
                            bestPertubationMatch.BestTriangle));

                    svg.DrawTriangle(
                        bestPertubationMatch.BestTriangle,
                        sourceImage.AverageColor(
                            bestPertubationMatch.BestTriangle));

                    Console.WriteLine($"{t1:D4} {bestScore}");
                }

                if (args.SaveIntermediateImages)
                    TrySaveImages(canvasImage, svg, $"output{t1:D4}");
            }

            TrySaveImages(canvasImage, svg, args.OutputFilePath);
        }

        // File I/O occasionally leads to transient failures here, so we retry
        // a reasonable number of times then bomb out if unsuccessful.
        private void TrySaveImages(Image canvasImage, SVG svg, string fileName)
        {
            var retryCount = 0;
            while (retryCount < 10)
            {
                try
                {
                    canvasImage.Save($"{fileName}.png");
                    svg.Save($"{fileName}.svg");
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception occurred while saving file: {e}");
                }
            }

            Console.WriteLine("Unable to save file after 10 retries. Aborting.");
            Environment.Exit(1);
        }

        // Structs
        // Classes
    }
}