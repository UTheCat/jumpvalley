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

                Pitch += mouseEventRelative.X;
                Yaw += mouseEventRelative.Y;
            }

            base._Input(@event);
        }
    }
}
