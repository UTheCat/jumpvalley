using Godot;
using System;

using Jumpvalley.Players.Camera;

namespace JumpvalleyApp.Players.Camera
{
    /// <summary>
    /// Handles control of the player's camera via a mouse
    /// </summary>
    public partial class MouseCamera : BaseCamera, IDisposable
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
            }
        }

        /// <summary>
        /// The amount of meters that the zoom out distance changes for every frame that the camera is being zoomed in or out by user input.
        /// </summary>
        public float CameraZoomAdjustment = 1;

        public MouseCamera() : base() { }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed(INPUT_CAMERA_PAN))
            {
                /*
                if (!isTurningCamera)
                {
                    isTurningCamera = true;

                    if (@event is InputEventMouse mouseEventInitial)
                    {
                        lastMousePos = mouseEventInitial.Position;
                    }
                }
                */
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

                // Later, keep the cursor in the same position while the camera is being turned
                // The approach here doesn't work well, and it may be better off programming a "software" mouse cursor.
                // See more details here:
                // https://docs.godotengine.org/en/stable/tutorials/inputs/custom_mouse_cursor.html
                //Vector2I mousePos = DisplayServer.MouseGetPosition();
                //Input.WarpMouse(lastMousePos);
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
        /// <see cref="MouseCamera"/> might've been disposed by the time
        /// the event handler method has been called.
        /// </summary>
        public event EventHandler<bool> IsTurningCameraToggled;
    }
}
