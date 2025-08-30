using Godot;
using System;
using System.Collections.Generic;

using UTheCat.Jumpvalley.Core.Levels.Interactives.Mechanics;
using UTheCat.Jumpvalley.Core.Players.Camera;

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
        /// This number is in meters.
        /// </summary>
        private static readonly float STEP_CLIMB_BOOST_WALL_SLOPE_ANGLE_RAYCAST_Y_OFFSET = -0.005f;

        /// <summary>
        /// This number is in meters.
        /// </summary>
        private static readonly float STEP_CLIMB_BOOST_WALL_SLOPE_ANGLE_RAYCAST_LENGTH = 0.1f;

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

        /// <summary>
        /// When making contact with a step and travelling towards the step's position, the character
        /// will automatically climb the step without jumping if the character Y-position boost needed
        /// to achieve this is within the range of (0, <i>the value of this field</i>]. This also applies
        /// if the contact was made midair.
        /// <br/><br/>
        /// To avoid potentially unwanted upward boosts, avoid setting this value too high.
        /// <br/><br/>
        /// This number is in meters.
        /// </summary>
        public float AutoClimbStepMaxYBoost = 0.505f;

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

        /// <summary>
        /// Multiplier for the force applied by a <i>colliding</i> rigid body to the character being handled by this <see cref="BaseMover"/>. 
        /// </summary>
        public float CharacterPushForceMultiplier = 5f;

        private Vector2 lastXZVelocity = Vector2.Zero;

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
        /// Gets the velocity that the character wants to move at for the current physics frame
        /// </summary>
        /// <param name="delta">The time it took to complete the physics frame in seconds</param>
        /// <param name="yaw">The yaw angle to make the move vector relative to.</param>
        /// <returns></returns>
        public Vector3 GetMoveVelocity(float delta, float yaw)
        {
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

            velocity.X = moveVector.X * Speed;
            velocity.Z = moveVector.Z * Speed;

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
                        float angleDiff = climbingNormal.Rotated(Vector3.Up, (float)Math.PI).SignedAngleTo(moveVector, Vector3.Up);

                        bool shouldClimbUp = Math.Abs(angleDiff) <= (0.45 * Math.PI);

                        if (shouldClimbUp)
                        {
                            climbVelocity = Speed;
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
                                    climbVelocity = -Speed;
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

            if (newXZVelocityDiff.Length() <= VELOCITY_DIFF_SNAP_THRESHOLD
            || Mathf.IsZeroApprox(newXZVelocityDiff.Normalized().Angle() - direction.Angle()) == false
            )
            {
                finalVelocity = goalVelocity;
            }

            return finalVelocity;
        }

        private static float ClosestToZero(float a, float b) => Math.Abs(a) < Math.Abs(b) ? a : b;

        /// <summary>
        /// Internal class for assisting <see cref="BaseMover"/> in figuring out how to apply force on rigid bodies
        /// that the mover's character has come into contact with.
        /// <br/><br/>
        /// Additionally, this class helps <see cref="BaseMover"/> figure out how its character's velocity should change when a rigid body
        /// hits the character.
        /// <br/><br/>
        /// For performance reasons, this class considers any contact between the rigid body and the character as an opportunity for
        /// force to be applied on both the rigid body and the character at the same time. 
        /// </summary>
        class RigidBodyPusher
        {
            public RigidBody3D Body = null;

            public CharacterBody3D Character = null;

            /// <summary>
            /// <see cref="Character"/>'s mover. 
            /// </summary>
            public BaseMover Mover = null;

            /// <summary>
            /// List of global-space coordinates that Body and Character are touching during the current physics frame
            /// </summary>
            public List<Vector3> CollisionPoints = [];

            /// <summary>
            /// The closest distance between the center of the character and
            /// the point at which <see cref="Body"/> and <see cref="Character"/> collided.
            /// </summary>
            public float CharCenterToCollisionPosSmallestDist = 0f;

            /// <summary>
            /// Force applied by <see cref="Body"/> on <see cref="Character"/>  
            /// </summary>
            //public Vector3 CharacterPushForce = Vector3.Zero;

            /// <summary>
            /// Collision normal to use when figuring out how to apply force to the character.
            /// This collision normal is the normal on the rigid body in which <see cref="Body"/>
            /// and <see cref="Character"/> touched.  
            /// </summary>
            public Vector3 CollisionNormal = Vector3.Zero;

            private Vector3 GetAvgCollisionPoint()
            {
                if (CollisionPoints.Count == 0) return Vector3.Zero;
                if (CollisionPoints.Count == 1) return CollisionPoints[0];

                Vector3 total = Vector3.Zero;
                foreach (Vector3 c in CollisionPoints)
                {
                    total += c;
                }

                return total / CollisionPoints.Count;
            }

            /// <summary>
            /// Pushes <see cref="Body"/>. 
            /// </summary>
            /// <param name="characterCurrentVelocity"></param>
            /// <param name="characterTargetVelocity"></param>
            public void PushRigidBody(Vector3 characterCurrentVelocity, Vector3 characterTargetVelocity)
            {
                if (characterCurrentVelocity == Vector3.Zero && characterTargetVelocity == Vector3.Zero) return;

                Vector3 charVelocity = (characterCurrentVelocity.Length() > characterTargetVelocity.Length()) ? characterCurrentVelocity : characterTargetVelocity;

                Vector3 pushDirection = charVelocity.Normalized();
                float forceMultiplier = charVelocity.Dot(pushDirection) - Body.LinearVelocity.Dot(pushDirection);

                // If character doesn't have enough velocity to overcome rigid body velocity
                // or if we know for sure that force application is physically impossible, stop.
                if (forceMultiplier <= 0) return;

                forceMultiplier *= Mover.ForceMultiplier * Mover.Mass / Body.Mass;

                Body.ApplyForce(pushDirection * forceMultiplier, GetAvgCollisionPoint() - Body.GlobalPosition);
            }

            /// <summary>
            /// Returns the change in velocity corresponding to the force being applied to <see cref="Character"/> 
            /// </summary>
            /// <param name="characterCurrentVelocity"></param>
            /// <param name="characterTargetVelocity"></param>
            public Vector3 GetCharacterVelocityChange(Vector3 characterCurrentVelocity)
            {
                // The amount in which finalVelocity (character velocity) has to change to match rigidBody.Velocity.
                float charVelocityRigidBodyVelocityDiff = Body.LinearVelocity.Dot(CollisionNormal) - characterCurrentVelocity.Dot(-CollisionNormal);

                // If we know for sure that applying the force would be physically impossible, don't apply the force.
                if (charVelocityRigidBodyVelocityDiff <= 0) return Vector3.Zero;

                // Objects heavier than the character should be able to push the character with greater force.
                float massRatio = Body.Mass / Mover.Mass;

                Vector3 characterPushForce = CollisionNormal * charVelocityRigidBodyVelocityDiff * massRatio * Mover.CharacterPushForceMultiplier;

                // Some rigid bodies absorb force on impact. Account for this.
                PhysicsMaterial rigidBodyPhysicsMaterial = Body.PhysicsMaterialOverride;
                if (rigidBodyPhysicsMaterial != null) characterPushForce *= 1f - rigidBodyPhysicsMaterial.Bounce;

                // We already divided by the mass of the character to get acceleration from force, there's no need to do it again
                return characterPushForce;
            }
        }

        private float GetCharacterHeight()
        {
            CharacterBody3D body = Body;

            if (body == null || !body.HasMeta(OverallBoundingBoxObject.CUSTOM_OVERALL_BOUNDING_BOX_META_NAME)) return 0;

            Aabb boundingBox = body.GetMeta(OverallBoundingBoxObject.OVERALL_BOUNDING_BOX_META_NAME).As<Aabb>();

            return boundingBox == default ? 0.0f : boundingBox.Size.Y;
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
                float acceleration;
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
                Vector3 realVelocity = body.GetRealVelocity();

                float stepClimbHighestYBoost = 0f;

                // Handle collisions.
                // For performance reasons, we handle the step-climb boost, as well as rigid-body and character pushing,
                // in the same iteration within this loop.
                Dictionary<RigidBody3D, RigidBodyPusher> currentFrameRigidBodyPushers = [];
                for (int i = 0; i < body.GetSlideCollisionCount(); i++)
                {
                    KinematicCollision3D collision = body.GetSlideCollision(i);
                    GodotObject collider = collision.GetCollider();
                    Vector3 kinematicCollisionPos = collision.GetPosition();

                    // See if we can automatically climb a step (perform a "step-climb boost")
                    //
                    // In some cases, the player will just barely miss a platform because they almost (but didn't)
                    // gain enough height (e.g. after a high or long jump).
                    // When this happens, we want to give the player's character a small upward boost so they can make the jump.
                    //
                    // In other cases, the player wants to climb up a short platform (e.g. staircase step) without jumping.
                    // We want to allow this.
                    float stepClimbMaxYBoost = AutoClimbStepMaxYBoost;
                    if (
                        stepClimbMaxYBoost > 0
                        && collider is PhysicsBody3D physicsBodyCollider
                        && (!IsClimbing || CurrentClimber == null || physicsBodyCollider != CurrentClimber.CurrentlyClimbedObject)
                        )
                    {
                        float characterBottomYPosLocal = -GetCharacterHeight() / 2f;
                        Vector3 characterBottom = body.ToGlobal(new Vector3(0, characterBottomYPosLocal, 0));
                        Vector3 characterBottomWithOffset = body.ToGlobal(new Vector3(0, characterBottomYPosLocal + stepClimbMaxYBoost, 0));
                        PhysicsDirectSpaceState3D spaceState = body.GetWorld3D().DirectSpaceState;

                        PhysicsRayQueryParameters3D climbBoostRayParams = PhysicsRayQueryParameters3D.Create(
                                new Vector3(kinematicCollisionPos.X, characterBottomWithOffset.Y, kinematicCollisionPos.Z),
                                new Vector3(kinematicCollisionPos.X, characterBottom.Y, kinematicCollisionPos.Z)
                                );
                        climbBoostRayParams.HitFromInside = true;

                        var stepClimbResults = spaceState.IntersectRay(climbBoostRayParams);

                        // stepClimbResults won't be empty if we can get a collision normal this way
                        Variant vCollisionNormal;
                        if (stepClimbResults.TryGetValue("normal", out vCollisionNormal))
                        {
                            // Step-climb boost shouldn't be given for wallhops.
                            // If the step-climb raycast didn't hit from inside, we're safe to say that
                            // we're not dealing with a wallhop.
                            //
                            // Additionally, this collisionNormal lets us figure out if the surface to step-climb *onto*
                            // has a slope angle that's low enough to walk on normally.
                            Vector3 collisionNormalToStepOnto = vCollisionNormal.As<Vector3>();
                            if (collisionNormalToStepOnto != Vector3.Zero && !(MathF.Acos(collisionNormalToStepOnto.Y) > body.FloorMaxAngle))
                            {
                                // stepClimbResults won't be empty if we can get a position this way
                                Variant vStepClimbRayCollisionPos;
                                if (stepClimbResults.TryGetValue("position", out vStepClimbRayCollisionPos))
                                {
                                    Vector3 rayCollisionPos = vStepClimbRayCollisionPos.As<Vector3>();
                                    Vector3 slopeAngleRaycastPositionBase = new Vector3(rayCollisionPos.X, rayCollisionPos.Y + STEP_CLIMB_BOOST_WALL_SLOPE_ANGLE_RAYCAST_Y_OFFSET, rayCollisionPos.Z);

                                    // We'll need a separate raycast for detecting the slope angle of the face of the platform that the character is trying to climb up
                                    // (e.g. if this angle is 90 degrees, the face of the platform would be considered a "wall")
                                    var slopeAngleRaycastResults = spaceState.IntersectRay(
                                        PhysicsRayQueryParameters3D.Create(
                                            slopeAngleRaycastPositionBase - new Vector3(finalVelocity.X, 0, finalVelocity.Z).Normalized() * STEP_CLIMB_BOOST_WALL_SLOPE_ANGLE_RAYCAST_LENGTH,
                                            slopeAngleRaycastPositionBase,
                                            4294967295,
                                            [body.GetRid()]
                                        )
                                    );

                                    Variant vSlopeNormal;
                                    if (slopeAngleRaycastResults.TryGetValue("normal", out vSlopeNormal))
                                    {
                                        // We only have to give the step-boost if the surface is too steep to just walk on.
                                        if (MathF.Acos(vSlopeNormal.As<Vector3>().Y) > body.FloorMaxAngle)
                                        {
                                            float characterYBoost = rayCollisionPos.Y - characterBottom.Y;

                                            // We only want to give the step-climb boost once per frame at most
                                            if (characterYBoost > 0f && characterYBoost > stepClimbHighestYBoost)
                                            {
                                                stepClimbHighestYBoost = characterYBoost;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (collider is RigidBody3D rigidBody)
                    {
                        RigidBodyPusher pusher;

                        Vector3 collisionNormal = collision.GetNormal();
                        Vector3 collisionPosCharacterPosDiff = kinematicCollisionPos - body.GlobalPosition;

                        // Remember, we only want to add force once per RigidBody3D per physics frame
                        // Likewise, each RigidBody3D can only apply force to the character once per physics frame.
                        if (currentFrameRigidBodyPushers.TryGetValue(rigidBody, out pusher))
                        {
                            pusher.CollisionPoints.Add(kinematicCollisionPos);

                            float collisionPosToCharacterPosDistance = collisionPosCharacterPosDiff.Length();
                            if (collisionPosToCharacterPosDistance < pusher.CharCenterToCollisionPosSmallestDist)
                            {
                                // Character to be pushed by RigidBody3D shouldn't change while we're scanning
                                // for more collision points between the character and the RigidBody3D, and so,
                                // we don't need to "update" push info in this regard.
                                //
                                // However, using a different collision point could potentially mean that the character was hit by the RigidBody3D
                                // at a different collision normal, and so, we still want to update character push force when we've found a collision point
                                // closer to the center of the character.
                                pusher.CharCenterToCollisionPosSmallestDist = collisionPosToCharacterPosDistance;
                                pusher.CollisionNormal = collisionNormal;
                            }
                        }
                        else
                        {
                            pusher = new RigidBodyPusher
                            {
                                Body = rigidBody,
                                CharCenterToCollisionPosSmallestDist = collisionPosCharacterPosDiff.Length(),
                                Character = Body,
                                CollisionNormal = collisionNormal,
                                Mover = this
                            };

                            pusher.CollisionPoints.Add(kinematicCollisionPos);

                            currentFrameRigidBodyPushers.Add(rigidBody, pusher);
                        }
                    }
                }

                // Do the RigidBody3D pushing and modify the "next" character velocity
                // to simulate force being applied by the touching rigid bodies on the character.
                foreach (RigidBodyPusher pusher in currentFrameRigidBodyPushers.Values)
                {
                    finalVelocity += pusher.GetCharacterVelocityChange(finalVelocity);
                    pusher.PushRigidBody(finalVelocity, moveVelocity);
                }

                // Apply step climb if there is one for this frame
                //
                // The result of the step climb should be that the character is standing on a surface
                // whose slope angle is low enough to be considered a "floor"
                if (stepClimbHighestYBoost > 0f)
                {
                    float jumpVelocity = JumpVelocity;

                    // Give them a small y-velocity boost if needed so they can climb the step without jumping
                    finalVelocity.Y = Math.Max(Math.Min(stepClimbHighestYBoost * Gravity / 2f, jumpVelocity / 2f), finalVelocity.Y);

                    // The above logic causes the character to sometimes get stuck on ledges when trying to step-climb.
                    // When this happens, allow the player to jump to escape this.
                    if (IsJumping) finalVelocity.Y = Math.Max(jumpVelocity, finalVelocity.Y);
                }

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

                // Move the character.
                body.Velocity = finalVelocity;
                body.MoveAndSlide();

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
