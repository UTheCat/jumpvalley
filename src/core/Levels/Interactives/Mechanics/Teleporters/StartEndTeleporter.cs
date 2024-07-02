using Godot;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics.Teleporters
{
    /// <summary>
    /// Type of teleporter that teleports a node to <see cref="Teleporter.Destination"/>
    /// when the node touches specified parts.
    /// </summary>
    public partial class StartEndTeleporter : Teleporter
    {
        public StartEndTeleporter(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {

        }
    }
}
