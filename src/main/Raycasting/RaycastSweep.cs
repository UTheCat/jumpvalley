using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Raycasting
{
    /// <summary>
    /// Sweeps over a region of 3D space using a series of equally-spaced-out raycasts.
    /// </summary>
    public partial class RaycastSweep
    {
        /// <summary>
        /// The total number of raycasts that the raycast sweep will perform from the start position to the end position.
        /// </summary>
        public int NumRaycasts { get; private set; }

        /// <summary>
        /// The starting position of the raycast sweep. This is where the first raycast will begin.
        /// </summary>
        public Vector3 StartPosition { get; private set; }

        /// <summary>
        /// The ending position of the raycast sweep. This is where the last raycast will begin.
        /// </summary>
        public Vector3 EndPosition { get; private set; }

        public RaycastSweep()
        {

        }
    }
}
