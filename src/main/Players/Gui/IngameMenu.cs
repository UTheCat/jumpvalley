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

        private Control backgroundNode;

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
                    transparencyTween.Speed = 1;

                    if (backgroundSizeTween != null)
                    {
                        backgroundSizeTween.Speed = 1;
                    }
                }
                else
                {
                    transparencyTween.Speed = -1;

                    if (backgroundSizeTween != null)
                    {
                        backgroundSizeTween.Speed = -1;
                    }
                }

                transparencyTween.Resume();
                if (backgroundSizeTween != null)
                {
                    backgroundSizeTween.Resume();
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
            backgroundNode = actualNode.GetNode<Control>("Background");

            transparencyTween = new MethodTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.Out);
            transparencyTween.InitialValue = 0;
            transparencyTween.FinalValue = 1;
            transparencyTween.OnStep += (object o, float frac) =>
            {
                ActualNode.Visible = frac > 0;
                Color modulate = ActualNode.Modulate;
                modulate.A = frac;
                ActualNode.Modulate = modulate;
            };

            if (backgroundNode != null)
            {
                backgroundSizeTween = new MethodTween(0.5, Tween.TransitionType.Quint, Tween.EaseType.Out);
                backgroundSizeTween.InitialValue = -20;
                backgroundSizeTween.FinalValue = 0;
                backgroundSizeTween.OnStep += (object o, float frac) =>
                {
                    float sizeOffset = (float)(backgroundSizeTween.GetCurrentValue() * 0.5);
                    backgroundNode.OffsetLeft = sizeOffset;
                    backgroundNode.OffsetRight = sizeOffset;
                    backgroundNode.OffsetTop = sizeOffset;
                    backgroundNode.OffsetBottom = sizeOffset;
                };
            }

            actualNode.Visible = false;
            IsShowing = false;
        }
    }
}
