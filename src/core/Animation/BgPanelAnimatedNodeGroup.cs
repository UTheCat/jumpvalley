using System;
using Jumpvalley.Gui;

namespace Jumpvalley.Animation
{
    /// <summary>
    /// Version of <see cref="AnimatedNodeGroup"/> that displays a background panel
    /// when at least one of the <see cref="AnimatedNode"/>s in the group are visible. 
    /// </summary>
    public partial class BgPanelAnimatedNodeGroup : AnimatedNodeGroup, IDisposable
    {
        /// <summary>
        /// The group's <see cref="BackgroundPanel"/> that will act as
        /// a background for the <see cref="AnimatedNode"/>s in the group. 
        /// </summary>
        public BackgroundPanel BgPanel { get; private set; }

        public BgPanelAnimatedNodeGroup(BackgroundPanel bgPanel)
        {
            BgPanel = bgPanel;
            VisibleNodesUpdated += HandleVisibleNodesUpdated;
        }

        private void HandleVisibleNodesUpdated(object _o, EventArgs _e)
        {
            bool shouldBeVisible = VisibleNodes.Count > 0;

            if (shouldBeVisible != BgPanel.IsVisible)
            {
                BgPanel.IsVisible = shouldBeVisible;
            }
        }

        /// <summary>
        /// Disposes of this <see cref="BgPanelAnimatedNodeGroup"/>,
        /// disconnecting the handler function used to modify the visibilty
        /// of <see cref="BgPanel"/>
        /// </summary>
        public void Dispose()
        {
            VisibleNodesUpdated -= HandleVisibleNodesUpdated;
        }
    }
}
