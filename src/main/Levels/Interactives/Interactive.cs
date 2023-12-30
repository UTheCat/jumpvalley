using System;
using System.Diagnostics;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// The base class for all Interactive types in Jumpvalley.
    /// </summary>
    public partial class Interactive : IDisposable
    {
        /// <summary>
        /// Whether or not <see cref="Initialize"/> has been called once already
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// The <see cref="Stopwatch"/> that the Interactive will run on
        /// </summary>
        public Stopwatch Clock { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Interactive"/> that will run on the given <see cref="Stopwatch"/>
        /// </summary>
        /// <param name="clock">The <see cref="Stopwatch"/> that the Interactive will run on</param>
        public Interactive(Stopwatch clock)
        {
            IsInitialized = false;
            Clock = clock;
        }

        /// <summary>
        /// Interactive initialization method that initializes the interactive after the constructor runs, in case such method is needed.
        /// <br/>
        /// By default, this method is only called once after the object's constructor runs.
        /// Initialize() typically shouldn't be called more than once for the same <see cref="Interactive"/> instance.
        /// </summary>
        public virtual void Initialize()
        {
            // This check should typically be at the very beginning of Initialize()
            if (IsInitialized) return;

            IsInitialized = true;
        }

        /// <summary>
        /// The interactive's start method. This method is called every time the user starts or restarts the interactive,
        /// and it's a great place to put code that will be run after initialization, but just before the interactive starts.
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// The interactive's stop method. This method is called right after the user stops or exits the interactive.
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// Disposes of this <see cref="Interactive"/> instance. This method is a great place to free up resources being used by the interactive instance,
        /// especially right before the interactive itself gets freed from memory.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
