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
        /// The node which indicates that its parent node is the root node of an interactive.
        /// Its name should begin with <c>_Interactive</c>
        /// </summary>
        public Node NodeMarker { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="InteractiveNode"/> for a given <see cref="Stopwatch"/> and <see cref="Node"/>
        /// </summary>
        /// <param name="stopwatch">The stopwatch to bind the interactive to</param>
        /// <param name="node">The node to operate over</param>
        public InteractiveNode(OffsetStopwatch stopwatch, Node nodeMarker) : base(stopwatch)
        {
            if (nodeMarker == null) throw new ArgumentNullException(nameof(nodeMarker));

            NodeMarker = nodeMarker;
        }

        /// <summary>
        /// Event that's raised when one of the metadata of the node changes.
        /// </summary>
        //public event EventHandler<NodeMetadataChangedArgs> NodeMetadataChanged;
    }
}
