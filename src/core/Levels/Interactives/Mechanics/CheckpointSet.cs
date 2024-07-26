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
        public static readonly string CHECKPOINTS_META_NAME = "checkpoints";
        public static readonly string CURRENT_CHECKPOINT_META_NAME = "current_checkpoint";

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

        private void UpdateCheckpointsFromNodePathsList()
        {
            Node marker = NodeMarker;
            if (marker != null)
            {
                Godot.Collections.Array paths;
                if (TryGetMarkerMeta<Godot.Collections.Array>(CHECKPOINTS_META_NAME, out paths))
                {
                    foreach (object path in paths)
                    {
                        if (path is NodePath nodePath)
                        {
                            Node checkpointMarker = marker.GetNode(nodePath);

                            if (checkpointMarker != null)
                            {
                                Interactive interactive = InteractiveNode.GetHandlerFromMarker(checkpointMarker);

                                if (interactive is Teleporter teleporter)
                                {
                                    Checkpoints.Add(teleporter);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
