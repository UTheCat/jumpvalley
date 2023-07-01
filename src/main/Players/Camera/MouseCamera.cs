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
        //private Vector2 lastMousePos = Vector2.Zero;

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
                isTurningCamera = true;
                //Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            else if (Input.IsActionJustReleased(INPUT_CAMERA_PAN))
            {
                isTurningCamera = false;
                //Input.MouseMode = Input.MouseModeEnum.Visible;
            }

            // Right-click to turn the camera
            if (isTurningCamera && @event is InputEventMouseMotion mouseEvent)
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
            isTurningCamera = false;

            base.Dispose();
        }
    }
}
