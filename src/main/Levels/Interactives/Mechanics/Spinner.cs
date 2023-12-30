using Godot;
using System;
using System.Diagnostics;

namespace Jumpvalley.Levels.Interactives.Mechanics
{
    /// <summary>
    /// A basic spinner. A spinner is a rotating platform.
    /// </summary>
    public partial class Spinner : InteractiveNode
    {
        /// <summary>
        /// The angular velocity of the spinner (the number of radians rotated per second for pitch, yaw, and roll).
        /// </summary>
        public Vector3 AngularVelocity;

        public Spinner(Stopwatch stopwatch, Node node) : base(stopwatch, node)
        {
            if ((node is StaticBody3D) == false) throw new ArgumentException("The node specified in the 2nd argument must be a StaticBody3D.");
        }
    }
}
