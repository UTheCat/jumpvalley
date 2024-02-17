using Godot;

namespace Jumpvalley.Raycasting
{
    /// <summary>
    /// Class that contains data about the result of a raycast sweep operation
    /// (where collisions with a <see cref="RaycastSweep"/>'s raycasts are checked).
    /// </summary>
    public partial class RaycastSweepResults
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
        /// The <see cref="Node3D"/> that the raycast hit.
        /// </summary>
        public Node3D CollidedObject;

        public RaycastSweepResults(RayCast3D raycast, Vector3 collisionPoint, Node3D collidedObject)
        {
            Raycast = raycast;
            CollisionPoint = collisionPoint;
            CollidedObject = collidedObject;
        }
    }
}
