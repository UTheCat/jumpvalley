using Godot;

using Jumpvalley.Levels.Interactives;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Mechanics.Teleporters
{
    /// <summary>
    /// Interactive that prepares an object to be teleported by an
    /// instance of <see cref="Teleporter"/>.
    /// <br/><br/>
    /// This class primarily supplies some data to help
    /// the root node of a <see cref="TeleportableObject"/>
    /// be teleported to the correct point.
    /// </summary>
    public partial class TeleportableObject : InteractiveNode
    {
        /// <summary>
        /// Name of the metadata entry that stores a teleportable object's
        /// overall bounding box calculation in the metadata of a
        /// teleportable object's <see cref="RootNode"/>
        /// </summary>
        public static readonly string OVERALL_BOUNDING_BOX_META_NAME = "teleportable_object_overall_bounding_box";

        private Aabb _overallBoundingBox;

        /// <summary>
        /// The bounding box of all the <see cref="VisualInstance3D"/>s
        /// that are a descendant of this <see cref="TeleportableObject"/>'s
        /// root node.
        /// </summary>
        public Aabb OverallBoundingBox
        {
            get => _overallBoundingBox;
            set
            {
                _overallBoundingBox = value;
                RootNode?.SetMeta(OVERALL_BOUNDING_BOX_META_NAME, value);
            }
        }

        public TeleportableObject(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            UpdateOverallBoundingBox();
        }

        /// <summary>
        /// Returns the overall bounding box of a specified
        /// root node by merging the bounding boxes of its descendants
        /// that are <see cref="VisualInstance3D"/>s.
        /// <br/><br/>
        /// The root node itself gets merged in too if it's a
        /// <see cref="VisualInstance3D"/>. 
        /// </summary>
        /// <param name="rootNode">The root node to get the overall bounding box of</param>
        /// <returns>
        /// The overall bounding box
        /// </returns>
        public static Aabb GetOverallBoundingBox(Node rootNode)
        {
            Aabb total = new Aabb();

            if (rootNode == null)
            {
                return total;
            }

            void Add(Node node)
            {
                if (node is VisualInstance3D visual)
                {
                    total = total.Merge(visual.GetAabb());
                }

                foreach (Node n in node.GetChildren())
                {
                    Add(node);
                }
            }
            Add(rootNode);

            return total;
        }

        /// <summary>
        /// Updates the value of <see cref="OverallBoundingBox"/>,
        /// (re)calculating the overall bounding box of this <see cref="TeleportableObject"/>'s
        /// root node.
        /// </summary>
        public void UpdateOverallBoundingBox()
        {
            OverallBoundingBox = GetOverallBoundingBox(RootNode);
        }
    }
}
