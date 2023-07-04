using Godot;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This class is responsible for turning a 3D body's yaw angle in the direction that it's moving.
    /// <br/>
    /// Such direction is approached gradually, unless it's specified that the direction should be set instantly.
    /// </summary>
    public partial class BodyRotator
    {
        /// <summary>
        /// The Node3D that this instance of <see cref="BodyRotator"/> is rotating.
        /// </summary>
        public Node3D Body { get; private set; }

        /// <summary>
        /// The yaw angle of the body that's being approached.
        /// <br/>
        /// The body's yaw should eventually be the value of this variable, assuming that the yaw doesn't change by then and that this <see cref="BodyRotator"/> is still active.
        /// </summary>
        public float Yaw = 0f;

        /// <summary>
        /// Whether or not the yaw of <see cref="Body"/> is instantly set to <see cref="Yaw"/> on rotation update.
        /// </summary>
        public bool TurnsInstantly = false;

        /// <summary>
        /// Creates a new instance of <see cref="BodyRotator"/>
        /// </summary>
        /// <param name="body">The Node3D (representing the body) to rotate</param>
        public BodyRotator(Node3D body)
        {
            Body = body;
        }

        /// <summary>
        /// Method to be called every frame-process step in the scene tree in order to update body rotation.
        /// </summary>
        /// <param name="delta"></param>
        public void Update(double delta)
        {
            Node3D body = Body;
            if (body != null)
            {
                Vector3 rotation = body.Rotation;

                // Reminder that in Godot, the "Y" component of a rotation "Vector3" corresponds to yaw angle
                // (also remember that angles aren't actually vectors)
                float yaw = Yaw;
                float rotY = rotation.Y;
                if (yaw != rotY)
                {
                    if (TurnsInstantly)
                    {
                        rotation.Y = yaw;
                    }
                    else
                    {
                        rotation.Y = Mathf.MoveToward(rotY, yaw, (float)delta);
                    }
                }
            }
        }
    }
}
