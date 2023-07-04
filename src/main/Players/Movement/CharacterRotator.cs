using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// This class is responsible for turning the character's yaw angle in the direction that it's moving.
    /// <br/>
    /// Such direction is approached gradually, unless it's specified that the direction should be set instantly.
    /// </summary>
    public partial class CharacterRotator: Node
    {
        /// <summary>
        /// The Node3D that this instance of <see cref="CharacterRotator"/> is rotating.
        /// </summary>
        public Node3D Character { get; private set; }

        /// <summary>
        /// The yaw angle of the character that's being approached.
        /// <br/>
        /// The character's yaw should eventually be the value of this variable, assuming that the yaw doesn't change by then and that this <see cref="CharacterRotator"/> is still active.
        /// </summary>
        public float Yaw = 0f;

        /// <summary>
        /// Creates a new instance of <see cref="CharacterRotator"/>
        /// </summary>
        /// <param name="character">The Node3D (representing the character) to rotate</param>
        public CharacterRotator(Node3D character)
        {
            Character = character;
        }

        public override void _Process(double delta)
        {
            Node3D character = Character;
            if (character != null)
            {
                Vector3 rotation = character.Rotation;

                // Reminder that in Godot, the "Y" component of a rotation "Vector3" corresponds to yaw angle
                // (also remember that angles aren't actually vectors)
                float yaw = Yaw;
                float rotY = rotation.Y;
                if (yaw != rotY)
                {
                    rotation.Y = Mathf.MoveToward(rotY, yaw, (float)delta);
                }
            }

            base._Process(delta);
        }
    }
}
