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

            var bestTriangle = Triangle.Random(64);
            var bestScore = double.MaxValue;

            for (var t = 0; t < 128; t++)
            {
                // FIXME
                var size = (128 - t) / 2 + 64;
                var newTriangle = Triangle.Random(size);

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

                var perturbedTriangle = Triangle.Perturb(bestTriangle, 1);

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

        public void Run(string filename)
        {
            var sourceImage = new Image(filename);

            var canvasImage = new Image();
            canvasImage.Fill(sourceImage.AverageColor());

            var bestScore = double.MaxValue;

            for (var t1 = 0; t1 < 256; t1++)
            {
                // Find a triangle

                var triangleThreads = new List<State>();

                for (var t2 = 0; t2 < 4; t2++)
                {
                    var state = new State
                    {
                        SourceImage = sourceImage,
                        CanvasImage = canvasImage
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
                        InitialState = bestTriangleMatch.BestTriangle
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

                    Console.WriteLine($"{t1:D4} {bestScore}");
                }

                canvasImage.Save($"output{t1:D4}.png");
            }

            canvasImage.Save("output.png");
        }

        // Structs
        // Classes
    }
}