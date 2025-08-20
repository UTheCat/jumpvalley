using Godot;
using System;

using UTheCat.Jumpvalley.Core.Players.Camera;

namespace UTheCat.Jumpvalley.App.Players.Camera
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

        /// <summary>
        /// Reference to a <see cref="Control"/> that's intended to absorb mouse input as a means of preventing unwanted mouse cursor
        /// hovering behavior from happening while the user is turning the camera. This <see cref="Control"/> should always cover the
        /// entire app window and be invisible, even when the <see cref="Control"/>'s Visible property is set to <c>true</c>.
        /// <br/><br/>
        /// To absorb the mouse input, this <see cref="Control"/> has to be in a position within the current <see cref="SceneTree"/>
        /// such that the <see cref="Control"/> has high input priority. Additionally, when this <see cref="UserInputCamera"/> instance
        /// gets instantiated, the <see cref="Control"/> should have its Visible property set to false.  
        /// </summary>
        public Control CameraTurnInvisibleOverlay = null;

        private float cameraPanningSpeedMultiplier = 0.02f;
        private Vector2I originalCursorPos = Vector2I.Zero;

        public UserInputCamera() : base() { }

        public override void _Input(InputEvent @event)
        {
            Control cameraTurnOverlay = CameraTurnInvisibleOverlay;
            if (cameraTurnOverlay != null && cameraTurnOverlay.Visible)
            {
                if (!Input.IsActionPressed(INPUT_CAMERA_PAN)) StopCameraTurn();

                if (IsTurningCamera && @event is InputEventMouseMotion mouseEvent) MouseTurnCamera(mouseEvent);
            }

            base._Input(@event);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            // Handle camera turning input
            if (Input.IsActionPressed(INPUT_CAMERA_PAN))
            {
                if (!IsTurningCamera)
                {
                    IsTurningCamera = true;

                    // MouseGetPosition() gets cursor position in screen coordinates.
                    // However, we want to store the mouse position relative to the position of the client area (the app window).
                    // This also needs to come before we set mouse mode to "Captured" so we can retrieve the mouse position
                    // before moving the cursor to the center of the window for camera turning.
                    originalCursorPos = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition();

                    Input.MouseMode = Input.MouseModeEnum.Captured;
                }
            }
            else
            {
                StopCameraTurn();
            }

            // Turn camera based on mouse input
            if (IsTurningCamera && @event is InputEventMouseMotion mouseEvent)
            {
                Control cameraTurnOverlay = CameraTurnInvisibleOverlay;

                // Should come before we set cameraTurnOverlay.Visible to true
                // as a means of preventing a race condition
                MouseTurnCamera(mouseEvent);

                if (cameraTurnOverlay != null) cameraTurnOverlay.Visible = true;
            }

            base._Input(@event);

            // Handle input for adjusting the camera's zoom
            if (Input.IsActionPressed(INPUT_CAMERA_ZOOM_IN))
            {
                ZoomOutDistance -= CameraZoomAdjustment;
            }
            else if (Input.IsActionPressed(INPUT_CAMERA_ZOOM_OUT))
            {
                ZoomOutDistance += CameraZoomAdjustment;
            }

            base._UnhandledInput(@event);
        }

        private void StopCameraTurn()
        {
            if (IsTurningCamera)
            {
                IsTurningCamera = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;
                DisplayServer.WarpMouse(originalCursorPos);

                Control cameraTurnOverlay = CameraTurnInvisibleOverlay;
                if (cameraTurnOverlay != null) cameraTurnOverlay.Visible = false;
            }
        }

        private void MouseTurnCamera(InputEventMouseMotion mouseEvent)
        {
            Vector2 mouseEventRelative = mouseEvent.Relative;

            float panningFactor = PanningSensitivity * PanningSpeed * cameraPanningSpeedMultiplier;
            Pitch += -mouseEventRelative.Y * panningFactor;
            Yaw += -mouseEventRelative.X * panningFactor;
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
