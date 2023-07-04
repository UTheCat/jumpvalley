using Godot;
using Jumpvalley.Players.Camera;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This is the base class that provides a player's character the ability to move in different directions, jump, and climb.
    /// It serves mainly as a controller interface for developers to build on.
    /// <br/>
    /// The design of this takes lots of inspiration from Roblox's PlayerModule.
    /// </summary>
    public partial class BaseMover : Node, System.IDisposable
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

        private float _speed;

        /// <summary>
        /// How fast the character can move in meters per second
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;

                if (Rotator != null)
                {
                    Rotator.Speed = value;
                }
            }
        }

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

        private bool _isRotationLocked = false;

        /// <summary>
        /// Whether or not the character's yaw is locked to some specified yaw angle
        /// </summary>
        public bool IsRotationLocked
        {
            get => _isRotationLocked;
            set
            {
                _isRotationLocked = value;

                if (Rotator != null)
                {
                    Rotator.TurnsInstantly = value;
                }
            }
        }

        private bool _isRunning;

        /// <summary>
        /// Whether or not the <see cref="BaseMover"/> is actively updating every physics and process frame
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;

                SetPhysicsProcess(value);
                SetProcess(value);
            }
        }

        private CharacterBody3D _body = null;

        /// <summary>
        /// The <see cref="CharacterBody3D"/> that this BaseMover is binded to.
        /// </summary>
        public CharacterBody3D Body
        {
            get => _body;
            set
            {
                _body = value;

                BodyRotator rotator = Rotator;
                if (rotator != null)
                {
                    rotator.Body = value;
                }
            }
        }

        /// <summary>
        /// The <see cref="BaseCamera"/> to bind <see cref="CameraYaw"/> to
        /// </summary>
        public BaseCamera Camera = null;

        private BodyRotator _rotator = null;

        /// <summary>
        /// The <see cref="BodyRotator"/> that will be rotating <see cref="Body"/>
        /// </summary>
        public BodyRotator Rotator
        {
            get => _rotator;
            private set
            {
                _rotator = value;

                if (value != null)
                {
                    value.TurnsInstantly = IsRotationLocked;
                    value.Speed = Speed;
                    value.Body = Body;
                }
            }
        }

        public BaseMover()
        {
            IsRunning = false;
            Rotator = new BodyRotator();
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
            // there are slight variances in the actual time passed between each step.
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

        private float GetYaw()
        {
            float yaw;
            if (Camera == null)
            {
                yaw = CameraYaw;
            }
            else
            {
                yaw = Camera.Yaw;
            }

            return yaw;
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
                body.Velocity = GetVelocity((float)delta, GetYaw());
                body.MoveAndSlide();
            }
        }

        /// <summary>
        /// Callback to associate with the normal process step in the current scene tree
        /// </summary>
        /// <param name="delta"></param>
        public void HandleProcessStep(double delta)
        {
            BodyRotator rotator = Rotator;
            
            // Only rotate if the rotation is locked (such as when shift lock is enabled) or when the character is moving
            if (rotator != null)
            {
                if (IsRotationLocked)
                {
                    // Set the angle to the camera's yaw
                    rotator.Yaw = GetYaw();
                }
                else if (ForwardValue != 0 || RightValue != 0)
                {
                    // Thanks to Godot 4.0 .NET thirdperson controller by vaporvee for helping me figure this one out
                    // The extra radians are added on top of the original camera yaw, since
                    // the direction of the character should be determined by the yaw corresponding to the move vector
                    // relative to the camera yaw.
                    rotator.Yaw = GetYaw() + (float)Math.Atan2(-RightValue, -ForwardValue);
                }

                rotator.Update(delta);
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

        public override void _Process(double delta)
        {
            HandleProcessStep(delta);
            base._Process(delta);
        }
    }
}
