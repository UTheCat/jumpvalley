using Godot;
using System;

using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// The base class for all Interactive types in Jumpvalley.
    /// </summary>
    public partial class Interactive : Node, IDisposable
    {
        /// <summary>
        /// Whether or not <see cref="Initialize"/> has been called once already
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Whether or not the interactive is running (e.g. after <see cref="Start"/> has been called).
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// The <see cref="OffsetStopwatch"/> that the Interactive will run on.
        /// This stopwatch can be used to synchronize the Interactive's various operations to whatever the current time
        /// on this stopwatch is.
        /// </summary>
        public OffsetStopwatch Clock { get; private set; }

        /// <summary>
        /// The object that's running this interactive.
        /// </summary>
        public object Runner = null;

        /// <summary>
        /// Creates a new instance of <see cref="Interactive"/> that will run on the given <see cref="OffsetStopwatch"/>.
        /// </summary>
        /// <param name="clock">The <see cref="Stopwatch"/> that the Interactive will run on</param>
        public Interactive(OffsetStopwatch clock)
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
        public virtual void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
        }

        /// <summary>
        /// The interactive's stop method. This method is called right after the user stops or exits the interactive.
        /// </summary>
        public virtual void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
        }

        /// <summary>
        /// Disposes of this <see cref="Interactive"/> instance. This method is a great place to free up resources being used by the interactive instance,
        /// especially right before the interactive itself gets freed from memory.
        /// </summary>
        public new void Dispose()
        {
            QueueFree();
            GC.SuppressFinalize(this);
        }
    }
}
