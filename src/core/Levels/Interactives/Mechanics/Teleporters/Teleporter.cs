using Godot;
using UTheCat.Jumpvalley.Core.Timing;

namespace UTheCat.Jumpvalley.Core.Levels.Interactives.Mechanics.Teleporters
{
    /// <summary>
    /// Interactive that sends a Node3D to a defined destination.
    /// </summary>
    public partial class Teleporter : InteractiveNode
    {
        private static readonly string SHOULD_SET_ROTATION_META_NAME = "should_set_rotation";
        private static readonly string TELEPORTS_ON_TOP_META_NAME = "teleports_on_top";
        private static readonly string DESTINATION_NODE_PATH_META_NAME = "destination_node_path";

        private bool _shouldSetRotation;

        /// <summary>
        /// Whether or not the Node3D being teleported should have
        /// its rotation set to the global-space rotation of <see cref="Destination"/> 
        /// </summary>
        public bool ShouldSetRotation
        {
            get => _shouldSetRotation;
            set
            {
                _shouldSetRotation = value;
                SetMarkerMeta(SHOULD_SET_ROTATION_META_NAME, value);
            }
        }

        private bool _teleportsOnTop;

        /// <summary>
        /// Whether or not nodes being sent to <see cref="Destination"/>
        /// should be teleported on top of <see cref="Destination"/>.
        /// <br/><br/>
        /// If <see cref="Destination"/> has height, <see cref="Destination"/>
        /// will have to be a <see cref="VisualInstance3D"/> in order to work properly. 
        /// </summary>
        public bool TeleportsOnTop
        {
            get => _teleportsOnTop;
            set
            {
                _teleportsOnTop = value;
                SetMarkerMeta(TELEPORTS_ON_TOP_META_NAME, value);
            }
        }

        private NodePath _destinationNodePath;

        /// <summary>
        /// The node path to the teleporter's destination node (relative to <see cref="NodeMarker"/>).
        /// This should be <see cref="null"/> if you want to set <see cref="Destination"/> manually.
        /// <br/><br/>
        /// Setting this to null will not set <see cref="Destination"/> to null. 
        /// </summary>
        public NodePath DestinationNodePath
        {
            get => _destinationNodePath;
            set
            {
                _destinationNodePath = value;

                if (_destinationNodePath != null)
                {
                    Destination = NodeMarker.GetNode<Node3D>(value);
                }
            }
        }

        /// <summary>
        /// Where <see cref="Node3D"/>s being teleported by this teleporter
        /// should be sent.
        /// </summary>
        public Node3D Destination;

        public Teleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            bool shouldSetRotation;
            if (TryGetMarkerMeta(SHOULD_SET_ROTATION_META_NAME, out shouldSetRotation))
            {
                ShouldSetRotation = shouldSetRotation;
            }

            bool teleportsOnTop;
            if (TryGetMarkerMeta(TELEPORTS_ON_TOP_META_NAME, out teleportsOnTop))
            {
                TeleportsOnTop = teleportsOnTop;
            }

            NodePath destinationNodePath;
            if (TryGetMarkerMeta(DESTINATION_NODE_PATH_META_NAME, out destinationNodePath))
            {
                DestinationNodePath = destinationNodePath;
            }
        }

        /// <summary>
        /// Returns the global-space 3D coordinates of the actual point
        /// where a specified <see cref="GodotObject"> will get sent to when being teleported
        /// by this teleporter.
        /// </summary>
        /// <param name="obj">The object to teleport to the destination</param>
        /// <returns>
        /// The destination point
        /// </returns>
        public Vector3 GetDestinationPoint(GodotObject obj)
        {
            Node3D destination = Destination;
            if (destination != null)
            {
                Vector3 pos = destination.GlobalPosition;

                if (TeleportsOnTop == true)
                {
                    if (destination is VisualInstance3D vDestination)
                    {
                        pos.Y += vDestination.GetAabb().Size.Y * 0.5f;
                    }

                    if (obj != null && obj.HasMeta(OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME))
                    {
                        Aabb boundingBox = obj.GetMeta(OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME).As<Aabb>();
                        pos.Y += boundingBox.Size.Y * 0.5f;
                    }
                }

                return pos;
            }

            return Vector3.Zero;
        }

        /// <summary>
        /// Sends a node to the destination specified in <see cref="Destination"/> 
        /// </summary>
        /// <param name="node">The node to send to the destination</param>
        public void SendToDestination(Node3D node)
        {
            node.GlobalPosition = GetDestinationPoint(node);

            if (ShouldSetRotation)
            {
                node.Rotation = Destination.GlobalRotation;
            }
        }
    }
}
