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
            AddChild(label);
        }

        public override void _Process(double delta)
        {
            CharacterBody3D body = mover.Body;
            if (body == null)
            {
                label.Text = "BaseMover we're currently testing has no CharacterBody3D assigned to it";
            }
            else
            {
                label.Text = $"Testing acceleration handled by {mover.Name}\nAcceleration: {mover.Acceleration} m/s^2\nVelocity: {body.Velocity}\nReal velocity: {body.GetRealVelocity()}";
            }

            base._Process(delta);
        }
    }
}
