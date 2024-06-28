using Godot;

using Jumpvalley.Levels.Interactives;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Mechanics
{
    /// <summary>
    /// Interactive that calculates its root node's
    /// overall bounding box.
    /// <br/><br/>
    /// Calculation is done via the <see cref="GetOverallBoundingBox"/>
    /// method.
    /// </summary>
    public partial class OverallBoundingBoxObject : InteractiveNode
    {
        /// <summary>
        /// Name of the metadata entry that stores an object's
        /// overall bounding box calculation in the object's metadata
        /// </summary>
        public static readonly string OVERALL_BOUNDING_BOX_META_NAME = "overall_bounding_box";

        /// <summary>
        /// Name of the metadata entry that stores an object's custom overall bounding box.
        /// This metadata entry stores an <see cref="Aabb"/> that should be user defined.
        /// </summary>
        public static readonly string CUSTOM_OVERALL_BOUNDING_BOX_META_NAME = "custom_overall_bounding_box";

        private Aabb _overallBoundingBox;

        /// <summary>
        /// The bounding box of all the <see cref="VisualInstance3D"/>s
        /// that are a descendant of this <see cref="OverallBoundingBoxObject"/>'s
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

        public OverallBoundingBoxObject(OffsetStopwatch stopwatch, Node marker) : base(stopwatch, marker)
        {
            UpdateOverallBoundingBox();
        }

        /// <summary>
        /// Returns the overall bounding box of a specified
        /// root node by merging its bounding box and that of its descendants.
        /// <br/><br/>
        /// This method obtains a node's bounding box either by getting the value of
        /// its <c>custom_overall_bounding_box</c> metadata entry (if it exists),
        /// or by obtaining its <see cref="Aabb"/> automatically (if it's a
        /// <see cref="VisualInstance3D"/> since they have the
        /// <see cref="VisualInstance3D.GetAabb"/> method). 
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
                if (node.HasMeta(CUSTOM_OVERALL_BOUNDING_BOX_META_NAME))
                {
                    total = total.Merge(
                        node.GetMeta(CUSTOM_OVERALL_BOUNDING_BOX_META_NAME).As<Aabb>()
                        );
                }
                else if (node is VisualInstance3D visual)
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
        /// (re)calculating the overall bounding box of this <see cref="OverallBoundingBoxObject"/>'s
        /// root node.
        /// </summary>
        public void UpdateOverallBoundingBox()
        {
            OverallBoundingBox = GetOverallBoundingBox(RootNode);
        }
    }
}
