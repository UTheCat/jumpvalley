using Godot;
using System;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// This class provides some components bind to a Godot control node and make adding animations to it easier.
    /// </summary>
    public partial class AnimatedControl
    {
        /// <summary>
        /// The root node of the Godot control to be animated
        /// </summary>
        public Control ActualNode { get; private set; }

        private bool _isVisible;

        /// <summary>
        /// Whether or not the level menu should be visible.
        /// Toggling this property will run the animation for showing/hiding the menu.
        /// </summary>
        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                RaiseVisibilityChanged(value);
            }
        }

        /// <summary>
        /// Creates a new instance of AnimatedControl
        /// </summary>
        /// <param name="actualNode">The root node of the Godot control to be animated</param>
        public AnimatedControl(Control actualNode)
        {
            ActualNode = actualNode;
        }

        /// <summary>
        /// Event that's raised when <see cref="IsVisible"/> gets toggled.
        /// The boolean argument of this event is the new value of <see cref="IsVisible"/>
        /// </summary>
        public event EventHandler<bool> VisibilityChanged;

        protected void RaiseVisibilityChanged(bool isVisible)
        {
            VisibilityChanged?.Invoke(this, isVisible);
        }
    }
}
