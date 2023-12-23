using Godot;
using Jumpvalley.Players.Camera;
using System;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This is the base class that provides a player's character the ability to move in different directions, jump, and climb.
    /// It serves mainly as a controller interface for developers to build on.
    /// <br/>
    /// The design of this takes lots of inspiration from Roblox's PlayerModule.
    /// </summary>
    public partial class BaseMover : Node, IDisposable
    {
        /// <summary>
        /// The current movement state of the character associated with the mover
        /// </summary>
        public enum BodyState
        {
            /// <summary>
            /// The character is not moving
            /// </summary>
            STOPPED = 0,

            /// <summary>
            /// The character is moving, but not at the user's request.
            /// </summary>
            MOVING = 1,

            /// <summary>
            /// The character is walking/running at the user's request.
            /// </summary>
            RUNNING = 2,

            /// <summary>
            /// The character is moving upward, but not at the user's request (e.g. the character is moving upward while <see cref="IsJumping"/> and <see cref="IsClimbing"/> are both false)
            /// </summary>
            RISING = 3,

            /// <summary>
            /// The character is jumping.
            /// A character is jumping only while <see cref="IsJumping"/> is set to true and the character is moving upward.
            /// </summary>
            JUMPING = 4,

            /// <summary>
            /// The character is climbing something
            /// </summary>
            CLIMBING = 5,

            /// <summary>
            /// The character is falling down
            /// </summary>
            FALLING = 6
        }

        private static string PROJECT_SETTINGS_PHYSICS_TICKS_PER_SECOND = "physics/common/physics_ticks_per_second";

        private BodyState _currentBodyState = BodyState.STOPPED;

        /// <summary>
        /// The name of the <see cref="CollisionShape3D"/> that should be primarily in charge of handling a character's collision.
        /// </summary>
        public static readonly string CHARACTER_ROOT_COLLIDER_NAME = "RootCollider";

        /// <summary>
        /// The current movement state of the character that's being moved by this <see cref="BaseMover"/>
        /// </summary>
        public BodyState CurrentBodyState
        {
            get => _currentBodyState;
            set
            {
                if (_currentBodyState == value) return;

                BodyState oldState = _currentBodyState;
                _currentBodyState = value;
                RaiseBodyStateChangedEvent(oldState, value);
            }
        }

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

                Climber climber = CurrentClimber;
                if (climber != null)
                {
                    climber.Hitbox = value.GetNode<CollisionShape3D>(CHARACTER_ROOT_COLLIDER_NAME);
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

        /// <summary>
        /// The <see cref="Climber"/> that's currently in charge of determining whether or not the character can climb at the current physics frame.
        /// </summary>
        public Climber CurrentClimber { get; private set; }

        public BaseMover()
        {
            IsRunning = false;
            Rotator = new BodyRotator();
            CurrentClimber = new Climber(null);

            CurrentClimber.OnCanClimbChanged += (object _o, bool canClimb) =>
            {
                IsClimbing = canClimb;
            };
            AddChild(CurrentClimber);
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

            bool isOnFloor = IsOnFloor();
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
                if (isOnFloor || IsClimbing)
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
                float climbVelocity = 0;

                // Remember that "wanting to move forward" while climbing means we want to go up,
                // and "wanting to move backward" while climbing means we want to go down.

                bool shouldApplyClimbVelocity = true;

                if (ForwardValue == 0 && RightValue == 0)
                {
                    climbVelocity = 0;
                }
                else
                {
                    // The collision point of the Climber's raycast such that the raycast hit a climbable object
                    Vector3 collisionPoint = CurrentClimber.RaycastCollisionPoint;

                    Vector3 characterPos = Body.GlobalPosition;
                    float moveVectorX = moveVector.X;
                    float moveVectorZ = moveVector.Z;

                    // Discovered a bug while testing: climbing up seems to be a little buggy.
                    // The bug occurs in cases where the player does not hit a climbable object at a perpendicular angle (or somewhere really close).
                    // In this case, one of the conditions that compare a collision point coordinate with the character's position coordinate could always be true.
                    //
                    // For example, assume your character's yaw angle is -0.05 radians when your character gets into climbing position.
                    // In this case, the x-coordinate of the collision point will always be greater than the character's position x-coordinate until the collision point moves.
                    // Because of this, if you tried to move right, you would climb up.
                    //
                    // This bug is somewhat miniscule however.
                    // This is due to the fact that in Juke's Towers of Hell and games alike,
                    // you can't climb up or down by trying to move left or right when your camera is basically facing the climbable object.
                    if (
                        (collisionPoint.X <= characterPos.X && moveVectorX <= 0)
                        || (collisionPoint.X >= characterPos.X && moveVectorX >= 0)
                        || (collisionPoint.Z <= characterPos.Z && moveVectorZ <= 0)
                        || (collisionPoint.Z >= characterPos.Z && moveVectorZ >= 0)
                        )
                    {
                        climbVelocity = Speed * timingAdjustment;
                    }
                    else
                    {
                        if (isOnFloor)
                        {
                            // If we're already on the floor, move like we're walking on the floor.
                            velocity.Y = 0;
                            climbVelocity = 0;
                            shouldApplyClimbVelocity = false;
                        }
                        else
                        {
                            climbVelocity = -Speed * timingAdjustment;
                        }
                    }

                    /*
                    // For some reason, the reverse of the above is true (this is likely a bug), so the sign is switched from greater than to less than for now.
                    if (ForwardValue < 0)
                    {
                        climbVelocity = Speed * timingAdjustment;
                    }
                    else if (ForwardValue == 0)
                    {
                        climbVelocity = 0;
                    }
                    else
                    {
                        if (isOnFloor)
                        {
                            // If we're already on the floor, move like we're walking on the floor.
                            velocity.Y = 0;
                            climbVelocity = 0;
                            shouldApplyClimbVelocity = false;
                        }
                        else
                        {
                            climbVelocity = -Speed * timingAdjustment;
                        }
                    }
                    */
                }

                if (shouldApplyClimbVelocity)
                {
                    velocity.X = 0;
                    velocity.Y = climbVelocity;
                    velocity.Z = 0;
                }
            }
            else if (!isOnFloor)
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

                // update CurrentBodyState according to the character's actual velocity and the values of IsJumping and IsClimbing
                Vector3 actualVelocity = body.Velocity;

                if (IsJumping && actualVelocity.Y > 0)
                {
                    // Jumping is placed first in line so that jumping can affect climbing
                    CurrentBodyState = BodyState.JUMPING;
                }
                else if (IsClimbing)
                {
                    CurrentBodyState = BodyState.CLIMBING;
                }
                else if (IsJumping == false && actualVelocity.Y > 0)
                {
                    CurrentBodyState = BodyState.RISING;
                }
                else if (actualVelocity.Y < 0)
                {
                    CurrentBodyState = BodyState.FALLING;
                }
                else if ((actualVelocity.X != 0 || actualVelocity.Z != 0) && IsOnFloor())
                {
                    if (RightValue != 0 || ForwardValue != 0)
                    {
                        CurrentBodyState = BodyState.RUNNING;
                    }
                    else
                    {
                        CurrentBodyState = BodyState.MOVING;
                    }
                }
                else
                {
                    CurrentBodyState = BodyState.STOPPED;
                }
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
                    rotator.Update(delta);
                }
                else if (ForwardValue != 0 || RightValue != 0)
                {
                    // Thanks to Godot 4.0 .NET thirdperson controller by vaporvee for helping me figure this one out
                    // The extra radians are added on top of the original camera yaw, since
                    // the direction of the character should be determined by the yaw corresponding to the move vector
                    // relative to the camera yaw.
                    rotator.Yaw = GetYaw() + (float)Math.Atan2(-RightValue, -ForwardValue);
                    rotator.GradualTurnEnabled = !IsClimbing;
                    rotator.Update(delta);
                }
            }
        }

        /// <summary>
        /// Disposes of this <see cref="BaseMover"/>
        /// </summary>
        public new void Dispose()
        {
            SetPhysicsProcess(false);
            QueueFree();

            // Currently, the Climber being used in this class is created during BaseMover's instantiation and from nowhere else
            CurrentClimber.Dispose();
            CurrentClimber = null;

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

        /// <summary>
        /// Event that's raised when the character being moved by this <see cref="BaseMover"/> changes.
        /// </summary>
        public event EventHandler<BodyStateChangedArgs> BodyStateChanged;

        protected void RaiseBodyStateChangedEvent(BodyState oldState, BodyState newState)
        {
            BodyStateChanged?.Invoke(this, new BodyStateChangedArgs(oldState, newState));
        }
    }
}
