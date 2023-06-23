using Godot;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This is the base class that provides a player's character the ability to move in different directions, jump, and climb.
    /// It serves mainly as a controller interface for developers to build on.
    /// <br/>
    /// There's no need to take care of upward and downward movement here since Godot's engine takes care of that.
    /// <br/>
    /// The design of this takes lots of inspiration from Roblox's PlayerModule.
    /// </summary>
    public partial class BaseMover
    {
        /// <summary>
        /// Magnitude in which the character wishes to go forward in the range of [0, 1].
        /// </summary>
        public float ForwardValue = 0;

        /// <summary>
        /// Magnitude in which the character wishes to go backward in the range of [0, 1].
        /// </summary>
        public float BackwardValue = 0;

        /// <summary>
        /// Magnitude in which the character wishes to go left in the range of [0, 1].
        /// </summary>
        public float LeftValue = 0;

        /// <summary>
        /// Magnitude in which the character wishes to go right in the range of [0, 1].
        /// </summary>
        public float RightValue = 0;

        /// <summary>
        /// The initial velocity of the character's jump
        /// </summary>
        public double JumpVelocity = 0;

        /// <summary>
        /// Whether or not the player is currently climbing
        /// </summary>
        public bool IsClimbing = false;

        /// <summary>
        /// Whether or not the player is currently jumping
        /// </summary>
        public bool IsJumping = false;

        /// <summary>
        /// Calculates and returns the move vector for the current forward, backward, left, and right values
        /// as well as whether or not the player is currently climbing anything.
        /// </summary>
        /// <param name="relativeTo">The Vector3 that the forward, backward, left, and right values are relative to</param>
        /// <returns>The calculated move vector</returns>
        public Vector3 GetMoveVector(Vector3 relativeTo)
        {
            return new Vector3(0, 0, 0);
        }
    }
}
