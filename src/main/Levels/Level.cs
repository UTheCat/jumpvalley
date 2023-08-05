using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level that's playable in Jumpvalley.
    /// <br/>
    /// Each level contains four primary components: interactives, music, static objects, and start point.
    /// More details can be found on Jumpvalley's wiki article on <see href="https://github.com/UTheDev/jumpvalley/wiki/Level-Layout">Level Layout</see>.
    /// </summary>
    public partial class Level : Interactive, IDisposable
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";

        /// <summary>
        /// Indicates the current run state of the level.
        /// </summary>
        public enum RunState
        {
            /// <summary>
            /// Indicates that the level is not running
            /// </summary>
            Stopped = 0,

            /// <summary>
            /// Indicates that the level is still running (in the background) but is paused
            /// </summary>
            Paused = 1,

            /// <summary>
            /// Indicates that the level is actively running and not paused
            /// </summary>
            Playing = 2
        }

        /// <summary>
        /// Information about the level that's specified in the level's info file.
        /// This includes things like level ID and full name.
        /// </summary>
        public LevelInfoFile Info { get; private set; }

        /// <summary>
        /// The root node of the level
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The node containing the level's interactives
        /// </summary>
        public Node Interactives { get; private set; }

        /// <summary>
        /// The node containing the level's music
        /// </summary>
        public Node Music { get; private set; }

        /// <summary>
        /// The node containing the level's static objects
        /// </summary>
        public Node StaticObjects { get; private set; }

        /// <summary>
        /// The Node3D that defines the position and rotation that the player's character will be set to when they play the level from the very beginning
        /// </summary>
        public Node3D StartPoint { get; private set; }

        /// <summary>
        /// The level's current run state
        /// </summary>
        public RunState CurrentRunState { get; private set; }

        /// <summary>
        /// Set externally to help the level's code access whatever object is running the level itself
        /// </summary>
        public LevelRunner Runner = null;

        /// <summary>
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its info file
        /// </summary>
        /// <param name="node">The root node of the level to represent</param>
        public Level(LevelInfoFile info, Node root): base(new Stopwatch())
        {
            Info = info;
            RootNode = root;

            CurrentRunState = RunState.Stopped;
        }

        /// <summary>
        /// Level initialization method that initializes the level after the constructor runs, in case such method is needed.
        /// <br/>
        /// By default, this method is only called once after the object's constructor runs.
        /// Initialize() typically shouldn't be called more than once for the same <see cref="Level"/> instance.
        /// </summary>
        public override void Initialize()
        {
            if (IsInitialized) return;

            base.Initialize();
        }

        /// <summary>
        /// The level's start method. This method is called every time the user starts or restarts the level,
        /// and it's a great place to put code that will be run after initialization, but just before the level starts.
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// The level's stop method. This method is called right after the user stops or exits the level.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// Disposes of this <see cref="Level"/> instance. This method is a great place to free up resources being used by the level instance,
        /// especially right before the level itself gets freed from memory.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
