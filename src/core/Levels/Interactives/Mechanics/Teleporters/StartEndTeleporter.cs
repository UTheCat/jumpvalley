using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics.Teleporters
{
    /// <summary>
    /// Type of teleporter that teleports a node to <see cref="Teleporter.Destination"/>
    /// when the node touches specified parts.
    /// </summary>
    public partial class StartEndTeleporter : Teleporter
    {
        private static readonly string START_NODE_PATHS_META_NAME = "start_node_paths";
        
        /// <summary>
        /// The list of node paths to the teleporter's start nodes.
        /// Obtained from <see cref="InteractiveNode.NodeMarker"/>'s
        /// <c>start_node_paths</c> metadata entry.
        /// </summary>
        public List<NodePath> StartNodePaths { get; private set; }

        public StartEndTeleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            StartNodePaths = new List<NodePath>();

            Array startNodesMetadata;
            if (TryGetMarkerMeta<Array>(START_NODE_PATHS_META_NAME, out startNodesMetadata))
            {
                foreach (Variant item in startNodesMetadata)
                {
                    NodePath path = item.As<NodePath>();
                    if (path != null)
                    {
                        StartNodePaths.Add(path);
                    }
                }
            }
        }
    }
}
