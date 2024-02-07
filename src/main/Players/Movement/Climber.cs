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

            updateRayCast();
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

        private void updateRayCast()
        {
            CollisionShape3D hitbox = Hitbox;
            if (hitbox == null) return;

            BoxShape3D collisionBox = hitbox.Shape as BoxShape3D;

            // For now, only box-shaped hitboxes will work.
            if (collisionBox == null) return;

            Vector3 boxSize = collisionBox.Size;

            // Start from center-height, then work our way to the bottom.
            // Remember that the position of the ray-cast is already relative to the hitbox's position
            // as it is parented to the hitbox itself
            rayCast.Position = new Vector3(0, 0, -boxSize.Z / 2 - 0.05f);
            rayCast.TargetPosition = new Vector3(0, -boxSize.Y / 2, 0);
        }

        private void updateArea()
        {
            CollisionShape3D hitbox = Hitbox;
            if (hitbox == null) return;

            BoxShape3D collisionBox = hitbox.Shape as BoxShape3D;

            // For now, only box-shaped hitboxes will work.
            if (collisionBox == null) return;

            Vector3 hitboxSize = collisionBox.Size;
            areaBox.Size = new Vector3(0.2f, hitboxSize.Y / 2, 0.1f);

            // Remember, position of the area is relative to the position of the hitbox.
            area.Position = new Vector3(0, -hitboxSize.Y / 4, -hitboxSize.Z / 2 - areaBox.Size.Z / 2);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            updateRayCast();

            PhysicsBody3D collidedObject = rayCast.GetCollider() as PhysicsBody3D;

            // If the collided object is a PhysicsBody3D and it has a metadata entry
            // named "is_climbable" set to true, we can climb.
            bool canClimb = collidedObject != null
                && collidedObject.HasMeta(IS_CLIMBABLE_METADATA_NAME)
                && collidedObject.GetMeta(IS_CLIMBABLE_METADATA_NAME).AsBool() == true;

            if (canClimb)
            {
                CurrentlyClimbedObject = collidedObject;
                RaycastCollisionPoint = rayCast.GetCollisionPoint();
            }
            else
            {
                CurrentlyClimbedObject = null;
            }

            CanClimb = canClimb;

            //Vector3 boxPos = Hitbox.Position;

            // The raycast's position should be relative to the position and rotation of the character's hitbox
            //rayCast.Position = BoxPos + new Vector3(0, 0, BoxSize.Z / 2).Rotated(Vector3.Up, BoxRotation.Y);
            //rayCast.TargetPosition = new Vector3(0, BoxSize.Y / 2, 0);
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
