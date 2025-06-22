using Godot;
using UTheCat.Jumpvalley.Core.Players.Camera;
using System;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.Players.Movement
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

        /// <summary>
        /// The name of the <see cref="CollisionShape3D"/> that should be primarily in charge of handling a character's collision.
        /// </summary>
        public static readonly string CHARACTER_ROOT_COLLIDER_NAME = "RootCollider";

        /// <summary>
        /// "Length/magnitude" between requested character move velocity and real velocity after acceleration changes.
        /// This is to prevent floating point errors from causing gradual velocity changes to "overshoot" past the goal velocity.
        /// Snapping will occur if the length/magnitude is at or below this value.
        /// </summary>
        private static readonly float VELOCITY_DIFF_SNAP_THRESHOLD = 0.1f;

        /// <summary>
        /// For preventing cases where being too close to a climbable object
        /// will cause the climbing shape-cast to not be able to hit the outer surface of the climbable object.
        /// </summary>
        private static readonly float CLIMBING_SHAPE_CAST_Z_OFFSET = 0.005f;

        /// <summary>
        /// The number of consecutive frames that a rigid body has to remain untouched in order for its RigidBodyPusher to be removed from <see cref="rigidBodyPushers"/> 
        /// </summary>
        private static readonly int RIGID_BODY_MAX_CONSECUTIVE_FRAMES_UNTOUCHED = 0;

        private BodyState _currentBodyState = BodyState.Stopped;

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
        /// The initial Y-velocity of the character's jump
        /// </summary>
        public float JumpVelocity = 5f;

        /// <summary>
        /// The acceleration that the character's XZ velocity increases at
        /// while the character is trying to move on the ground.
        /// <br/>
        /// This doesn't affect upward and downward movement,
        /// and therefore, this only affects X and Z movement.
        /// </summary>
        public float Acceleration = 16f;

        /// <summary>
        /// The acceleration that the character's XZ velocity increases at while
        /// in the air.
        /// </summary>
        public float AirAcceleration = 8f;

        /// <summary>
        /// The deceleration that the character's XZ velocity decreases at when
        /// on the ground and one of these other conditions is true:
        /// <list type="bullet">
        /// <item>The character is trying to stop</item>
        /// <item>The character has exceeded max speed</item>
        /// </list>
        /// </summary>
        public float Deceleration = 16f;

        /// <summary>
        /// The deceleration that the character's XZ velocity decreases at when
        /// in the air and one of these other conditions is true:
        /// <list type="bullet">
        /// <item>The character is trying to stop</item>
        /// <item>The character has exceeded max speed</item>
        /// </list>
        /// </summary>
        public float AirDeceleration = 8f;

        private float _speed;

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

        private bool _isFastTurnEnabled = false;

        /// <summary>
        /// If the value of this property is true, the character's yaw angle will instantly be set
        /// to a specified "destination" yaw (for example, the current yaw of a <see cref="BaseCamera"/>). Otherwise, while the character is moving,
        /// the character's yaw will gradually approach the destination yaw until the character's
        /// yaw and the destination yaw match.
        /// </summary>
        public bool IsFastTurnEnabled
        {
            get => _isFastTurnEnabled;
            set
            {
                bool valueChanged = value != _isFastTurnEnabled;

                _isFastTurnEnabled = value;

                if (Rotator != null)
                {
                    Rotator.TurnsInstantly = value;
                }

                if (valueChanged)
                {
                    RaiseOnFastTurnToggled(value);
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
                //SetProcess(value);
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
                CharacterBody3D oldBody = _body;
                _body = value;

                BodyRotator rotator = Rotator;
                Climber climber = CurrentClimber;

                // Make sure climbing shape-cast isn't in a scene tree before continuing
                climbingShapeCast.GetParent()?.RemoveChild(climbingShapeCast);

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
                        //float climberHitboxWidth = climber.HitboxWidth;
                        //float climberHitboxDepth = climber.HitboxDepth;

                        // For simplification
                        //float xPos = climberHitboxWidth / 2;

                        float zPos = -(boxShape.Size.Z / 2) + CLIMBING_SHAPE_CAST_Z_OFFSET;

                        BoxShape3D shapeCastBox = climbingShapeCast.Shape as BoxShape3D;
                        if (shapeCastBox != null)
                        {
                            // Height of the climbing shape-cast should be half the height of the character.
                            // 1 additional meter is added to the shape-cast's height to prevent the character from
                            // getting stuck while climbing when at the very top or bottom of a ladder
                            Vector3 size = shapeCastBox.Size;
                            size.Y = (boxShape.Size.Y * 0.5f) + 1f;
                            shapeCastBox.Size = size;

                            climbingShapeCast.Position = new Vector3(0, -boxShape.Size.Y * 0.25f, zPos);

                            value.AddChild(climbingShapeCast);
                        }
                    }
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
                    value.TurnsInstantly = IsFastTurnEnabled;
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
        /// The most recent character velocity.
        /// In BaseMover's PhysicsProcess updater, this is read by <see cref="GetMoveVelocity"/>
        /// to determine what the velocity in the previous physics frame was before
        /// this value gets updated again.
        /// </summary>
        public Vector3 LastVelocity { get; private set; }

        /// <summary>
        /// Shape-cast used to grab the normal of an object that the player is climbing on
        /// </summary>
        private ShapeCast3D climbingShapeCast;

        /// <summary>
        /// The forces to apply when <see cref="HandlePhysicsStep"/> is called.
        /// The order in which the forces in this list are applied is from the end of the list to the beginning of the list.
        /// <br/><br/>
        /// Forces are in newtons.
        /// <br/><br/>
        /// To be implemented.
        /// </summary>
        //public List<Vector3> ForceQueue = new List<Vector3>();

        /// <summary>
        /// The mass of the character in kilograms.
        /// </summary>
        public float Mass = 60f;

        /// <summary>
        /// The "magnitude" in which the character pushes movable rigid bodies.
        /// <br/><br/>
        /// This is basically the same as the physical strength one uses to push an object.
        /// </summary>
        public float ForceMultiplier = 5f;

        //private ConsoleLogger logger;

        private Vector2 lastXZVelocity = Vector2.Zero;

        private float lastVerticalVelocity = 0f;

        private Dictionary<RigidBody3D, RigidBodyPusher> rigidBodyPushers = [];

        /// <summary>
        /// Constructs a new instance of BaseMover that can be used to handle character movement
        /// </summary>
        public BaseMover()
        {
            IsRunning = false;
            SetProcess(false);

            Rotator = new BodyRotator();

            CurrentClimber = new Climber(null);
            CurrentClimber.OnCanClimbChanged += (object _o, bool canClimb) =>
            {
                IsClimbing = canClimb;
            };

            LastVelocity = Vector3.Zero;

            climbingShapeCast = new ShapeCast3D();

            // For performance reasons
            climbingShapeCast.Enabled = false;

            float hitboxDepth = CurrentClimber.HitboxDepth;
            climbingShapeCast.TargetPosition = new Vector3(0f, 0f, -hitboxDepth - CLIMBING_SHAPE_CAST_Z_OFFSET);

            BoxShape3D shapeCastBox = new BoxShape3D();
            shapeCastBox.Size = new Vector3(CurrentClimber.HitboxWidth, 0f, hitboxDepth + CLIMBING_SHAPE_CAST_Z_OFFSET);
            climbingShapeCast.Shape = shapeCastBox;

            AddChild(CurrentClimber);

            //logger = new ConsoleLogger(nameof(BaseMover));
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

        private Vector3 GetUpDirection()
        {
            return Body == null ? Vector3.Up : Body.UpDirection;
        }

        /// <summary>
        /// Calculates and returns the move vector that the player wants to move the character in, regardless of whether or not they're currently jumping or climbing.
        /// <br/>
        /// The calculated move vector can be rotated to a specified yaw angle. This is useful when you want to make the character move in the direction that the camera is facing.
        /// </summary>
        /// <param name="yaw">The yaw angle that the forward and right values are relative to.</param>
        /// <returns>The calculated move vector</returns>
        public Vector3 GetMoveDirection(float yaw)
        {
            // The Rotate() call rotates the MoveVector to the specified yaw angle.
            return new Vector3(RightValue, 0, ForwardValue).Rotated(Vector3.Up, yaw).Normalized();
        }

        /// <summary>
        /// Returns whether or not the character is trying to move.
        /// <br/><br/>
        /// The character is considered to want to move when
        /// either <see cref="ForwardValue"/> or <see cref="RightValue"/>
        /// isn't zero.
        /// </summary>
        /// <returns>
        /// Whether or not the character is trying to move.
        /// </returns>
        public bool IsTryingToMove()
        {
            return ForwardValue != 0 || RightValue != 0;
        }

        /// <summary>
        /// Updates XZ velocity (velocity along the X and Z axes) that the character is trying to move at for the current physics frame.
        /// Intended to be called every physics frame.
        /// <br/><br/>
        /// This function takes physics framerate, acceleration, and target move direction into account.
        /// </summary>
        /// <param name="physicsFrameDelta">The time it took to complete the most recent physics frame in seconds.</param>
        /// <param name="acceleration">Acceleration in meters per second squared.
        /// This controls how fast we approach the XZ velocity.
        /// This value should also be positive, as negative values will cause the XZ-velocity to advance away from the target XZ-velocity.
        /// </param>
        /// <param name="yaw">The intended move 'yaw'. For example, this yaw could be the yaw of the player's camera.</param> 
        /// <returns>
        /// The updated XZ velocity. The Y-value in the returned Vector2 is the velocity in the Z axis.
        /// Each component in the returned Vector2 is in meters per second.
        /// </returns>
        private Vector2 UpdateXZVelocity(float physicsFrameDelta, float acceleration, float yaw)
        {
            Vector3 moveDirection = GetMoveDirection(yaw);

            // For the goalXZVelocity Vector2, Y-coordinate is the goal velocity's Z coordinate.
            Vector2 newXZVelocity = ApproachXZVelocity(
                lastXZVelocity,
                new Vector2(moveDirection.X, moveDirection.Z),
                acceleration,
                physicsFrameDelta
            );
            lastXZVelocity = newXZVelocity;

            return newXZVelocity;
        }

        /// <summary>
        /// Updates vertical velocity of the character based on physics framerate and vertical acceleration.
        /// Intended to be called every physics frame.
        /// </summary>
        /// <param name="physicsFrameDelta">The time it took to complete the most recent physics frame in seconds.</param>
        /// <param name="verticalAcceleration">
        /// Change in vertical velocity in meters per second squared.
        /// Should be negative when trying to simulate gravity.
        /// </param>
        /// <returns>
        /// The updated vertical velocity in meters per second.
        /// </returns>
        private float UpdateVerticalVelocity(float physicsFrameDelta, float verticalAcceleration)
        {
            float newVerticalVelocity = lastVerticalVelocity + verticalAcceleration * physicsFrameDelta;

            lastVerticalVelocity = newVerticalVelocity;

            return newVerticalVelocity;
        }

        /// <summary>
        /// Gets the velocity that the character wants to move at for the current physics frame
        /// </summary>
        /// <param name="delta">The time it took to complete the physics frame in seconds</param>
        /// <param name="yaw">The yaw angle to make the move vector relative to.</param>
        /// <returns></returns>
        public Vector3 GetMoveVelocity(float delta, float yaw)
        {
            //int physicsTicksPerSecond = Engine.PhysicsTicksPerSecond;

            // This is needed because while physics steps should occur at constant time intervals,
            // there are slight variances in the actual time passed between each step.
            //float timingAdjustment = delta * physicsTicksPerSecond;

            bool isOnFloor = IsOnFloor();

            Vector3 moveVector = GetMoveDirection(yaw);
            Vector3 velocity;

            if (Body == null)
            {
                velocity = moveVector;
            }
            else
            {
                Vector3 requestedVelocity = Body.Velocity;
                Vector3 realVelocity = Body.GetRealVelocity();
                velocity = new Vector3(
                    requestedVelocity.X,
                    ClosestToZero(realVelocity.Y, requestedVelocity.Y),
                    requestedVelocity.Z
                );
            }

            velocity.X = moveVector.X * Speed;// * timingAdjustment;
            velocity.Z = moveVector.Z * Speed;// * timingAdjustment;

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

                if (!IsTryingToMove())
                {
                    climbVelocity = 0;
                }
                else
                {
                    // Update climbing shape-cast's state
                    climbingShapeCast.ForceShapecastUpdate();

                    // Determine the 3d object's normal that we're climbing on
                    // Because the object can have curvy surfaces,
                    // and because the shape-cast can hit multiple objects at once,
                    // we want to use the raycast that "travelled" the smallest distance
                    // as the raycast we're working with.
                    int collisionCount = climbingShapeCast.GetCollisionCount();
                    float shortestDistance = -1f;
                    Vector3 climbingNormal = Vector3.Zero; // Ladder collision normal
                    //logger.Print($"climbingShapeCast reported {collisionCount} collisions");
                    for (int i = 0; i < collisionCount; i++)
                    {
                        if (Climber.IsClimbable(climbingShapeCast.GetCollider(i)))
                        {
                            // climbing shape-cast's "raycasts" can only travel in the Z axis
                            float distance = Math.Abs(climbingShapeCast.ToLocal(climbingShapeCast.GetCollisionPoint(i)).Z);

                            if (shortestDistance < 0f
                                || climbingShapeCast.ToLocal(climbingShapeCast.GetCollisionPoint(i)).Z < shortestDistance)
                            {
                                climbingNormal = climbingShapeCast.GetCollisionNormal(i);
                                shortestDistance = distance;
                            }
                        }
                    }

                    // shortestDistance is only less than zero if we haven't
                    // found a ladder collision normal
                    if (shortestDistance >= 0f)
                    {

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

                        bool shouldClimbUp = Math.Abs(angleDiff) <= (0.45 * Math.PI);

                        if (shouldClimbUp)
                        {
                            climbVelocity = Speed;// * timingAdjustment;
                        }
                        else
                        {
                            bool shouldClimbDown = Math.Abs(angleDiff) >= (0.55 * Math.PI);

                            if (shouldClimbDown)
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
                                    climbVelocity = -Speed;// * timingAdjustment;
                                }
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
        /// Calculates velocity for an axis based on the acceleration as specified in
        /// <see cref="SpeedUpAcceleration"/> and <see cref="SlowDownAcceleration"/>.
        /// </summary>
        /// <param name="currentVelocity"></param>
        /// <param name="goalVelocity">The velocity to eventually achieve</param>
        /// <param name="timeDelta">Number of seconds since the last physics frame</param>
        /// <param name="speedUpAcceleration"></param>
        /// <param name="slowDownAcceleration">This value must be negative in order for slowing down to work properly</param>
        /// <returns></returns>
        private static float CalculateVelocity(
            float currentVelocity,
            float goalVelocity,
            float timeDelta,
            float speedUpAcceleration,
            float slowDownAcceleration,
            bool isSpeedingUp
            )
        {
            float acceleration;
            float newVelocity = 0f;

            if (isSpeedingUp)
            {
                acceleration = speedUpAcceleration;
                if (goalVelocity > currentVelocity)
                {
                    newVelocity = Math.Min(goalVelocity, currentVelocity + (acceleration * timeDelta));
                }
                else if (goalVelocity < currentVelocity)
                {
                    newVelocity = Math.Max(goalVelocity, currentVelocity - (acceleration * timeDelta));
                }
                else
                {
                    newVelocity = goalVelocity;
                }
            }
            else
            {
                // If we're already stopped, you can't slow down any further.
                if (currentVelocity == 0f) return 0f;

                acceleration = slowDownAcceleration;
                if (currentVelocity > 0)
                {
                    newVelocity = Math.Max(0f, currentVelocity + (acceleration * timeDelta));
                }
                else if (currentVelocity < 0)
                {
                    newVelocity = Math.Min(0f, currentVelocity - (acceleration * timeDelta));
                }
            }

            return newVelocity;
        }

        /// <summary>
        /// Calculates XZ velocity using Vector2.
        /// x-velocity maps to the Vector2's x-coordinate,
        /// and z-velocity maps to the Vector2's y-coordinate.
        /// </summary>
        /// <param name="currentVelocity"></param>
        /// <param name="goalVelocity"></param>
        /// <param name="acceleration"></param>
        /// <param name="physicsStepDelta"></param>
        /// <returns></returns>
        private static Vector2 ApproachXZVelocity(
            Vector2 currentVelocity,
            Vector2 goalVelocity,
            float acceleration,
            float physicsStepDelta
        )
        {
            // The direction that velocity is changing in.
            // We only calculate this based on X and Z movement, since
            // we don't want the value of the Acceleration variable
            // to affect upward and downward movement.
            // We are therefore using a Vector2 instead of a Vector3 here to calculate direction change for the X and Z movement
            // (for optimization purposes) (the Vector2's Y-value would be the Z component of our 3d velocity).
            Vector2 direction = (new Vector2(goalVelocity.X, goalVelocity.Y) - new Vector2(currentVelocity.X, currentVelocity.Y)).Normalized();
            Vector2 xzVelocityDelta = direction * acceleration * physicsStepDelta;

            Vector2 finalVelocity = currentVelocity + new Vector2(xzVelocityDelta.X, xzVelocityDelta.Y);

            // We don't want velocity changes to "overshoot" past the destination velocity.
            // Therefore, we'll have to snap the velocity to the goal velocity once it's time to do so.
            // We know we've shot past the goal velocity if the direction from the current velocity to the goal velocity
            // changed as a result of applying acceleration for this frame.
            Vector2 newXZVelocityDiff = new Vector2(goalVelocity.X, goalVelocity.Y) - new Vector2(finalVelocity.X, finalVelocity.Y);
            //logger.Print($"Velocity angle diff: {newXZVelocityDiff.Normalized().Angle() - direction.Angle()}");
            if (newXZVelocityDiff.Length() <= VELOCITY_DIFF_SNAP_THRESHOLD
            || Mathf.IsZeroApprox(newXZVelocityDiff.Normalized().Angle() - direction.Angle()) == false
            )
            {
                finalVelocity = goalVelocity;
                //logger.Print("Snapped velocity");
            }

            return finalVelocity;
        }

        private static float ClosestToZero(float a, float b) => Math.Abs(a) < Math.Abs(b) ? a : b;
        
        /// <summary>
        /// This nested class is intended to "smooth" pushing RigidBody3Ds.
        /// <br/><br/>
        /// When Godot thinks the character is colliding with the RigidBody3D at multiple spots in the same physics frame,
        /// pushing the RigidBody3D in the right direction can be RNG.
        /// <br/><br/>
        /// Therefore, this class was made to assist with making a push in a physically valid position that's as close to
        /// the rigid body's center of mass as possible.
        /// </summary>
        class RigidBodyPusher
        {
            public RigidBody3D Body = null;

            /// <summary>
            /// Position offset from <see cref="Body"/>'s origin in global coordinates. 
            /// </summary>
            public Vector3 PositionOffset = Vector3.Zero;

            public Vector3 PushForce = Vector3.Zero;

            /// <summary>
            /// Number of consecutive physics frames that the character has *not* touched <see cref="Body"/> 
            /// </summary>
            public int ConsecutiveFramesUntouched = 0;

            public void Push() => Body.ApplyForce(PushForce, PositionOffset);
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
                float fDelta = (float)delta;
                float yaw = GetYaw();

                // Make sure this physics frame picks up the latest character rotation
                //Rotator.Update(yaw);
                BodyRotator rotator = Rotator;

                // Only rotate if the rotation if fast turn is enabled or when the character is moving
                if (rotator != null)
                {
                    if (IsFastTurnEnabled)
                    {
                        // Set the angle to the camera's yaw
                        rotator.Yaw = yaw;
                        rotator.Update(delta);
                    }
                    else if (ForwardValue != 0 || RightValue != 0)
                    {
                        // Thanks to Godot 4.0 .NET thirdperson controller by vaporvee for helping me figure this one out
                        // The extra radians are added on top of the original camera yaw, since
                        // the direction of the character should be determined by the yaw corresponding to the move vector
                        // relative to the camera yaw.
                        rotator.Yaw = yaw + (float)Math.Atan2(-RightValue, -ForwardValue);
                        rotator.GradualTurnEnabled = IsJumping || (!IsClimbing);
                        rotator.Update(delta);
                    }
                }

                // The velocity we want to approach
                Vector3 lastVelocity = LastVelocity;
                Vector3 moveVelocity = GetMoveVelocity(fDelta, yaw);

                Vector2 lastXZVelocity = new Vector2(lastVelocity.X, lastVelocity.Z);
                Vector2 goalXZVelocity = new Vector2(moveVelocity.X, moveVelocity.Z);

                // Determine which value of acceleration to use
                float acceleration = 0f;
                bool isTryingToMove = IsTryingToMove();
                bool hasExceededMaxSpeed = lastXZVelocity.Length() > Speed;
                bool canMoveFaster = isTryingToMove && hasExceededMaxSpeed == false;

                if (IsOnFloor())
                {
                    if (canMoveFaster)
                    {
                        acceleration = Acceleration;
                    }
                    else
                    {
                        acceleration = Deceleration;
                    }
                }
                else
                {
                    if (canMoveFaster)
                    {
                        acceleration = AirAcceleration;
                    }
                    else
                    {
                        acceleration = AirDeceleration;
                    }
                }

                // Apply acceleration
                // Acceleration should be relative to the change in direction based
                // on how the currently requested velocity differs from the previous velocity.
                Vector2 newXZvelocity = ApproachXZVelocity(
                    lastXZVelocity,
                    goalXZVelocity,
                    acceleration,
                    fDelta
                );
                Vector3 finalVelocity = new Vector3(newXZvelocity.X, moveVelocity.Y, newXZvelocity.Y);
                //logger.Print($"Current velocity: {lastVelocity} | Velocity after MoveAndSlide: {body.Velocity}");

                Vector3 realVelocity = body.GetRealVelocity();

                // Store the current velocity for the next physics frame to use.
                //
                // When updating LastVelocity, for the Y value, between real velocity after MoveAndSlide and requested velocity after move and slide,
                // use whichever one is closest to 0.
                // This is mainly to *prevent* these two issues:
                // - Character builds up downwards velocity when IsOnFloor() returns false but the character
                //   is not moving downward.
                // - Character suddenly and unexpectedly jolts upward at a high velocity.
                Vector3 requestedVelocityAfterMove = body.Velocity;
                LastVelocity = new Vector3(
                    requestedVelocityAfterMove.X,
                    ClosestToZero(realVelocity.Y, requestedVelocityAfterMove.Y),
                    requestedVelocityAfterMove.Z
                    );

                // Figure out how to push objects we've come into contact with. This part intentionally comes before the call to MoveAndSlide().
                // Thanks to this forum post for helping me figure out how to implement this:
                // https://forum.godotengine.org/t/how-to-fix-movable-box-physics/75853
                // as well as this code snippet by majikayogames on GitHub
                // https://gist.github.com/majikayogames/cf013c3091e9a313e322889332eca109
                Dictionary<RigidBody3D, RigidBodyPusher> currentFrameRigidBodyPushers = [];
                for (int i = 0; i < body.GetSlideCollisionCount(); i++)
                {
                    KinematicCollision3D collision = body.GetSlideCollision(i);
                    if (collision.GetCollider() is RigidBody3D rigidBody)
                    {
                        RigidBodyPusher pusher;
                        if (rigidBodyPushers.TryGetValue(rigidBody, out pusher))
                        {
                            // ConsecutiveFramesUntouched will get set to 0 in an upcoming loop
                            pusher.ConsecutiveFramesUntouched = -1;
                            continue;
                        }

                        Vector3 collisionNormal = collision.GetNormal();
                        Vector3 forcePositionOffset = collision.GetPosition() - rigidBody.GlobalPosition + collisionNormal * collision.GetDepth();

                        // Works, but slightly buggy. Friction really needs to be implemented properly if we want to this calculate push force this way.
                        //Vector3 pushForce = requestedVelocityAfterMove.Normalized() * Mass * acceleration * fDelta;

                        // Also works, but still slightly buggy.
                        // With this push force calculation, moving balls by standing on one of their sloped positions causes the balls and the character to fly around.
                        //Vector3 pushForce = -collisionNormal * Mass * acceleration * fDelta;

                        // Seems to work the best so far.
                        // This one also takes the character's current travel speed into account, making push force a little more realistic.
                        // This formula also has the same behavior with moving balls as mentioned above, but oh well.
                        // At the end of the day, 3D platformers typically don't have realistic physics anyways.
                        //Vector3 pushForce = -collisionNormal * Mass * acceleration * fDelta * newXZvelocity.Length()

                        // We could also try only using move velocity
                        // float currentMass = Mass; // To prevent race conditions
                        // Vector3 pushForce = moveVelocity.Normalized() * currentMass * acceleration * fDelta * new Vector3(
                        //     moveVelocity.X,
                        //     Mass,
                        //     moveVelocity.Z
                        // ).Length() / new Vector3(Speed, currentMass, 0).Length();

                        // or more accurately
                        //Vector3 pushForce = moveVelocity.Normalized() * Mass * acceleration * fDelta;
                        //Console.WriteLine($"normalized move velocity: {moveVelocity.Normalized()}");

                        // Based on some info online as well as the implementation of RigidBody3D.ApplyForce(),
                        // this seems like the best approach

                        // We only want to push downwards or upwards if the absolute value of the character's vertical velocity
                        // is greater than the absolute value of the vertical velocity of the object to push.
                        //collisionNormal.Y *= Math.Max(0f, realVelocity.Y / rigidBody.LinearVelocity.Y);

                        Vector3 pushDirection = -collisionNormal;

                        // The amount in which rigidBody.Velocity has to change by to match pushDirection.
                        // This number has a minimum of 0 to ensure that the character only pushes objects that it's travelling towards.
                        float diffToPushDirection = Math.Max(0f, finalVelocity.Dot(pushDirection) - rigidBody.LinearVelocity.Dot(pushDirection));

                        // If rigidBody has more mass, it should be harder to push.
                        float massRatio = Mass / rigidBody.Mass;

                        // Allowing the character to push objects from above and below an object has caused some issues,
                        // so it makes sense to prevent such behavior.
                        pushDirection.Y = 0f;

                        // Put it together
                        Vector3 pushForce = pushDirection * diffToPushDirection * massRatio * ForceMultiplier;

                        Console.WriteLine($"Calculated push force for {rigidBody.Name} to be {pushForce}\n\tNegative collision normal: {-collisionNormal}\n\tdiffToPushDirection: {Mathf.RadToDeg(diffToPushDirection)} degrees\n\tMass ratio:{massRatio}");

                        if (currentFrameRigidBodyPushers.TryGetValue(rigidBody, out pusher))
                        {
                            Vector3 centerOfMass = rigidBody.CenterOfMass;
                            if ((forcePositionOffset - centerOfMass).Length() < (pusher.PositionOffset - centerOfMass).Length())
                            {
                                pusher.PositionOffset = forcePositionOffset;
                                pusher.PushForce = pushForce;
                            }
                        }
                        else
                        {
                            pusher = new RigidBodyPusher
                            {
                                Body = rigidBody,
                                PositionOffset = forcePositionOffset,
                                PushForce = pushForce
                            };

                            currentFrameRigidBodyPushers.Add(rigidBody, pusher);
                            rigidBodyPushers.Add(rigidBody, pusher);
                        }
                    }
                }

                // Do the actual RigidBody3D pushing.
                if (currentFrameRigidBodyPushers.Count > 0) Console.WriteLine("---- ITERATING THROUGH RIGIDBODYPUSHERS ----");
                foreach (RigidBodyPusher pusher in currentFrameRigidBodyPushers.Values)
                {
                    pusher.Push();
                    Console.WriteLine($"Pushed {pusher.Body.Name}\n\tPush force: {pusher.PushForce}\n\tBody's current velocity: {pusher.Body.LinearVelocity}\n\tWhere force was applied (relative to body origin): {pusher.PositionOffset}");
                }
                if (currentFrameRigidBodyPushers.Count > 0) Console.WriteLine("--------------------------------------------");

                // Move the character.
                body.Velocity = finalVelocity;
                body.MoveAndSlide();
                
                // Update ConsecutiveFramesUntouched count for each RigidBodyPusher in the rigidBodyPushers list,
                // and remove from the list as necessary.
                foreach (RigidBodyPusher pusher in rigidBodyPushers.Values)
                {
                    pusher.ConsecutiveFramesUntouched += 1;

                    if (pusher.ConsecutiveFramesUntouched >= RIGID_BODY_MAX_CONSECUTIVE_FRAMES_UNTOUCHED) rigidBodyPushers.Remove(pusher.Body);
                }

                // Update current body state.
                // We use the character's real velocity here because it gives a more accurate description of how the character
                // is currently moving, especially for vertical movement.
                if (IsJumping && realVelocity.Y > 0)
                {
                    // Jumping is placed first in line so that jumping can affect climbing
                    CurrentBodyState = BodyState.Jumping;
                }
                else if (IsClimbing)
                {
                    CurrentBodyState = BodyState.Climbing;
                }
                else if (IsJumping == false && realVelocity.Y > 0)
                {
                    CurrentBodyState = BodyState.Rising;
                }
                else if (realVelocity.Y < 0)
                {
                    CurrentBodyState = BodyState.Falling;
                }
                else if ((realVelocity.X != 0 || realVelocity.Z != 0) && IsOnFloor())
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
        /*
        public void HandleProcessStep(double delta)
        {
            BodyRotator rotator = Rotator;

            // Only rotate if the rotation if fast turn is enabled or when the character is moving
            if (rotator != null)
            {
                if (IsFastTurnEnabled)
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
                    rotator.GradualTurnEnabled = IsJumping || (!IsClimbing);
                    rotator.Update(delta);
                }
            }
        }
        */

        /// <summary>
        /// Disposes of this <see cref="BaseMover"/>
        /// </summary>
        public new void Dispose()
        {
            IsRunning = false;
            QueueFree();

            // Currently, the Climber being used in this class is created during BaseMover's instantiation and from nowhere else
            CurrentClimber.Dispose();
            CurrentClimber = null;

            climbingShapeCast.QueueFree();
            climbingShapeCast.Dispose();
            climbingShapeCast = null;

            base.Dispose();
        }

        public override void _PhysicsProcess(double delta)
        {
            HandlePhysicsStep(delta);
            base._PhysicsProcess(delta);
        }

        /*
        public override void _Process(double delta)
        {
            HandleProcessStep(delta);
            base._Process(delta);
        }
        */

        /// <summary>
        /// Event that's raised when the character being moved by this <see cref="BaseMover"/> changes.
        /// </summary>
        public event EventHandler<BodyStateChangedArgs> BodyStateChanged;

        protected void RaiseBodyStateChangedEvent(BodyState oldState, BodyState newState)
        {
            BodyStateChanged?.Invoke(this, new BodyStateChangedArgs(oldState, newState));
        }

        /// <summary>
        /// Event that's raised when the value of <see cref="IsFastTurnEnabled"/> changes.
        /// The boolean event argument is the new value of <see cref="IsFastTurnEnabled"/>. 
        /// </summary>
        public event EventHandler<bool> OnFastTurnToggled;

        protected void RaiseOnFastTurnToggled(bool fastTurnEnabled)
        {
            OnFastTurnToggled?.Invoke(this, fastTurnEnabled);
        }
    }
}
