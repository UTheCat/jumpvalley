using Godot;

using Jumpvalley.Levels.Interactives;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Mechanics.Teleporters
{
    /// <summary>
    /// Interactive that prepares an object to be teleported by an
    /// instance of <see cref="Teleporter"/>.
    /// <br/><br/>
    /// This class primarily supplies some data to help
    /// the root node of a <see cref="TeleportableObject"/>
    /// be teleported to the correct point.
    /// </summary>
    public partial class TeleportableObject : InteractiveNode
    {
        /// <summary>
        /// The bounding box of all the <see cref="VisualInstance3D"/>s
        /// that are a descendant of this <see cref="TeleportableObject"/>'s
        /// root node.
        /// </summary>
        public Aabb OverallBoundingBox;

        public TeleportableObject(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }
    }
}
