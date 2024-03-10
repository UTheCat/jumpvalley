using Godot;

namespace Jumpvalley.Raycasting
{
    /// <summary>
    /// Class that contains data about the result of a raycast sweep operation
    /// (where collisions with a <see cref="RaycastSweep"/>'s raycasts are checked).
    /// </summary>
    public partial class RaycastSweepResult
    {
        /// <summary>
        /// The raycast that got collided with.
        /// </summary>
        public RayCast3D Raycast;

        /// <summary>
        /// The global-space position where the raycast hit an object.
        /// </summary>
        public Vector3 CollisionPoint;

        /// <summary>
        /// The <see cref="GodotObject"/> that the raycast hit.
        /// </summary>
        public GodotObject CollidedObject;

        /// <summary>
        /// The numeric index of the raycast that got collided with.
        /// This index should be equal to the index in <see cref="RaycastSweep.Raycasts"/> where this raycast is stored.
        /// </summary>
        public int RaycastIndex;

        public RaycastSweepResult(RayCast3D raycast, Vector3 collisionPoint, GodotObject collidedObject, int raycastIndex)
        {
            Raycast = raycast;
            CollisionPoint = collisionPoint;
            CollidedObject = collidedObject;
            RaycastIndex = raycastIndex;
        }
    }
}
