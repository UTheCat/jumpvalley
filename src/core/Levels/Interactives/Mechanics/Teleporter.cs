using Godot;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// Interactive that sends a Node3D to its destination.
    /// </summary>
    public partial class Teleporter : InteractiveNode
    {
        public Teleporter(OffsetStopwatch stopwatch, Node actualNode) : base(stopwatch, actualNode) { }
    }
}
