using System;
using UTheCat.Jumpvalley.Core.Animation;
using UTheCat.Jumpvalley.Core.Gui;

namespace UTheCat.Jumpvalley.App.Gui
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

        public bool ShouldBeVisible { get; private set; } = false;

        public BgPanelAnimatedNodeGroup(BackgroundPanel bgPanel)
        {
            BgPanel = bgPanel;

            UpdateBgPanelVisibility();
            CurrentlyVisibleNodeChanged += HandleCurrentlyVisibleNodeChanged;
        }

        private void UpdateBgPanelVisibility()
        {
            bool shouldBeVisible = CurrentlyVisibleNode != null;
            ShouldBeVisible = shouldBeVisible;

            if (shouldBeVisible != BgPanel.IsVisible)
            {
                BgPanel.IsVisible = shouldBeVisible;
            }
        }

        private void HandleCurrentlyVisibleNodeChanged(object _o, EventArgs _e)
        {
            UpdateBgPanelVisibility();
        }

        /// <summary>
        /// Disposes of this <see cref="BgPanelAnimatedNodeGroup"/>,
        /// disconnecting the handler function used to modify the visibilty
        /// of <see cref="BgPanel"/>
        /// </summary>
        public new void Dispose()
        {
            CurrentlyVisibleNodeChanged -= HandleCurrentlyVisibleNodeChanged;

            base.Dispose();
        }
    }
}
