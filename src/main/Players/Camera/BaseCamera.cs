using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Camera
{
    /// <summary>
    /// The base class that handles a 3d camera focusing on a 3d object. It includes support for camera panning and zoom controls.
    /// </summary>
    public partial class BaseCamera
    {
        /// <summary>
        /// How sensitive camera panning is to player input.
        /// </summary>
        public float PanningSensitivity = 0;

        /// <summary>
        /// "Default" camera panning speed in radians per second.
        /// This assumes that the camera is being turned at constant speed on hardware such as a joystick,
        /// or like using the left and right arrow keys to turn your camera in most Roblox games.
        /// <br/>
        /// This value gets scaled by <see cref="PanningSensitivity"/>.
        /// </summary>
        public float PanningSpeed = 0;


    }
}
