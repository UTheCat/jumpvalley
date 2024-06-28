using Godot;
using Jumpvalley.Levels.Mechanics;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Teleporters
{
    /// <summary>
    /// Interactive that sends a Node3D to a defined destination.
    /// </summary>
    public partial class Teleporter : InteractiveNode
    {
        /// <summary>
        /// Whether or not the Node3D being teleported should have
        /// its rotation set to the rotation of <see cref="Destination"/> 
        /// </summary>
        public bool ShouldSetRotation;

        /// <summary>
        /// Whether or not nodes being sent to <see cref="Destination"/>
        /// should be teleported on top of <see cref="Destination"/>.
        /// <br/><br/>
        /// If <see cref="Destination"/> has height, <see cref="Destination"/>
        /// will have to be a <see cref="VisualInstance3D"/> in order to work properly. 
        /// </summary>
        public bool TeleportsOnTop;

        /// <summary>
        /// Where <see cref="Node3D"/>s being teleported by this teleporter
        /// should be sent.
        /// </summary>
        public Node3D Destination;

        public Teleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker) { }

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

                if (TeleportsOnTop == true && destination is VisualInstance3D vDestination)
                {
                    pos.Y += vDestination.GetAabb().Size.Y * 0.5f;
                }

                if (obj != null && obj.HasMeta(OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME))
                {
                    Aabb boundingBox = obj.GetMeta(OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME).As<Aabb>();
                    pos.Y += boundingBox.Size.Y * 0.5f;
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

        }
    }
}
