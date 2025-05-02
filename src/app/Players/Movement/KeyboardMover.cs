using Godot;

using UTheCat.Jumpvalley.Core.Players.Movement;

namespace UTheCat.Jumpvalley.App.Players.Movement
{
    /// <summary>
    /// Subclass of <see cref="BaseMover"/> that handles character movement via keyboard input.
    /// </summary>
    public partial class KeyboardMover : BaseMover
    {
        private readonly string CHARACTER_MOVE_LEFT = "character_move_left";
        private readonly string CHARACTER_MOVE_RIGHT = "character_move_right";
        private readonly string CHARACTER_MOVE_FORWARD = "character_move_forward";
        private readonly string CHARACTER_MOVE_BACKWARD = "character_move_backward";
        private readonly string CHARACTER_JUMP = "character_jump";

        public KeyboardMover() : base() { }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed(CHARACTER_JUMP))
            {
                IsJumping = true;
            }
            else if (Input.IsActionJustReleased(CHARACTER_JUMP))
            {
                IsJumping = false;
            }

            // Get move direction from keyboard input
            Vector2 direction = Input.GetVector(CHARACTER_MOVE_LEFT, CHARACTER_MOVE_RIGHT, CHARACTER_MOVE_FORWARD, CHARACTER_MOVE_BACKWARD);
            RightValue = direction.X;
            ForwardValue = direction.Y;
        }
    }
}
