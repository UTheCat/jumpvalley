using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Camera
{
    /// <summary>
    /// Handles control of the player's camera via a mouse
    /// </summary>
    public partial class MouseCamera: BaseCamera, IDisposable
    {
        private static readonly string INPUT_CAMERA_PAN = "camera_pan";

        private bool isTurningCamera = false;

        public MouseCamera() : base() { }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed(INPUT_CAMERA_PAN))
            {
                isTurningCamera = true;
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            else if (Input.IsActionJustReleased(INPUT_CAMERA_PAN))
            {
                isTurningCamera = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }

            // Right-click to turn the camera
            if (isTurningCamera && @event is InputEventMouseMotion mouseEvent)
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
            isTurningCamera = false;

            base.Dispose();
        }
    }
}
