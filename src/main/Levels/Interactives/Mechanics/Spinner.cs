using Godot;
using System;
using System.Diagnostics;

namespace Jumpvalley.Levels.Interactives.Mechanics
{
    /// <summary>
    /// A basic spinner. A spinner is a rotating platform.
    /// </summary>
    public partial class Spinner : InteractiveNode, IDisposable
    {
        private StaticBody3D body;
        private Vector3 _angularVelocity;

        /// <summary>
        /// The constant angular velocity of the spinner (the number of radians rotated per second for pitch, yaw, and roll).
        /// </summary>
        public Vector3 AngularVelocity
        {
            get => _angularVelocity;
            set
            {
                _angularVelocity = value;
                body.ConstantAngularVelocity = value;
            }
        }

        public Spinner(Stopwatch stopwatch, Node node) : base(stopwatch, node)
        {
            body = node as StaticBody3D;
            if (body == null) throw new ArgumentException("The node specified in the 2nd argument must be a StaticBody3D.");

            AngularVelocity = body.ConstantAngularVelocity;
            body.AddChild(this);
        }

        private void Update(float delta)
        {
            body.Rotation += new Vector3(
                AngularVelocity.X * delta,
                AngularVelocity.Y * delta,
                AngularVelocity.Z * delta
                );
        }

        public override void _PhysicsProcess(double delta)
        {
            Update((float)delta);
        }

        public new void Dispose()
        {
            SetPhysicsProcess(false);
            body.RemoveChild(this);
            QueueFree();
            base.Dispose();
        }
    }
}
