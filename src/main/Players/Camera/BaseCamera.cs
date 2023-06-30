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
        /// <br/>
        /// This value multiplies <see cref="PanningSpeed"/> when camera rotations aren't done at a constant speed, such as when rotation is controlled by the mouse.
        /// </summary>
        public float PanningSensitivity = 0;

        /// <summary>
        /// "Default" camera panning speed in radians per second.
        /// <br/>
        /// This value is used when the camera needs to be turned at a constant speed, and it serves as a factor for calculating the speed
        /// of the camera rotation every input/process frame when the speed isn't constant.
        /// </summary>
        public float PanningSpeed = 0;


    }
}
