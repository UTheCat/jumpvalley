using Godot;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// Interactive that sends a Node3D to a defined destination.
    /// </summary>
    public partial class Teleporter : InteractiveNode
    {
        /// <summary>
        /// Whether or not the Node3D being teleported should have
        /// its rotation set to the rotation of <see cref="Destination"/> 
        /// </summary>
        public bool ShouldSetRotation;

        /// <summary>
        /// Where <see cref="Node3D"/>s being teleported by this teleporter
        /// should be sent
        /// </summary>
        public Node3D Destination;

        public Teleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }
    }
}
