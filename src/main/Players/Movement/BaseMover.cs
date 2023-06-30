using Godot;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This is the base class that provides a player's character the ability to move in different directions, jump, and climb.
    /// It serves mainly as a controller interface for developers to build on.
    /// <br/>
    /// The design of this takes lots of inspiration from Roblox's PlayerModule.
    /// </summary>
    public partial class BaseMover: Node, System.IDisposable
    {
        private static string PROJECT_SETTINGS_PHYSICS_TICKS_PER_SECOND = "physics/common/physics_ticks_per_second";

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
        /// The current yaw angle of the camera that's currently associated with the character
        /// </summary>
        public float CameraYaw = 0;

        /// <summary>
        /// Whether or not the player is currently climbing
        /// </summary>
        public bool IsClimbing = false;

        /// <summary>
        /// Whether or not the player is currently jumping
        /// </summary>
        public bool IsJumping = false;

        /// <summary>
        /// Whether or not the character's yaw is locked to some specified yaw angle
        /// </summary>
        public bool IsRotationLocked = false;

        /// <summary>
        /// The <see cref="CharacterBody3D"/> that this BaseMover is binded to.
        /// </summary>
        public CharacterBody3D Body = null;

        public BaseMover()
        {
            SetPhysicsProcess(false);
        }

        /// <summary>
        /// Returns whether or not the associated <see cref="CharacterBody3D"/> is on the floor.
        /// </summary>
        /// <returns></returns>
        public bool IsOnFloor()
        {
            if (Body != null)
            {
                return Body.IsOnFloor();
            }

            return false;
        }

        /// <summary>
        /// Calculates and returns the move vector that the player wants to move the character in, regardless of whether or not they're currently jumping or climbing.
        /// <br/>
        /// The calculated move vector can be rotated to a specified yaw angle. This is useful when you want to make the character move in the direction that the camera is facing.
        /// </summary>
        /// <param name="yaw">The yaw angle that the forward and right values are relative to.</param>
        /// <returns>The calculated move vector</returns>
        public Vector3 GetMoveVector(float yaw)
        {
            // The Rotate() call rotates the MoveVector to the specified yaw angle.
            return new Vector3(RightValue, 0, ForwardValue).Rotated(Vector3.Up, yaw).Normalized();
        }

        /// <summary>
        /// Gets the character's velocity for some sort of physics frame.
        /// </summary>
        /// <param name="delta">The time it took to complete the physics frame in seconds</param>
        /// <param name="yaw">The yaw angle to make the move vector relative to.</param>
        /// <returns></returns>
        public Vector3 GetVelocity(float delta, float yaw)
        {
            float physicsTicksPerSecond = (float)ProjectSettings.GetSetting(PROJECT_SETTINGS_PHYSICS_TICKS_PER_SECOND);

            // This is needed because while physics steps should occur at constant time intervals,
            // there are slight variances in between each step.
            float timingAdjustment = delta * physicsTicksPerSecond;

            Vector3 moveVector = GetMoveVector(yaw);
            Vector3 velocity;

            if (Body == null)
            {
                velocity = moveVector;
            }
            else
            {
                velocity = Body.Velocity;
            }

            velocity.X = moveVector.X * Speed * timingAdjustment;
            velocity.Z = moveVector.Z * Speed * timingAdjustment;

            if (IsJumping)
            {
                // Basically how jumping works in Flood Escape 2 by Crazyblox Games
                if (IsOnFloor() || IsClimbing)
                {
                    velocity.Y = JumpVelocity;
                }
                else
                {
                    velocity.Y -= Gravity * delta;
                }
            }
            else if (IsClimbing)
            {
                // change these later, as this climbing logic isn't accurate
                velocity.X = 0;
                velocity.Y = Speed;
                velocity.Z = 0;
            }
            else if (!IsOnFloor())
            {
                velocity.Y -= Gravity * delta;
            }

            return velocity;
        }

        /// <summary>
        /// Callback to associate with the physics process step in the current scene tree
        /// </summary>
        /// <param name="delta">The time it took and should take to complete the physics frame in seconds</param>
        public void HandlePhysicsStep(double delta)
        {
            CharacterBody3D body = Body;
            if (body != null)
            {
                body.Velocity = GetVelocity((float)delta, CameraYaw);
                body.MoveAndSlide();
            }
        }

        /// <summary>
        /// Disposes of this <see cref="BaseMover"/>
        /// </summary>
        public new void Dispose()
        {
            SetPhysicsProcess(false);
            QueueFree();
            base.Dispose();
        }

        public override void _PhysicsProcess(double delta)
        {
            HandlePhysicsStep(delta);
            base._PhysicsProcess(delta);
        }
    }
}
