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
        /// should be sent.
        /// <br/><br/>
        /// If this node is a <see cref="MeshInstance3D"/> and has a metadata entry named <c>teleports_on_top</c> set to true,
        /// objects sent to this destination will be teleported on top of this node.
        /// </summary>
        public Node3D Destination;

        public Teleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }

        /// <summary>
        /// Sends a node to the destination specified in <see cref="Destination"/> 
        /// </summary>
        /// <param name="node">The node to send to the destination</param>
        public void SendToDestination(Node3D node)
        {

        }
    }
}
