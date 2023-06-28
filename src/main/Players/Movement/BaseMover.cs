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
    public partial class BaseMover: CharacterBody3D
    {
        /// <summary>
        /// Scalar in which the character wishes to go forward in the range of [-1, 1].
        /// <br/>
        /// A value less than 0 indicates that the character wants to go backwards while a value greater than 0 indicates that the character wants to go forwards.
        /// </summary>
        public float ForwardValue = 0;

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
        /// How fast the character can move in meters per second
        /// </summary>
        public float Speed = 5f;

        /// <summary>
        /// Whether or not the player is currently climbing
        /// </summary>
        public bool IsClimbing = false;

        /// <summary>
        /// Whether or not the player is currently jumping
        /// </summary>
        public bool IsJumping = false;

        /// <summary>
        /// Whether or not the character's y-axis rotation is locked to some specified y-axis angle
        /// </summary>
        public bool IsRotationLocked = false;

        /// <summary>
        /// Calculates and returns the move vector that the player wants to move the character in, regardless of whether or not they're currently jumping or climbing.
        /// <br/>
        /// The calculated move vector can be rotated to a specified y-axis angle. This is useful when you want to make the character move in the direction that the camera is facing.
        /// </summary>
        /// <param name="yAngle">The Y-axis angle that the forward and right values are relative to.</param>
        /// <returns>The calculated move vector</returns>
        public Vector3 GetMoveVector(float yAngle)
        {
            // The Rotate() call rotates the MoveVector to the specified y-axis angle.
            return new Vector3(RightValue, 0, ForwardValue).Rotated(Vector3.Up, yAngle).Normalized();
        }

        /// <summary>
        /// Gets the character's velocity for some sort of physics frame.
        /// </summary>
        /// <param name="delta">The time it took to complete the physics frame in seconds</param>
        /// <param name="yAngle">The Y-axis angle to make the move vector relative to.</param>
        /// <returns></returns>
        public Vector3 GetVelocity(double delta, float yAngle)
        {
            Vector3 moveVector = GetMoveVector(yAngle);

            return Vector3.Zero;
        }
    }
}
