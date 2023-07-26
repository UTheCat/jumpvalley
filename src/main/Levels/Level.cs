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
    public partial class Level
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";

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
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its root node
        /// </summary>
        /// <param name="node">The root node of the level to represent</param>
        public Level(Node node)
        {
            Interactives = node.GetNode(INTERACTIVES_NODE_NAME);
            Music = node.GetNode(MUSIC_NODE_NAME);
            StaticObjects = node.GetNode(STATIC_OBJECTS_NODE_NAME);
        }
    }
}
