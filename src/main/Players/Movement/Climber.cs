using Godot;
using System;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// Allows a character to climb objects. Specifically, these objects are <see cref="PhysicsBody3D"/>s
    /// A PhysicsBody3D can be climbed if it has a boolean metadata entry named "is_climbable" that's set to true.
    /// </summary>
    public partial class Climber : Node, IDisposable
    {
        /// <summary>
        /// Name of the metadata entry that specifies whether or not a <see cref="PhysicsBody3D"/> is climbable
        /// </summary>
        public readonly string IS_CLIMBABLE_METADATA_NAME = "is_climbable";

        /// <summary>
        /// The <see cref="Area3D"/> that allows the character to climb when a climbable PhysicsBody3D is intersecting with it
        /// </summary>
        private Area3D area;

        /// <summary>
        /// The box that defines <see cref="area"/>'s region
        /// </summary>
        private BoxShape3D areaBox;

        private bool _canClimb = false;

        /// <summary>
        /// Whether or not the character is able to climb because the raycast has hit a climbable <see cref="PhysicsBody3D"/>
        /// </summary>
        public bool CanClimb
        {
            get => _canClimb;
            private set
            {
                if (_canClimb == value) return;

                _canClimb = value;
                RaiseOnCanClimbChanged(value);
            }
        }

        /// <summary>
        /// The most recent collison point where the <see cref="Climber"/>'s raycast hit a climbable <see cref="PhysicsBody3D"/>.
        /// When a <see cref="Climber"/> object is first instantiated, this is set to <see cref="Vector3.Zero"/>.
        /// <br/>
        /// Whatever Vector3 this variable is set to should be accurate as long as <see cref="CanClimb"/> is true.
        /// </summary>
        public Vector3 RaycastCollisionPoint { get; private set; } = Vector3.Zero;

        /// <summary>
        /// The <see cref="PhysicsBody3D"/> that's currently being climbed.
        /// This is set to null if we aren't climbing anything currently.
        /// </summary>
        public PhysicsBody3D CurrentlyClimbedObject { get; private set; } = null;

        private CollisionShape3D _hitbox;

        /// <summary>
        /// The character's hitbox that this <see cref="Climber"/> is associated with.
        /// </summary>
        public CollisionShape3D Hitbox
        {
            get => _hitbox;
            set
            {
                CollisionShape3D oldHitbox = _hitbox;
                if (oldHitbox != null)
                {
                    oldHitbox.RemoveChild(area);
                }

                _hitbox = value;
                
                if (value != null)
                {
                    value.AddChild(area);
                }
            }
        }

        /// <summary>
        /// The width of the climbing hitbox.
        /// This length is parallel to the x-axis that's relative to the position and rotation of the character's hitbox.
        /// </summary>
        public float HitboxWidth = 0.25f;

        /// <summary>
        /// The depth of the climbing hitbox.
        /// This length is parallel to the z-axis that's relative to the position and rotation of the character's hitbox.
        /// </summary>
        public float HitboxDepth = 0.2f;

        /// <summary>
        /// Creates a new instance of <see cref="Climber"/>
        /// </summary>
        public Climber(CollisionShape3D hitbox)
        {
            Name = $"{nameof(Climber)}_{GetHashCode()}";

            area = new Area3D();
            area.Name = $"{nameof(Climber)}_{GetHashCode()}_{nameof(area)}";

            areaBox = new BoxShape3D();

            CollisionShape3D areaShape = new CollisionShape3D();
            areaShape.Shape = areaBox;
            area.AddChild(areaShape);

            Hitbox = hitbox;

            updateArea();
        }

        /// <summary>
        /// Disposes of this <see cref="Climber"/> object and the <see cref="RayCast3D"/> instance being used to power it
        /// </summary>
        public new void Dispose()
        {
            QueueFree();
            area.QueueFree();
            area.Dispose();
            base.Dispose();
        }

        private void updateArea()
        {
            CollisionShape3D hitbox = Hitbox;
            if (hitbox == null) return;

            BoxShape3D collisionBox = hitbox.Shape as BoxShape3D;

            // For now, only box-shaped hitboxes will work.
            if (collisionBox == null) return;

            Vector3 hitboxSize = collisionBox.Size;
            areaBox.Size = new Vector3(HitboxWidth, hitboxSize.Y / 2, HitboxDepth);

            // Remember, position of the area is relative to the position of the hitbox.
            area.Position = new Vector3(0, -hitboxSize.Y / 4, -hitboxSize.Z / 2 - areaBox.Size.Z / 2);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            updateArea();

            bool canClimb = false;
            foreach (Node3D n in area.GetOverlappingBodies())
            {
                PhysicsBody3D collidedObject = n as PhysicsBody3D;

                if (collidedObject != null)
                {
                    // If the collided object is a PhysicsBody3D and it has a metadata entry
                    // named "is_climbable" set to true, we can climb.
                    canClimb = collidedObject != null
                        && collidedObject.HasMeta(IS_CLIMBABLE_METADATA_NAME)
                        && collidedObject.GetMeta(IS_CLIMBABLE_METADATA_NAME).AsBool() == true;

                    if (canClimb)
                    {
                        CurrentlyClimbedObject = collidedObject;
                    }
                    else
                    {
                        CurrentlyClimbedObject = null;
                    }

                    // We don't need to look through the rest of the list
                    // if we know we're already on a climbable object
                    if (canClimb)
                    {
                        break;
                    }
                }
            }

            CanClimb = canClimb;
        }

        /// <summary>
        /// Event that's raised when the value of <see cref="CanClimb"/> changes.
        /// The 2nd argument for this event is a boolean that's set to the new value of <see cref="CanClimb"/>.
        /// </summary>
        public event EventHandler<bool> OnCanClimbChanged;

        protected void RaiseOnCanClimbChanged(bool canClimb)
        {
            OnCanClimbChanged?.Invoke(this, canClimb);
        }
    }
}
