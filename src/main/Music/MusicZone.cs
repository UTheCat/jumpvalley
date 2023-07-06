using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single music zone
/// <br/>
/// Music zones each have their own playlist. Such playlists typically only contain one song, but it's possible for the playlists to have multiple songs.
/// </summary>
namespace Jumpvalley.Music
{
    public partial class MusicZone: MusicGroup
    {
        /// <summary>
        /// The name of the <see cref="Node3D"/>s that make up the geometry of the music zone
        /// </summary>
        public static readonly string MUSIC_ZONE_GEOMETRY_NAME = "ZonePart";

        /// <summary>
        /// The boxes that make up the geometry of the music zone.
        /// </summary>
        public List<Node3D> boxes = new List<Node3D>();

        public MusicZone(Node node) : base(node)
        {
            Godot.Collections.Array<Node> nodes = GetChildren();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Node3D box && box.Name.Equals(MUSIC_ZONE_GEOMETRY_NAME))
                {
                    boxes.Add(box);
                }
            }
        }

        /// <summary>
        /// Based on the size, rotation, and position of each box that makes up the music zone, this method returns whether or not the given
        /// point is within the music zone.
        /// </summary>
        /// <param name="point">The point (in global space coordinates)</param>
        /// <returns>Whether or not the point is in the music zone</returns>
        public bool IsPointInZone(Vector3 point)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                Node3D box = boxes[i];
                Vector3 boxSize = box.Scale;

                // Convert the given point to object space
                point = box.ToLocal(point);

                // Check if the point is inside the box
                if (Math.Abs(point.X) <= boxSize.X / 2
                    && Math.Abs(point.Y) <= boxSize.Y / 2
                    && Math.Abs(point.Z) <= boxSize.Z / 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
