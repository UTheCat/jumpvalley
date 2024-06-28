using System;
using Godot;
using Jumpvalley.Levels.Interactives;
using Jumpvalley.Levels.Interactives.Mechanics.Teleporters;
using Jumpvalley.Timing;

namespace JumpvalleyGame.Testing
{
    /// <summary>
    /// Class that tests the functionality of the <see cref="Teleporter"/> class 
    /// </summary>
    public partial class TeleporterTest : IDisposable
    {
        private Node3D nodeToTeleport;
        private Node3D destination;
        private Teleporter teleporter;

        public TeleporterTest(Node3D nodeToTeleport, Node3D destination)
        {
            this.nodeToTeleport = nodeToTeleport;
            this.destination = destination;

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
