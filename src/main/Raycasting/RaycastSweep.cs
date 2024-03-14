using Godot;
using System;
using System.Collections.Generic;

namespace Jumpvalley.Raycasting
{
    /// <summary>
    /// Sweeps over a region of 3D space using a series of equally-spaced-out raycasts.
    /// <br/>
    /// <br/>
    /// The center raycast, being located in the middle (or close to it), is assigned to the greatest integer that's less than or equal to the median index.
    /// "index" here refers to the numerical index that a raycast is located in within a raycast sweep from left to right.
    /// </summary>
    public partial class RaycastSweep : Node3D, IDisposable
    {
        /// <summary>
        /// List of possible orders in which RaycastSweep can iterate over its raycasts when <see cref="PerformRaycast"/> is called
        /// </summary>
        public enum SweepOrder
        {
            /// <summary>
            /// Sweep from left to right
            /// </summary>
            LeftToRight,

            /// <summary>
            /// Sweep from right to left
            /// </summary>
            RightToLeft,

            /// <summary>
            /// Sweep starting from the center.
            /// Then go from left to right (excluding the center raycast)
            /// </summary>
            CenterLeftRight,

            /// <summary>
            /// Sweep starting from the center.
            /// Then go from right to left (excluding the center raycast)
            /// </summary>
            CenterRightLeft
        }

        /// <summary>
        /// The total number of raycasts that the raycast sweep will perform from the start position to the end position.
        /// </summary>
        public int NumRaycasts;

        /// <summary>
        /// The length in meters that each raycast in the <see cref="RaycastSweep"/> should extend in the relative Z-axis of this <see cref="Node3D"/>
        /// </summary>
        public float RaycastLength;

        /// <summary>
        /// The starting position of the raycast sweep.
        /// This position is relative to the position of this <see cref="Node3D"/> and is where the first raycast will begin.
        /// </summary>
        public Vector3 StartPosition;

        /// <summary>
        /// The ending position of the raycast sweep.
        /// This position is relative to the position of this <see cref="Node3D"/> and is where the last raycast will begin.
        /// </summary>
        public Vector3 EndPosition;

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
        public RaycastSweep(int numRaycasts, Vector3 startPosition, Vector3 endPosition, float raycastLength)
        {
            Name = $"{nameof(RaycastSweep)}_{GetHashCode()}";

            NumRaycasts = numRaycasts;
            StartPosition = startPosition;
            EndPosition = endPosition;
            RaycastLength = raycastLength;
            Raycasts = new List<RayCast3D>();

            UpdateRaycastLayout();
        }

        /// <summary>
        /// Updates the number of raycasts, the positioning of the raycasts, and the length of the raycasts based on the current values of
        /// <see cref="NumRaycasts"/>, <see cref="RaycastLength"/>, <see cref="StartPosition"/>, and <see cref="EndPosition"/>.
        /// </summary>
        public void UpdateRaycastLayout()
        {
            int numRaycasts = NumRaycasts;

            if (numRaycasts < 2) throw new ArgumentOutOfRangeException("numRaycasts", "RaycastSweep requires that there are at least 2 raycasts to work with.");

            float raycastLength = RaycastLength;
            Vector3 startPosition = StartPosition;
            Vector3 endPosition = EndPosition;

            Vector3 positionDifference = endPosition - startPosition;

            // posIncrement is the position difference between each raycast. 
            // We want to cover the entire raycasting range as defined by startPosition and endPosition,
            // so we'll need to set the position increment in a way that allows for the full raycasting range
            // to be covered and allows for the raycasts to be equally spaced from each other.
            Vector3 posIncrement = positionDifference / (numRaycasts - 1);
            Vector3 currentStartPos = startPosition;

            int oldNumRaycasts = Raycasts.Count;
            int numRaycastsDiff = Math.Abs(numRaycasts - oldNumRaycasts);

            // Remove from SceneTree while performing layout update, just to be safe
            foreach (RayCast3D r in Raycasts)
            {
                if (r.GetParent() == this)
                {
                    RemoveChild(r);
                }
            }

            // Account for difference in number of raycasts
            if (numRaycasts > oldNumRaycasts)
            {
                for (int i = 0; i < numRaycastsDiff; i++)
                {
                    // Create raycast for the current index
                    RayCast3D r = new RayCast3D();
                    r.Name = $"Raycast{Raycasts.Count}";

                    // For performance reasons, we don't want to update every physics frame by default.
                    // This is because we really only need to update when PerformRaycast() is called.
                    //r.Enabled = false;

                    // Using ForceRaycastUpdate() seems to be buggy, so we need to keep this for now.
                    r.Enabled = true;

                    Raycasts.Add(r);
                }
            }
            else if (numRaycasts < oldNumRaycasts)
            {
                for (int i = 0; i < numRaycastsDiff; i++)
                {
                    Raycasts.RemoveAt(Raycasts.Count - 1);
                }
            }

            // Reposition raycasts, and add them back as a child of this RaycastSweep
            foreach (RayCast3D r in Raycasts)
            {
                r.Position = currentStartPos;

                // Remember that target position is relative to the position of the *raycast itself*.
                r.TargetPosition = new Vector3(0, 0, raycastLength);

                AddChild(r);

                // Move onto the next raycast with an updated start position
                currentStartPos += posIncrement;
            }
        }

        /// <summary>
        /// Gets the current collision info for the specified raycast.
        /// </summary>
        /// <param name="r">The raycast to use</param>
        /// <param name="raycastIndex">The numeric index of the raycast in the <see cref="Raycasts"/> list it belongs to</param>
        /// <returns></returns>
        private static RaycastSweepResult GetCurrentRaycastCollisionInfo(RayCast3D r, int raycastIndex)
        {
            // Needed since the raycast collision information doesn't update every frame by default
            // (which is for performance reasons)
            // Currently not using this method since this seems to be buggy.
            //r.ForceRaycastUpdate();

            if (r.IsColliding())
            {
                return new RaycastSweepResult(r, r.GetCollisionPoint(), r.GetCollider(), raycastIndex);
            }

            return null;
        }

        /// <summary>
        /// Runs through the raycasts in <see cref="Raycasts"/> in the order specified.
        /// <br/>
        /// <br/>
        /// This returns raycast collision information about the first raycast in <see cref="Raycasts"/>
        /// that got collided with. If no raycast in <see cref="Raycasts"/> was hit,
        /// this method returns null.
        /// </summary>
        /// <param name="order">The order in which to run through the raycasts</param>
        /// <returns>The results of this raycast operation</returns>
        public RaycastSweepResult PerformRaycast(SweepOrder order)
        {
            int numRaycasts = Raycasts.Count;

            if (order == SweepOrder.CenterLeftRight || order == SweepOrder.CenterRightLeft)
            {
                // Remember, we're working with indexes
                int centerIndex = (numRaycasts - 1) / 2;
                RaycastSweepResult centerResult = GetCurrentRaycastCollisionInfo(Raycasts[centerIndex], centerIndex);

                // If the center raycast hit something, we won't need to check the rest.
                if (centerResult != null) return centerResult;

                if (order == SweepOrder.CenterLeftRight)
                {
                    for (int i = 0; i < numRaycasts; i++)
                    {
                        if (i != centerIndex)
                        {
                            RaycastSweepResult result = GetCurrentRaycastCollisionInfo(Raycasts[i], i);
                            if (result != null) return result;
                        }
                    }
                }
                else if (order == SweepOrder.CenterRightLeft)
                {
                    for (int i = numRaycasts - 1; i > -1; i--)
                    {
                        if (i != centerIndex)
                        {
                            RaycastSweepResult result = GetCurrentRaycastCollisionInfo(Raycasts[i], i);
                            if (result != null) return result;
                        }
                    }
                }
            }
            else
            {
                if (order == SweepOrder.LeftToRight)
                {
                    for (int i = 0; i < numRaycasts; i++)
                    {
                        RaycastSweepResult result = GetCurrentRaycastCollisionInfo(Raycasts[i], i);
                        if (result != null) return result;
                    }
                }
                else if (order == SweepOrder.RightToLeft)
                {
                    for (int i = numRaycasts - 1; i > -1; i--)
                    {
                        RaycastSweepResult result = GetCurrentRaycastCollisionInfo(Raycasts[i], i);
                        if (result != null) return result;
                    }
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
