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
        /// These paths should be relative to <see cref="InteractiveNode.NodeMarker"/>.
        /// <br/><br/>
        /// Obtained from <see cref="InteractiveNode.NodeMarker"/>'s
        /// <c>start_node_paths</c> metadata entry.
        /// </summary>
        public List<NodePath> StartNodePaths { get; private set; }

        /// <summary>
        /// The list of start nodes. Nodes in <see cref="NodesToTeleport"/>
        /// will get teleported by this <see cref="StartEndTeleporter"/>
        /// when they touch a node in this list.
        /// </summary>
        public List<Node> StartNodes { get; private set; }

        /// <summary>
        /// Nodes that can be teleported by this <see cref="StartEndTeleporter"/> 
        /// </summary>
        public List<Node> NodesToTeleport { get; private set; }

        public StartEndTeleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            StartNodePaths = new List<NodePath>();
            StartNodes = new List<Node>();
            NodesToTeleport = new List<Node>();

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

                PopulateStartNodesList();
            }
        }

        /// <summary>
        /// Removes a node from the <see cref="NodesToTeleport"/> list
        /// </summary>
        /// <param name="node">The node to remove</param>
        public void RemoveNodeToTeleport(Node node)
        {
            NodesToTeleport.Remove(node);
        }

        /// <summary>
        /// Adds a node to the <see cref="NodesToTeleport"/> list
        /// </summary>
        /// <param name="node">The node to add</param>
        public void AddNodeToTeleport(Node node)
        {
            if (node != null && NodesToTeleport.Contains(node) == false) NodesToTeleport.Add(node);
        }

        private void PopulateStartNodesList()
        {
            foreach (NodePath path in StartNodePaths)
            {
                RigidBody3D rigidBody = NodeMarker.GetNode<RigidBody3D>(path);
                if (rigidBody != null)
                {
                    StartNodes.Add(rigidBody);
                }
            }
        }

        private void HandleStartNodeTouch(Node body)
        {
            if (NodesToTeleport.Contains(body))
            {
                if (body is Node3D node3d)
                {
                    SendToDestination(node3d);
                }
            }
        }

        public override void Start()
        {
            if (IsRunning) return;

            base.Start();

            foreach (Node node in StartNodes)
            {
                if (node is RigidBody3D body)
                {
                    body.BodyEntered += HandleStartNodeTouch;
                }
            }
        }

        public override void Stop()
        {
            if (!IsRunning) return;

            base.Stop();

            foreach (RigidBody3D body in StartNodes)
            {
                body.BodyEntered -= HandleStartNodeTouch;
            }
        }
    }
}
