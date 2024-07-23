using System.Collections.Generic;
using Godot;
using Jumpvalley.Levels.Interactives.Mechanics.Teleporters;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics
{
    /// <summary>
    /// Interactive that handles checkpoints for a level.
    /// </summary>
    public partial class CheckpointSet : InteractiveNode
    {
        /// <summary>
        /// The set's checkpoints (typically a level's checkpoints)
        /// </summary>
        public List<Teleporter> Checkpoints;

        /// <summary>
        /// The checkpoint that the player is currently on
        /// </summary>
        public int CurrentCheckpoint;

        /// <summary>
        /// The node that gets sent to one of the checkpoints in the set.
        /// </summary>
        public Node3D NodeToTeleport;

        public CheckpointSet(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }
    }
}
