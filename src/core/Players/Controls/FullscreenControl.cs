using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Controls
{
    /// <summary>
    /// Allows the user to toggle fullscreen on and off
    /// </summary>
    public partial class FullscreenControl : Node
    {
        public static readonly string INPUT_MAP_TOGGLE_FULLSCREEN = "fullscreen_toggle";

        public bool _fullscreenEnabled = false;

        /// <summary>
        /// Whether or not fullscreen is enabled
        /// </summary>
        public bool FullscreenEnabled
        {
            get => _fullscreenEnabled;
            set
            {
                _fullscreenEnabled = value;

                if (value)
                {
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                }
                else
                {
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="FullscreenControl"/>
        /// </summary>
        /// <param name="enabled">Whether or not fullscreen is initially enabled</param>
        public FullscreenControl(bool enabled)
        {
            FullscreenEnabled = enabled;
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed(INPUT_MAP_TOGGLE_FULLSCREEN))
            {
                FullscreenEnabled = !FullscreenEnabled;
            }

            base._Input(@event);
        }
    }
}
