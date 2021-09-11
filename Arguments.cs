namespace Painter
{
    internal class Arguments
    {
        internal Arguments(
            string inputFilePath,
            string outputFilePath,
            int iterationCount,
            bool saveIntermediateImages)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            IterationCount = iterationCount;
            SaveIntermediateImages = saveIntermediateImages;
        }

        internal string InputFilePath { get; }

        internal string OutputFilePath { get; }

        internal int IterationCount { get; }

        internal bool SaveIntermediateImages { get; }
    }
}
