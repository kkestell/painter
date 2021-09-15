using System.Threading;

namespace Painter
{
    class State
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

        public EventWaitHandle EventWaitHandle { get; private set; } = new ManualResetEvent(false);

        public double BestScore { get; set; }

        public Triangle BestTriangle { get; set; }

        public Image CanvasImage { get; set; }

        public Image SourceImage { get; set; }

        public Triangle InitialState { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        // Indexers
        // Methods
        // Structs
        // Classes
    }
}
