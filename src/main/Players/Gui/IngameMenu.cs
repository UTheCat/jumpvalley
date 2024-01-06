using Godot;
using System;

using Jumpvalley.Tweening;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// Code for the in-game menu,
    /// the menu displayed when clicking the three-dots at the bottom of the user's screen when the user is currently in a level.
    /// </summary>
    public partial class InGameMenu
    {
        /// <summary>
        /// The root node of the in-game menu
        /// </summary>
        public Control ActualNode { get; private set; }

        /// <summary>
        /// Tween handling the transparency of the menu's items, including its background panel
        /// </summary>
        private SceneTreeTween transparencyTween;

        /// <summary>
        /// Tween handling the appearance of the menu's background panel
        /// </summary>
        private SceneTreeTween backgroundSizeTween;

        private Control backgroundNode;
        private Label titleLabel;
        private Label subtitleLabel;

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
        /// Constructs a new instance of the in-game menu handler.
        /// </summary>
        /// <param name="actualNode">The root node of the in-game menu</param>
        /// <param name="tree">The scene tree that the in-game menu is in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public InGameMenu(Control actualNode, SceneTree tree)
        {
            if (actualNode == null) throw new ArgumentNullException("actualNode", "The actualNode argument (argument #1) cannot be null.");

            ActualNode = actualNode;
            backgroundNode = actualNode.GetNode<Control>("Background");
            titleLabel = actualNode.GetNode<Label>("Title");
            subtitleLabel = actualNode.GetNode<Label>("Subtitle");

            transparencyTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.Out, tree);
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
                backgroundSizeTween = new SceneTreeTween(0.5, Tween.TransitionType.Quint, Tween.EaseType.Out, tree);
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
            
            if (titleLabel != null)
            {
                titleLabel.Text = actualNode.Tr("MENU_TITLE");
                subtitleLabel.Text = actualNode.Tr("MENU_SUBTITLE");
            }

            IsShowing = false;
        }
    }
}
