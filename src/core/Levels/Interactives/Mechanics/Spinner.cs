using Godot;
using System;

using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives.Mechanics
{
    /// <summary>
    /// A basic spinner. A spinner is a rotating platform.
    /// The root node of this interactive should be a StaticBody3D.
    /// <br/>
    /// <br/>
    /// Make sure to specify its constant angular velocity in the
    /// <c>constant_angular_velocity</c> or <c>constant_angular_velocity_degrees</c> metadata entries
    /// instead of the StaticBody3D's built in ConstantAngularVelocity property.
    /// This is so the spinner doesn't move objects when it isn't running.
    /// The spinner won't turn if you don't specify the angular velocity in one of the metadata entries.
    /// </summary>
    public partial class Spinner : InteractiveNode, IDisposable
    {
        /// <summary>
        /// The name of the metadata entry that defines the spinner's constant angular velocity in radians per second.
        /// This metadata entry should be placed in the spinner's interactive node marker.
        /// </summary>
        public readonly string CONSTANT_ANGULAR_VELOCITY_METADATA_NAME = "constant_angular_velocity";

        /// <summary>
        /// The name of the metadata entry that defines the spinner's constant angular velocity in degrees.
        /// This metadata entry only exists to allow level developers to define the spinner's constant angular velocity in degrees per second
        /// in the Godot editor.
        /// The value of this metadata entry will only be applied to the Spinner when its code instance is first initialized in the constructor.
        /// <br/>
        /// <br/>
        /// This metadata entry is optional. If specified, the constant_angular_velocity metadata entry will be set to the value of this metadata entry,
        /// but in radians per second instead of degrees per second.
        /// </summary>
        public readonly string CONSTANT_ANGULAR_VELOCITY_DEGREES_METADATA_NAME = "constant_angular_velocity_degrees";

        private StaticBody3D body;

        /// <summary>
        /// Whatever the original value of <see cref="body.ConstantAngularVelocity"/> was, before the instance of this class changed it
        /// when the spinner was started via <see cref="Start"/>
        /// </summary>
        private Vector3 originalConstantAngularVelocity;

        private Vector3 _constantAngularVelocity;

        /// <summary>
        /// The constant angular velocity of the spinner, or how fast it's spinning.
        /// This is the rate at which the spinner's yaw, pitch, and roll are changing in radians per second.
        /// </summary>
        public Vector3 ConstantAngularVelocity
        {
            get => _constantAngularVelocity;
            set
            {
                _constantAngularVelocity = value;
                SetMarkerMeta(CONSTANT_ANGULAR_VELOCITY_METADATA_NAME, value);
            }
        }

        public Spinner(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            body = RootNode as StaticBody3D;
            if (body == null) throw new ArgumentException("The node specified in the 2nd argument must be a StaticBody3D.");

            Vector3 angularVelocityDeg;
            if (TryGetMarkerMeta<Vector3>(
                CONSTANT_ANGULAR_VELOCITY_DEGREES_METADATA_NAME,
                out angularVelocityDeg))
            {
                Vector3 angularVelocityRad = Vector3.Zero;
                angularVelocityRad.X = Mathf.DegToRad(angularVelocityDeg.X);
                angularVelocityRad.Y = Mathf.DegToRad(angularVelocityDeg.Y);
                angularVelocityRad.Z = Mathf.DegToRad(angularVelocityDeg.Z);

                SetMarkerMeta(CONSTANT_ANGULAR_VELOCITY_METADATA_NAME, angularVelocityRad);
            }

            Vector3 angularVelocity;
            if (TryGetMarkerMeta<Vector3>(CONSTANT_ANGULAR_VELOCITY_METADATA_NAME, out angularVelocity))
            {
                ConstantAngularVelocity = angularVelocity;
            }
            else
            {
                ConstantAngularVelocity = Vector3.Zero;
            }
        }

        private void Update(float delta)
        {
            Vector3 angularVelocity = ConstantAngularVelocity;
            body.Rotation += new Vector3(
                angularVelocity.X * delta,
                angularVelocity.Y * delta,
                angularVelocity.Z * delta
                );
        }

        public override void Start()
        {
            if (IsRunning) return;

            base.Start();

            // Store the original constant angular velocity of the spinner's StaticBody3D
            // as well need to set it back to this velocity when the spinner is "stopped"
            // (or not running due to a call to Stop())
            originalConstantAngularVelocity = body.ConstantAngularVelocity;

            body.ConstantAngularVelocity = ConstantAngularVelocity;
        }

        public override void Stop()
        {
            if (!IsRunning) return;

            base.Stop();
            body.ConstantAngularVelocity = originalConstantAngularVelocity;
        }

        public override void _PhysicsProcess(double delta)
        {
            Update((float)delta);
        }

        public new void Dispose()
        {
            SetPhysicsProcess(false);
            Stop();
            QueueFree();

            base.Dispose();
        }
    }
}
