using Godot;
using System;

using Jumpvalley.Players.Camera;

namespace JumpvalleyApp.Players.Camera
{
    /// <summary>
    /// <see cref="BaseCamera"/> that rotates, zooms in, and zooms out
    /// when appropriate to do so in response to user input.
    /// </summary>
    public partial class UserInputCamera : BaseCamera, IDisposable
    {
        private static readonly string INPUT_CAMERA_PAN = "camera_pan";
        private static readonly string INPUT_CAMERA_ZOOM_IN = "camera_zoom_in";
        private static readonly string INPUT_CAMERA_ZOOM_OUT = "camera_zoom_out";

        private bool _isTurningCamera = false;

        public bool IsTurningCamera
        {
            get => _isTurningCamera;
            private set
            {
                bool valueChanged = value != _isTurningCamera;
                _isTurningCamera = value;

                if (valueChanged) RaiseIsTurningCameraToggled(value);
            }
        }

        /// <summary>
        /// The amount of meters that the zoom out distance changes for every frame that the camera is being zoomed in or out by user input.
        /// </summary>
        public float CameraZoomAdjustment = 1;

        public UserInputCamera() : base() { }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed(INPUT_CAMERA_PAN))
            {
                IsTurningCamera = true;
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            else if (Input.IsActionJustReleased(INPUT_CAMERA_PAN))
            {
                IsTurningCamera = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else if (Input.IsActionPressed(INPUT_CAMERA_ZOOM_IN))
            {
                ZoomOutDistance -= CameraZoomAdjustment;
            }
            else if (Input.IsActionPressed(INPUT_CAMERA_ZOOM_OUT))
            {
                ZoomOutDistance += CameraZoomAdjustment;
            }

            // Right-click to turn the camera
            if (IsTurningCamera && @event is InputEventMouseMotion mouseEvent)
            {
                Vector2 mouseEventRelative = mouseEvent.Relative;

                float panningFactor = PanningSensitivity * PanningSpeed * 0.02f;
                Pitch += -mouseEventRelative.Y * panningFactor;
                Yaw += -mouseEventRelative.X * panningFactor;
            }

            base._Input(@event);
        }

        public new void Dispose()
        {
            SetProcessInput(false);
            IsTurningCamera = false;

            base.Dispose();
        }

        /// <summary>
        /// Event raised when <see cref="IsTurningCamera"/> changes.
        /// The boolean event argument is the new value of <see cref="IsTurningCamera"/>.
        /// <br/><br/>
        /// Avoid accessing the <see cref="IsTurningCamera"/> property directly
        /// in an event handler method handling this event. This is because the
        /// <see cref="UserInputCamera"/> might've been disposed by the time
        /// the event handler method has been called.
        /// </summary>
        public event EventHandler<bool> IsTurningCameraToggled;

        protected void RaiseIsTurningCameraToggled(bool isTurning)
        {
            IsTurningCameraToggled?.Invoke(this, isTurning);
        }
    }
}
