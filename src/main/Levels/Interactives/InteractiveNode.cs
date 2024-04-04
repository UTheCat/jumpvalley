using Godot;
using System;

using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// A subclass of <see cref="Interactive"/> that operates over a Godot node.
    /// It makes using a Godot node's properties and metadata easier.
    /// </summary>
    public partial class InteractiveNode: Interactive
    {
        /// <summary>
        /// The Godot node being operated on
        /// </summary>
        public Node ActualNode { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="InteractiveNode"/> for a given <see cref="Stopwatch"/> and <see cref="Node"/>
        /// </summary>
        /// <param name="stopwatch">The stopwatch to bind the interactive to</param>
        /// <param name="node">The node to operate over</param>
        public InteractiveNode(OffsetStopwatch stopwatch, Node node) : base(stopwatch)
        {
            if (node == null) throw new ArgumentNullException("node");

            ActualNode = node;
        }

        /// <summary>
        /// Event that's raised when one of the metadata of the node changes.
        /// </summary>
        //public event EventHandler<NodeMetadataChangedArgs> NodeMetadataChanged;
    }
}
