using Godot;
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
    public partial class BaseCamera: Node, IDisposable
    {
        /// <summary>
        /// How sensitive camera panning is to player input.
        /// <br/>
        /// This value multiplies <see cref="PanningSpeed"/> when camera rotations are done through user input, such as when rotation is controlled by the mouse.
        /// </summary>
        public float PanningSensitivity = 0;

        /// <summary>
        /// "Default" camera panning speed in radians per second.
        /// <br/>
        /// This value is used when the camera needs to be turned at a constant speed. It also serves as a factor for calculating the speed
        /// of the camera rotation every input/process frame when the camera rotation is done in response to user input.
        /// </summary>
        public float PanningSpeed = 0;

        /// <summary>
        /// The yaw angle of the camera
        /// </summary>
        public float Yaw = 0;

        private float _pitch;

        /// <summary>
        /// The pitch angle of the camera
        /// </summary>
        public float Pitch
        {
            get => _pitch;
            set => _pitch = Math.Clamp(value, MinPitch, MaxPitch);
        }

        /// <summary>
        /// The roll angle of the camera
        /// </summary>
        public float Roll = 0;

        /// <summary>
        /// The smallest pitch angle in radians that the camera can rotate in.
        /// </summary>
        public float MinPitch = (float)(-0.5 * Math.PI);

        /// <summary>
        /// The largest pitch angle in radians that the camera can rotate in.
        /// </summary>
        public float MaxPitch = (float)(0.5 * Math.PI);

        /// <summary>
        /// The offset of the camera's position to the right from the object that the camera is focusing on
        /// </summary>
        public float RightOffset = 0;

        private float _zoomOutDistance;

        /// <summary>
        /// How far zoomed out (in meters) that the camera is from the object that the camera is focusing on
        /// <br/>
        /// A value of 0 means that the camera is in first person while a value greater than 0 means that the camera is in third person.
        /// </summary>
        public float ZoomOutDistance
        {
            get => _zoomOutDistance;
            set => Math.Clamp(value, MinZoomOutDistance, MaxZoomOutDistance);
        }

        /// <summary>
        /// The minimum value of <see cref="ZoomOutDistance"/> in meters
        /// </summary>
        public float MinZoomOutDistance = 0;

        /// <summary>
        /// The maximum value of <see cref="ZoomOutDistance"/> in meters
        /// </summary>
        public float MaxZoomOutDistance = 0;

        /// <summary>
        /// The actual camera node that this <see cref="BaseCamera"/> is associated with.
        /// </summary>
        public Camera3D Camera = null;

        /// <summary>
        /// The actual 3d node that the camera is focusing on
        /// </summary>
        public Node3D FocusedNode = null;

        public BaseCamera()
        {
            Pitch = 0;
            ZoomOutDistance = MinZoomOutDistance;
        }

        /// <summary>
        /// Creates and returns a Vector3 with "x" representing the camera's pitch, "y" representing the camera's yaw, and "z" representing the camera's roll.
        /// </summary>
        /// <returns>The created Vector3</returns>
        public Vector3 GetRotation()
        {
            return new Vector3(Pitch, Yaw, Roll);
        }

        /// <summary>
        /// Calculates the camera position based on the position of the FocusedNode and the value of <see cref="RightOffset"/>
        /// </summary>
        /// <returns>The calculated camera position</returns>
        public Vector3 GetPosition()
        {
            if (FocusedNode != null)
            {
                
                //Node3D node3D = new Node3D();
                //Transform3D transform = node3D.Transform;

                Vector3 camPos = FocusedNode.Position;

                // In first person, the position of the camera is always the same as the
                // position of the object that the camera is focused on.
                if (ZoomOutDistance == 0)
                {
                    return camPos;
                }

                // However, if we're working with third person, we'll need to work in object space.
                // Let's get the rotation.
                Vector3 camRot = GetRotation();

                // First, rotate the horizontal and camera zooming offsets around the Y-axis (order matters; see the link above)
                // Then, rotate them around the X-axis.
                // Finally, apply the offsets.
                // (Remember that +Z means forward, therefore -Z means backward)
                camPos += new Vector3(RightOffset, 0, -ZoomOutDistance).Rotated(Vector3.Up, camRot.Y).Rotated(Vector3.Right, camRot.X);

                return camPos;

                /*
                if (RightOffset == 0)
                {
                    return camPos;
                }
                */

                // Based on the current yaw of the camera, rotate the Vector3 corresponding to the rightward offset
                // in order to calculate the rightward offset for different yaw values
                //return camPos += new Vector3(RightOffset, 0, 0).Rotated(Vector3.Up, camPos.Y);
            }

            return Vector3.Zero;
        }

        public override void _Process(double delta)
        {
            Camera3D camera = Camera;
            Node3D focusedNode = FocusedNode;

            if (camera != null && focusedNode != null)
            {
                // use transforms for rotation for the reasons described in this article:
                // https://docs.godotengine.org/en/stable/tutorials/3d/using_transforms.html
                Transform3D cTransform = camera.Transform;
                cTransform.Basis = new Basis();
                //camera.Orthonormalize();

                // Order matters: It has to be rotation around the Y-axis, then rotation around the X-axis
                Vector3 camRot = GetRotation();
                camera.RotateObjectLocal(Vector3.Up, camRot.Y);
                camera.RotateObjectLocal(Vector3.Right, camRot.X);

                // Then set the position of the camera
                camera.Position = GetPosition();
            }

            base._Process(delta);
        }

        /// <summary>
        /// Disposes of this <see cref="BaseCamera"/> object
        /// </summary>
        public new void Dispose()
        {
            QueueFree();
            base.Dispose();
        }
    }
}
