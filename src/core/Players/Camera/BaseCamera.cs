using Godot;
using System;

namespace UTheCat.Jumpvalley.Core.Players.Camera
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
            set
            {
                value = Math.Clamp(value, MinZoomOutDistance, MaxZoomOutDistance);
                if (_zoomOutDistance != value)
                {
                    _zoomOutDistance = value;
                    RaiseZoomOutDistanceChanged(value);
                }
            }
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

                Vector3 camPos = FocusedNode.GlobalPosition;

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
                //camPos += new Vector3(RightOffset, 0, ZoomOutDistance).Rotated(Vector3.Up, camRot.Y).Rotated(Vector3.Right, camRot.X);
                //camPos += new Vector3(RightOffset, 0, ZoomOutDistance).Rotated(Vector3.Right, camRot.X);
                //camPos = camPos.Rotated(Vector3.Right, camRot.X);

                // quick little patch I suppose
                //camPos.Z -= ZoomOutDistance;

                // Use transforms to achieve the above
                // See this article for an explaination on why:
                // https://docs.godotengine.org/en/stable/tutorials/3d/using_transforms.html
                //Transform3D transform = new Transform3D();
                //transform.Basis = Basis.Identity;
                //transform.Origin = camPos;
                //transform.Basis = transform.Basis.Rotated(Vector3.Up, camRot.Y);
                //transform = transform.Orthonormalized();

                // We were rotating the transform around the x-axis, causing the yaw gyro to be tilted.
                // Lets result to trignometry with 2d right triangles to get what we want instead.
                //transform.Basis = transform.Basis.Rotated(Vector3.Right, camRot.X);
                //transform = transform.Orthonormalized();

                //transform = transform.Rotated(Vector3.Up, camRot.Y);
                //transform = transform.Rotated(Vector3.Right, camRot.X);
                //transform = transform.Translated(new Vector3(RightOffset, 0, ZoomOutDistance));

                /*
                Node3D node3D = new Node3D();
                Transform3D transform = node3D.Transform;
                node3D.GlobalPosition = camPos;
                node3D.Rotation = camRot;
                node3D.TranslateObjectLocal(new Vector3(RightOffset, 0, ZoomOutDistance));
                */

                // Use the transform to transform the camera offset, then return the final camera position
                //Vector3 transformedRightOffset = transform.Basis.X * RightOffset;

                //
                //Vector3 transformedZoomOffset = transform.Basis.Z * ZoomOutDistance;

                // Refer to camera_pos_calculation.png for details
                float zoomOutDistance = ZoomOutDistance;

                // Negated pitch angle must be used to calculate the next two variables.
                // This is because we're applying a positional offset to the first person camera when calculating the camera's position for third-person.
                // Also keep in mind: the "Pitch" variable refers to the pitch of the camera when the camera is in first-person
                float pitchAngle = -camRot.X;

                float transformHorizontalDistance = zoomOutDistance * (float)Math.Cos((double)pitchAngle);
                float transformHeight = zoomOutDistance * (float)Math.Sin((double)pitchAngle);

                //Vector3 camOffset = transformedRightOffset + transformedZoomOffset;//new Vector3(RightOffset, 0, ZoomOutDistance);

                // Essentially, the base of the aforementioned 2D right triangle lies on the Z-axis of the transform
                //Vector3 camOffset = transformedRightOffset + (transformHorizontalDistance * transform.Basis.Z) + (transformHeight * transform.Basis.Y);
                Vector3 camOffset = (new Vector3(RightOffset, transformHeight, transformHorizontalDistance)).Rotated(Vector3.Up, camRot.Y);
                //camPos += new Vector3(RightOffset, 0, ZoomOutDistance) * transform;
                camPos += camOffset;

                return camPos;
                //return transform.Origin;
                //return transform.Origin;

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

        /// <summary>
        /// Updates the position and rotation of the currently associated Camera
        /// </summary>
        public void Update()
        {
            Camera3D camera = Camera;
            Node3D focusedNode = FocusedNode;

            if (camera != null && focusedNode != null)
            {
                // use transforms for rotation for the reasons described in this article:
                // https://docs.godotengine.org/en/stable/tutorials/3d/using_transforms.html
                //Transform3D cTransform = camera.Transform;
                //cTransform.Basis = new Basis();
                //camera.Orthonormalize();

                // Order matters: It has to be rotation around the Y-axis, then rotation around the X-axis
                Vector3 camRot = GetRotation();
                camera.Rotation = camRot;
                //camera.Basis = new Basis();
                //camera.RotateObjectLocal(Vector3.Up, camRot.Y);
                //camera.RotateObjectLocal(Vector3.Right, camRot.X);

                // Then set the position of the camera
                camera.Position = GetPosition();
            }
        }

        /// <summary>
        /// Disposes of this <see cref="BaseCamera"/> object
        /// </summary>
        public new void Dispose()
        {
            QueueFree();
            base.Dispose();
        }

        public override void _Process(double delta)
        {
            Update();
            base._Process(delta);
        }

        /// <summary>
        /// Event that's raised when the value of zoom out distance changes.
        /// <br/>
        /// The float argument is the new value of <see cref="ZoomOutDistance"/>
        /// </summary>
        public event EventHandler<float> ZoomOutDistanceChanged;

        protected void RaiseZoomOutDistanceChanged(float zoomOutDistance)
        {
            ZoomOutDistanceChanged?.Invoke(this, zoomOutDistance);
        }
    }
}
