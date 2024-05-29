using Jumpvalley.Gui;

namespace Jumpvalley.Animation
{
    /// <summary>
    /// Version of <see cref="AnimatedNodeGroup"/> that displays a background panel
    /// when at least one of the <see cref="AnimatedNode"/>s in the group are visible. 
    /// </summary>
    public partial class BgPanelAnimatedNodeGroup
    {
        /// <summary>
        /// The group's <see cref="BackgroundPanel"/> that will act as
        /// a background for the <see cref="AnimatedNode"/>s in the group. 
        /// </summary>
        public BackgroundPanel BgPanel { get; private set; }
    }
}
