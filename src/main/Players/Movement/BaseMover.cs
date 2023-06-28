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
        /// Scalar in which the character wishes to go forward in the range of [-1, 1].
        /// <br/>
        /// A value less than 0 indicates that the character wants to go backwards while a value greater than 0 indicates that the character wants to go forwards.
        /// </summary>
        public float ForwardValue = 0;

        /// <summary>
        /// Magnitude in which the character wishes to go backward in the range of [0, 1].
        /// </summary>
        //public float BackwardValue = 0;

        /// <summary>
        /// Magnitude in which the character wishes to go left in the range of [0, 1].
        /// </summary>
        //public float LeftValue = 0;

        /// <summary>
        /// Scalar in which the character wishes to go right in the range of [-1, 1].
        /// <br/>
        /// A value less than 0 indicates that the character wants to go left while a value greater than 0 indicates that the character wants to go right.
        /// </summary>
        public float RightValue = 0;

        /// <summary>
        /// The gravity that the character experiences in meters per second squared.
        /// </summary>
        public float Gravity = 9.8f;

        /// <summary>
        /// The initial velocity of the character's jump
        /// </summary>
        public float JumpVelocity = 5f;

        /// <summary>
        /// Whether or not the player is currently climbing
        /// </summary>
        public bool IsClimbing = false;

        /// <summary>
        /// Whether or not the player is currently jumping
        /// </summary>
        public bool IsJumping = false;

        /// <summary>
        /// Calculates and returns the move vector that the player wants to move the character in, regardless of whether or not they're currently jumping or climbing.
        /// 
        /// </summary>
        /// <param name="relativeTo">The Vector3 that the forward, backward, left, and right values are relative to</param>
        /// <returns>The calculated move vector</returns>
        public Vector3 GetMoveVector(Vector3 relativeTo)
        {
            return new Vector3(0, 0, 0);
        }

        
    }
}
