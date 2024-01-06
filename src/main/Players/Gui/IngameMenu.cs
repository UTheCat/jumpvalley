using Godot;
using System;

using Jumpvalley.Tweening;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// Code for the ingame menu,
    /// the menu displayed when clicking the three-dots at the bottom of the user's screen when the user is currently in a level.
    /// </summary>
    public partial class IngameMenu
    {
        /// <summary>
        /// The root node of the ingame menu
        /// </summary>
        public Control ActualNode { get; private set; }

        /// <summary>
        /// Tween handling the transparency of the menu's items, including its background panel
        /// </summary>
        private MethodTween transparencyTween;

        /// <summary>
        /// Tween handling the appearance of the menu's background panel
        /// </summary>
        private MethodTween backgroundSizeTween;

        private bool _isShowing;

        /// <summary>
        /// Whether or not the in-game menu should be visible.
        /// Toggling this property will run the animation for showing/hiding the menu.
        /// </summary>
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                if (value)
                {
                    
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of the ingame menu handler.
        /// </summary>
        /// <param name="actualNode">The root node of the ingame menu</param>
        /// <exception cref="ArgumentNullException"></exception>
        public IngameMenu(Control actualNode)
        {
            if (actualNode == null) throw new ArgumentNullException("actualNode", "The actualNode argument (argument #1) cannot be null.");

            ActualNode = actualNode;

            transparencyTween = new MethodTween();
            transparencyTween.OnStep += (object o, float frac) =>
            {

            };

            IsShowing = false;
        }
    }
}
