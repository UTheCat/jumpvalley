using Godot;
using System;
using System.Collections.Generic;

namespace Jumpvalley.Raycasting
{
    /// <summary>
    /// Sweeps over a region of 3D space using a series of equally-spaced-out raycasts.
    /// </summary>
    public partial class RaycastSweep: Node3D, IDisposable
    {
        /// <summary>
        /// The total number of raycasts that the raycast sweep will perform from the start position to the end position.
        /// </summary>
        public int NumRaycasts { get; private set; }

        /// <summary>
        /// The length in meters that each raycast in the <see cref="RaycastSweep"/> should extend in the relative Z-axis of this <see cref="Node3D"/>
        /// </summary>
        public float RaycastLength { get; private set; }

        /// <summary>
        /// The starting position of the raycast sweep.
        /// This position is relative to the position of this <see cref="Node3D"/> and is where the first raycast will begin.
        /// </summary>
        public Vector3 StartPosition { get; private set; }

        /// <summary>
        /// The ending position of the raycast sweep.
        /// This position is relative to the position of this <see cref="Node3D"/> and is where the last raycast will begin.
        /// </summary>
        public Vector3 EndPosition { get; private set; }

        /// <summary>
        /// The raycasts being used by the <see cref="RaycastSweep"/>.
        /// </summary>
        public List<RayCast3D> Raycasts { get; private set; }

        /// <summary>
        /// Creates a new <see cref="RaycastSweep"/> with the given raycast count, starting position, and ending position
        /// </summary>
        /// <param name="numRaycasts"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public RaycastSweep(int numRaycasts, Vector3 startPosition, Vector3 endPosition)
        {
            if (numRaycasts < 2) throw new ArgumentOutOfRangeException("numRaycasts", "RaycastSweep requires that there are at least 2 raycasts to work with.");

            Name = $"{nameof(RaycastSweep)}_{GetHashCode()}";

            NumRaycasts = numRaycasts;
            StartPosition = startPosition;
            EndPosition = endPosition;

            Vector3 positionDifference = endPosition - startPosition;
            float posDifferenceLength = positionDifference.Length();

            // posIncrement is the position difference between each raycast. 
            // We want to cover the entire raycasting range as defined by startPosition and endPosition,
            // so we'll need to set the position increment in a way that allows for the full raycasting range
            // to be covered and allows for the raycasts to be equally spaced from each other.
            Vector3 posIncrement = positionDifference / (numRaycasts - 1);
            Vector3 currentStartPos = startPosition;
            for (int i = 0; i < numRaycasts; i++)
            {
                // Create raycast for the current index
                RayCast3D r = new RayCast3D();
                r.Name = $"Raycast{i}";

                // For performance reasons, we don't want to update every physics frame by default.
                // This is because we really only need to update when PerformRaycast() is called.
                r.Enabled = false;

                r.Position = currentStartPos;
                r.TargetPosition = new Vector3(currentStartPos.X, currentStartPos.Y, currentStartPos.Z + RaycastLength);

                AddChild(r);
                Raycasts.Add(r);

                // Move onto the next raycast with an updated start position
                currentStartPos += posIncrement;
            }
        }

        /// <summary>
        /// Runs through the raycasts in <see cref="Raycasts"/> from left-to-right.
        /// This returns raycast collision information about the first raycast in <see cref="Raycasts"/>
        /// that got collided with. If no raycast in <see cref="Raycasts"/> was hit,
        /// this function returns null.
        /// </summary>
        /// <returns>The results of this raycast operation</returns>
        public RaycastSweepResult PerformRaycast()
        {
            for (int i = 0; i < Raycasts.Count; i++)
            {
                RayCast3D r = Raycasts[i];
                // Needed since the raycast collision information doesn't update every frame by default
                // (which is for performance reasons)
                r.ForceRaycastUpdate();

                if (r.IsColliding())
                {
                    return new RaycastSweepResult(r, r.GetCollisionPoint(), r.GetCollider(), i);
                }
            }

            return null;
        }

        /// <summary>
        /// Disposes of this <see cref="RaycastSweep"/>, including its raycasts.
        /// </summary>
        public new void Dispose()
        {
            QueueFree();

            foreach (RayCast3D r in Raycasts)
            {
                r.QueueFree();
                r.Dispose();
            }
            Raycasts.Clear();

            base.Dispose();
        }
    }
}
