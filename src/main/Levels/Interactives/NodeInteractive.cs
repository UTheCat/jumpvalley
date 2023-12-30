using Godot;
using System.Diagnostics;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// A subclass of <see cref="Interactive"/> that operates over a Godot node.
    /// It makes using a Godot node's properties and metadata easier and provides events to respond to changes in a node's properties or attributes.
    /// </summary>
    public partial class NodeInteractive: Interactive
    {
        public NodeInteractive(Stopwatch stopwatch, Node node) : base(stopwatch)
        {

        }
    }
}
