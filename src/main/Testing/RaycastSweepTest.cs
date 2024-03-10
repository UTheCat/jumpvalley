using Godot;
using System;

using Jumpvalley.Raycasting;

namespace Jumpvalley.Testing
{
    /// <summary>
    /// Test class for making sure the <see cref="RaycastSweep"/> class works properly
    /// </summary>
    public partial class RaycastSweepTest: Node
    {
        public RaycastSweep TestRaycastSweep;
        public Label RaycastResultLabel;

        /// <summary>
        /// Creates a new instance of <see cref="RaycastSweepTest"/>
        /// </summary>
        /// <param name="raycastSweep">The raycast sweep to apply</param>
        /// <param name="node">The Node3D to use the raycast sweep for</param>
        public RaycastSweepTest(RaycastSweep raycastSweep, Node3D node)
        {
            Name = $"{nameof(RaycastSweepTest)}_{GetHashCode()}";

            RaycastResultLabel = new Label();
            RaycastResultLabel.Name = $"RaycastSweepResultLabel_{GetHashCode()}";

            TestRaycastSweep = raycastSweep;
            node?.AddChild(raycastSweep);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            RaycastSweepResult results = TestRaycastSweep.PerformRaycast();

            string resultText = $"RaycastSweep results for {TestRaycastSweep.Name}\n";
            
            if (results == null)
            {
                resultText += "Currently not colliding with anything";
            }
            else
            {
                resultText += $"Collided object: {results.CollidedObject}\nCollision point: {results.CollisionPoint}\nColliding raycast: {results.Raycast}\nColliding raycast index: {results.RaycastIndex}";
            }

            RaycastResultLabel.Text = resultText;
        }
    }
}
