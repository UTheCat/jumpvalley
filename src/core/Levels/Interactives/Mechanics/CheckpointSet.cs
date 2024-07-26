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

        private int _currentCheckpoint;

        /// <summary>
        /// The checkpoint that the player is currently on
        /// </summary>
        public int CurrentCheckpoint
        {
            get => _currentCheckpoint;
            set
            {
                _currentCheckpoint = value;
                SetMarkerMeta(CURRENT_CHECKPOINT_META_NAME, value);
            }
        }

        public CheckpointSet(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            Checkpoints = new List<Teleporter>();

            int currentCheckpoint;
            if (TryGetMarkerMeta<int>(CURRENT_CHECKPOINT_META_NAME, out currentCheckpoint))
            {
                CurrentCheckpoint = currentCheckpoint;
            }

            UpdateCheckpointsFromNodePathsList();
        }

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

        /// <summary>
        /// Teleports a specified node to the current checkpoint
        /// </summary>
        /// <param name="node">The node to teleport</param>
        public void SendToCurrentCheckpoint(Node3D node)
        {
            Checkpoints[CurrentCheckpoint]?.SendToDestination(node);
        }
    }
}
