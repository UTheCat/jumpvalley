using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a single music zone.
    /// <br/>
    /// Music zones each have their own playlist. Such playlists typically only contain one song, but it's possible for the playlists to have multiple songs.
    /// <br/>
    /// To specify the geometry that makes up the music zone, the music zone has to have a node named "ZoneParts" as a direct child.
    /// This "ZoneParts" node should contain MeshInstance3Ds using box meshes (preferred for performance reasons) or CSGBox3Ds.
    /// These boxes make up the music zone's geometry.
    /// </summary>
    public partial class MusicZone: MusicGroup
    {
        /// <summary>
        /// The name of the <see cref="Node"/> that contains the 3D boxes that make up the geometry of the music zone.
        /// Such boxes should be direct children of the node, and they can either be MeshInstance3Ds (preferred for performance reasons) or CSGBox3Ds.
        /// </summary>
        public static readonly string MUSIC_ZONE_GEOMETRY_CONTAINER_NAME = "ZoneParts";

        /// <summary>
        /// The MeshInstance3D boxes that make up the geometry of the music zone.
        /// </summary>
        public List<MeshInstance3D> MeshBoxes = new List<MeshInstance3D>();

        /// <summary>
        /// The CsgBox3Ds that make up the geometry of the music zone.
        /// </summary>
        public List<CsgBox3D> CsgBoxes = new List<CsgBox3D>();

        public MusicZone(Node node) : base(node)
        {
            Godot.Collections.Array<Node> childNodes = node.GetNode(MUSIC_ZONE_GEOMETRY_CONTAINER_NAME).GetChildren();

            foreach (Node n in childNodes)
            {
                if (n is MeshInstance3D mesh)
                {
                    MeshBoxes.Add(mesh);
                }
                else if (n is CsgBox3D csgBox)
                {
                    CsgBoxes.Add(csgBox);
                }
            }
        }

        /// <summary>
        /// Returns whether or not the given point <paramref name="localPoint"/> is within the given box's size <paramref name="boxSize"/>.
        /// </summary>
        /// <returns>Whether or not the given point <paramref name="localPoint"/> is within the given box's size <paramref name="boxSize"/>.</returns>
        public static bool IsLocalSpacePointInBox(Vector3 localPoint, Vector3 boxSize)
        {
            return Math.Abs(localPoint.X) <= boxSize.X / 2
                    && Math.Abs(localPoint.Y) <= boxSize.Y / 2
                    && Math.Abs(localPoint.Z) <= boxSize.Z / 2;
        }

        /// <summary>
        /// Based on the size, rotation, and position of each box that makes up the music zone, this method returns whether or not the given
        /// point is within the music zone.
        /// </summary>
        /// <param name="point">The point (in global space coordinates)</param>
        /// <returns>Whether or not the point is in the music zone</returns>
        public bool IsPointInZone(Vector3 point)
        {
            // First check whether or not we're inside one of the MeshInstance3Ds
            foreach (MeshInstance3D mesh in MeshBoxes)
            {
                if (mesh.Mesh is BoxMesh box)
                {
                    // Convert the given point to object space
                    point = mesh.ToLocal(point);

                    // Check if the point is inside the box
                    if (IsLocalSpacePointInBox(point, box.Size))
                    {
                        return true;
                    }
                }
            }

            // Then check whether or not we're inside one of the CsgBox3Ds
            foreach (CsgBox3D box in CsgBoxes)
            {
                // Convert the given point to object space
                point = box.ToLocal(point);

                // Check if the point is inside the box
                if (IsLocalSpacePointInBox(point, box.Size))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
