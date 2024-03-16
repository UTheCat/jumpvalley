using Godot;
using Jumpvalley.Players.Camera;
using Jumpvalley.Raycasting;
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
            Stopped = 0,

            /// <summary>
            /// The character is moving, but not at the user's request.
            /// </summary>
            Moving = 1,

            /// <summary>
            /// The character is walking/running at the user's request.
            /// </summary>
            Running = 2,

            /// <summary>
            /// The character is moving upward, but not at the user's request (e.g. the character is moving upward while <see cref="IsJumping"/> and <see cref="IsClimbing"/> are both false)
            /// </summary>
            Rising = 3,

            /// <summary>
            /// The character is jumping.
            /// A character is jumping only while <see cref="IsJumping"/> is set to true and the character is moving upward.
            /// </summary>
            Jumping = 4,

            /// <summary>
            /// The character is climbing something
            /// </summary>
            Climbing = 5,

            /// <summary>
            /// The character is falling down
            /// </summary>
            Falling = 6
        }

        private static string PROJECT_SETTINGS_PHYSICS_TICKS_PER_SECOND = "physics/common/physics_ticks_per_second";

        private BodyState _currentBodyState = BodyState.Stopped;

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

        // For climbing stuff

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
                Climber climber = CurrentClimber;

                // Remove climbing raycast sweep from the scene tree it's in (if it's in one), just in case.
                climbingRaycastSweep.GetParent()?.RemoveChild(climbingRaycastSweep);

                if (value == null)
                {
                    rotator.Body = null;
                    climber.Hitbox = null;

                    return;
                }

                if (rotator != null)
                {
                    rotator.Body = value;
                }
                
                if (climber != null)
                {
                    CollisionShape3D hitbox = value.GetNode<CollisionShape3D>(CHARACTER_ROOT_COLLIDER_NAME);
                    climber.Hitbox = hitbox;

                    BoxShape3D boxShape = hitbox.Shape as BoxShape3D;

                    if (boxShape != null)
                    {
                        float climberHitboxWidth = climber.HitboxWidth;
                        float climberHitboxDepth = climber.HitboxDepth;

                        // For simplification
                        float xPos = climberHitboxWidth / 2;

                        // Offset by 0.005 meters away from the character hitbox to make sure we don't end up detecting the character hitbox itself
                        float zPos = -(boxShape.Size.Z / 2) - 0.005f;

                        // Remember that the climbing raycast sweep is a child node of the character
                        climbingRaycastSweep.StartPosition = new Vector3(-xPos, 0, zPos);
                        climbingRaycastSweep.EndPosition = new Vector3(xPos, 0, zPos);
                        climbingRaycastSweep.RaycastLength = -climberHitboxDepth * 10;
                        climbingRaycastSweep.UpdateRaycastLayout();
                    }
                }

                // The position of the climbing raycast sweep should be based on the position of the character
                value.AddChild(climbingRaycastSweep);
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

        /// <summary>
        /// Raycast sweep used to grab the normal of an object that the player is climbing on
        /// </summary>
        private RaycastSweep climbingRaycastSweep;

        /// <summary>
        /// Constructs a new instance of BaseMover that can be used to handle character movement
        /// </summary>
        public BaseMover()
        {
            IsRunning = false;
            Rotator = new BodyRotator();

            CurrentClimber = new Climber(null);
            CurrentClimber.OnCanClimbChanged += (object _o, bool canClimb) =>
            {
                IsClimbing = canClimb;
            };

            climbingRaycastSweep = new RaycastSweep(5, Vector3.Zero, Vector3.Zero, -1f);

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
            int physicsTicksPerSecond = Engine.PhysicsTicksPerSecond;

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
                    // Move raycast sweep's raycasts to have a y-position equal to the y-position of the object currently being climbed,
                    // to make sure at least one of the raycasts hit the object being climbed.
                    // We'll also need to change their x and z positions too.
                    Vector3 climbedObjectPos = CurrentClimber.CurrentlyClimbedObject.GlobalPosition;

                    Vector3 climbingRaycastSweepPos = climbingRaycastSweep.GlobalPosition;

                    climbingRaycastSweep.GlobalPosition = new Vector3(climbingRaycastSweepPos.X, climbedObjectPos.Y, climbingRaycastSweepPos.Z);

                    // Determine the 3d object's normal that we're climbing on
                    RaycastSweepResult raycastSweepResult = climbingRaycastSweep.PerformSweep(RaycastSweep.SweepOrder.CenterLeftRight);
                    if (raycastSweepResult != null)
                    {
                        Vector3 climbingNormal = raycastSweepResult.Raycast.GetCollisionNormal();

                        // Get the angles we need to compare normal with move direction,
                        // and do the math as needed according to what was put in the Jumpvalley wiki
                        // for determining whether or not to climb up in the current frame.

                        // This method for calculating angleDiff doesn't work out too well if
                        // one angle is in the 1st or 4th quadrant and the other angle is in the 2nd or 3rd quadrant.
                        /*
                        double ladderCollisionAngle = -Math.Atan(climbingNormal.Z / climbingNormal.X);
                        double moveAngle = Math.Atan(moveVector.Z / moveVector.X);
                        double angleDiff = Math.Abs(moveAngle - ladderCollisionAngle);
                        bool shouldClimbUp = angleDiff <= (Math.PI / 2);
                        Console.WriteLine($"Climbing normal z-coordinate: {climbingNormal.Z}\nClimbing normal x-coordinate: {climbingNormal.X}\nLadder collision angle (after being negated): {ladderCollisionAngle / Math.PI}pi");
                        Console.WriteLine($"Move angle: {moveAngle/Math.PI}pi\nAngle difference: {angleDiff/Math.PI}pi\nShould climb up: {shouldClimbUp.ToString()}");
                        */

                        // Apparently, Godot's Vector3.SignedAngleTo method exists, making this much easier to implement.
                        float angleDiff = climbingNormal.Rotated(Vector3.Up, (float)Math.PI).SignedAngleTo(moveVector, Vector3.Up);
                        //Console.WriteLine($"Angle difference: {angleDiff/Math.PI}pi");
                        bool shouldClimbUp = Math.Abs(angleDiff) <= (Math.PI / 2);

                        if (shouldClimbUp)
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
                    }
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
                    CurrentBodyState = BodyState.Jumping;
                }
                else if (IsClimbing)
                {
                    CurrentBodyState = BodyState.Climbing;
                }
                else if (IsJumping == false && actualVelocity.Y > 0)
                {
                    CurrentBodyState = BodyState.Rising;
                }
                else if (actualVelocity.Y < 0)
                {
                    CurrentBodyState = BodyState.Falling;
                }
                else if ((actualVelocity.X != 0 || actualVelocity.Z != 0) && IsOnFloor())
                {
                    if (RightValue != 0 || ForwardValue != 0)
                    {
                        CurrentBodyState = BodyState.Running;
                    }
                    else
                    {
                        CurrentBodyState = BodyState.Moving;
                    }
                }
                else
                {
                    CurrentBodyState = BodyState.Stopped;
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

            climbingRaycastSweep.Dispose();
            climbingRaycastSweep = null;

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
