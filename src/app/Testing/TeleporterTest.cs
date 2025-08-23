using System;
using Godot;
using UTheCat.Jumpvalley.Core.Levels.Interactives;
using UTheCat.Jumpvalley.Core.Levels.Interactives.Mechanics;
using UTheCat.Jumpvalley.Core.Levels.Interactives.Mechanics.Teleporters;
using UTheCat.Jumpvalley.Core.Timing;

namespace UTheCat.Jumpvalley.App.Testing
{
    /// <summary>
    /// Class that tests the functionality of the <see cref="Teleporter"/> class 
    /// </summary>
    public partial class TeleporterTest : IDisposable
    {
        private Node3D nodeToTeleport;
        private Teleporter teleporter;

        public TeleporterTest(Node3D nodeToTeleport, Node3D destination)
        {
            this.nodeToTeleport = nodeToTeleport;

            nodeToTeleport.SetMeta(
                OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME,
                OverallBoundingBoxObject.GetOverallBoundingBox(nodeToTeleport)
            );

            teleporter = new Teleporter(
                new OffsetStopwatch(new TimeSpan()),
                destination.GetNode(InteractiveNode.NODE_MARKER_NAME_PREFIX)
            );
        }

        public void Teleport()
        {
            teleporter.SendToDestination(nodeToTeleport);
        }

        public void Dispose()
        {
            teleporter.Dispose();
        }
    }
}
