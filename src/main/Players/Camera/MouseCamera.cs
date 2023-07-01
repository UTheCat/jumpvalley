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
    public partial class MouseCamera: BaseCamera
    {
        public MouseCamera() : base() { }

        public override void _Input(InputEvent @event)
        {
            // Right-click to turn the camera
            if (@event is InputEventMouseMotion mouseEvent && mouseEvent.ButtonMask == MouseButtonMask.Right)
            {
                Vector2 mouseEventRelative = mouseEvent.Relative;

                float panningFactor = PanningSensitivity * PanningSpeed * 0.02f;
                Pitch += -mouseEventRelative.Y * panningFactor;
                Yaw += -mouseEventRelative.X * panningFactor;
            }

            base._Input(@event);
        }
    }
}
