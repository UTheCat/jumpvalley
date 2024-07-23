using Godot;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics
{
    /// <summary>
    /// Interactive that handles checkpoints for a level.
    /// </summary>
    public partial class CheckpointSet : InteractiveNode
    {
        public CheckpointSet(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }
    }
}
