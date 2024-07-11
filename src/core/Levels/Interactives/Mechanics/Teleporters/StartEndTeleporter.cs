using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics.Teleporters
{
    /// <summary>
    /// Type of teleporter that teleports a node to <see cref="Teleporter.Destination"/>
    /// when the node touches specified parts called start nodes.
    /// <br/><br/>
    /// Start nodes should be <see cref="Area3D"/>s.  
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
        public List<Node3D> StartNodes { get; private set; }

        /// <summary>
        /// Nodes that can be teleported by this <see cref="StartEndTeleporter"/> 
        /// </summary>
        public List<Node3D> NodesToTeleport { get; private set; }

        public StartEndTeleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            StartNodePaths = new List<NodePath>();
            StartNodes = new List<Node3D>();
            NodesToTeleport = new List<Node3D>();

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
        public void RemoveNodeToTeleport(Node3D node)
        {
            NodesToTeleport.Remove(node);
        }

        /// <summary>
        /// Adds a node to the <see cref="NodesToTeleport"/> list
        /// </summary>
        /// <param name="node">The node to add</param>
        public void AddNodeToTeleport(Node3D node)
        {
            if (node != null && NodesToTeleport.Contains(node) == false) NodesToTeleport.Add(node);
        }

        private void PopulateStartNodesList()
        {
            foreach (NodePath path in StartNodePaths)
            {
                Area3D area = NodeMarker.GetNode<Area3D>(path);
                if (area != null)
                {
                    StartNodes.Add(area);
                }
            }
        }

        private void HandleStartNodeTouch(Node3D body)
        {
            System.Console.WriteLine(body.Name);
            if (NodesToTeleport.Contains(body))
            {
                System.Console.WriteLine($"Teleporting {body.Name} to {GetDestinationPoint(body)}");
                SendToDestination(body);
            }
        }

        public override void Start()
        {
            if (IsRunning) return;

            base.Start();

            foreach (Node3D node in StartNodes)
            {
                if (node is Area3D area)
                {
                    area.BodyEntered += HandleStartNodeTouch;
                }
            }
        }

        public override void Stop()
        {
            if (!IsRunning) return;

            base.Stop();

            foreach (Node3D node in StartNodes)
            {
                if (node is Area3D area)
                {
                    area.BodyEntered -= HandleStartNodeTouch;
                }
            }
        }
    }
}
