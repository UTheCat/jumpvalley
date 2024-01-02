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

        public Spinner(Stopwatch stopwatch, Node node) : base(stopwatch, node)
        {
            body = node as StaticBody3D;
            if (body == null) throw new ArgumentException("The node specified in the 2nd argument must be a StaticBody3D.");
        }

        private void Update(float delta)
        {
            Vector3 angularVelocity = body.ConstantAngularVelocity;
            body.Rotation += new Vector3(
                angularVelocity.X * delta,
                angularVelocity.Y * delta,
                angularVelocity.Z * delta
                );
        }

        public override void Start()
        {
            if (IsRunning) return;

            body.AddChild(this);
            base.Start();
        }

        public override void Stop()
        {
            if (!IsRunning) return;

            body.RemoveChild(this);
            base.Stop();
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
