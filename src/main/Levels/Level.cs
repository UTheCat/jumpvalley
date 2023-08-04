using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level that's playable in Jumpvalley.
    /// <br/>
    /// Each level contains three primary components: the interactives, the music, and the static objects.
    /// More details can be found on Jumpvalley's wiki article on <see href="https://github.com/UTheDev/jumpvalley/wiki/Level-Layout">Level Layout</see>.
    /// </summary>
    public partial class Level: IDisposable
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";

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
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its info file
        /// </summary>
        /// <param name="node">The root node of the level to represent</param>
        public Level(LevelInfoFile info, Node root)
        {
            Info = info;
            RootNode = root;
        }

        /// <summary>
        /// Level initialization method that initializes the level after the constructor runs, in case such method is needed.
        /// <br/>
        /// By default, this method is only called once after the object's constructor runs.
        /// Initialize() typically shouldn't be called more than once for the same <see cref="Level"/> instance.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// The level's start method. This method is called every time the user starts or restarts the level,
        /// and it's a great place to put code that will be run after initialization, but just before the level starts.
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// The level's stop method. This method is called right after the user stops or exits the level.
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// Disposes of this <see cref="Level"/> instance. This method is a great place to free up resources being used by the level instance,
        /// especially right before the level itself gets freed from memory.
        /// </summary>
        public void Dispose()
        {
            RootNode.QueueFree();
            RootNode.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
