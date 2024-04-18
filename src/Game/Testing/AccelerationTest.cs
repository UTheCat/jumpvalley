using Godot;
using Jumpvalley.Players.Movement;

namespace Jumpvalley.Game.Testing
{
    /// <summary>
    /// Tests how BaseMover acceleration calculations affect the velocity of the character it's assigned to.
    /// </summary>
    public partial class AccelerationTest : Node
    {
        private BaseMover mover;
        private Label label;

        public AccelerationTest(BaseMover moverToTest)
        {
            mover = moverToTest;

            label = new Label();
        }

        public override void _Process(double delta)
        {
            label.Text = $"";

            base._Process(delta);
        }
    }
}
