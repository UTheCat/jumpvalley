using Godot;
using System;

using UTheCat.Jumpvalley.Core.Animation;
using UTheCat.Jumpvalley.Core.Tweening;

namespace UTheCat.Jumpvalley.App.Gui
{
    /// <summary>
    /// Code for the level menu, a type of menu that's typically displayed only when the player is playing a level.
    /// </summary>
    public partial class LevelMenu : AnimatedNode, IDisposable
    {
        /// <summary>
        /// Tween handling the transparency of the menu's items, including its background panel
        /// </summary>
        private SceneTreeTween transparencyTween;

        /// <summary>
        /// Tween handling the appearance of the menu's background panel
        /// </summary>
        private SceneTreeTween backgroundSizeTween;

        /// <summary>
        /// The Godot control displaying the menu's background
        /// </summary>
        public Control BackgroundControl { get; private set; }

        /// <summary>
        /// The text label displaying the menu's title
        /// </summary>
        public Label TitleLabel { get; private set; }

        /// <summary>
        /// The text label displaying the menu's subtitle
        /// </summary>
        public Label SubtitleLabel { get; private set; }

        /// <summary>
        /// The Godot control displaying the menu's items
        /// </summary>
        public Control ItemsControl { get; private set; }

        /// <summary>
        /// The button that closes the menu when pressed
        /// </summary>
        public Button CloseButton { get; private set; }

        /// <summary>
        /// Whether or not the level menu should be visible.
        /// Toggling this property will run the animation for showing/hiding the menu.
        /// </summary>
        public override bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                base.IsVisible = value;

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

        private float widthHeightRatio;

        /// <summary>
        /// Constructs a new instance of the level menu handler.
        /// </summary>
        /// <param name="actualNode">The root node of the level menu</param>
        /// <param name="tree">The scene tree that the level menu is in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LevelMenu(Control actualNode, SceneTree tree) : base(actualNode)
        {
            BackgroundControl = actualNode.GetNodeOrNull<Control>("Background");
            TitleLabel = actualNode.GetNodeOrNull<Label>("Title");
            SubtitleLabel = actualNode.GetNodeOrNull<Label>("Subtitle");
            ItemsControl = actualNode.GetNodeOrNull<Control>("Items");
            CloseButton = actualNode.GetNodeOrNull<Button>("CloseButton");

            Vector2 nodeSize = actualNode.Size;
            widthHeightRatio = nodeSize.X / nodeSize.Y;

            transparencyTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.Out, tree);
            transparencyTween.InitialValue = 0;
            transparencyTween.FinalValue = 1;
            transparencyTween.OnStep += (object o, double frac) =>
            {
                actualNode.Visible = frac > 0.0;
                Color modulate = actualNode.Modulate;
                modulate.A = (float)frac;
                actualNode.Modulate = modulate;
            };

            if (BackgroundControl != null)
            {
                backgroundSizeTween = new SceneTreeTween(0.5, Tween.TransitionType.Quad, Tween.EaseType.Out, tree);

                // While tween is running, the height of BackgroundControl is set to
                // its original height - backgroundSizeTween.GetCurrentValue() * 0.5.
                // The width of BackgroundControl is adjusted accordingly with whatever
                // widthHeightRatio is calculated to be.
                backgroundSizeTween.InitialValue = 40.0;
                backgroundSizeTween.FinalValue = 0.0;
                backgroundSizeTween.OnStep += (object o, double _frac) =>
                {
                    float heightOffset = (float)(backgroundSizeTween.GetCurrentValue() * 0.5);
                    float widthOffset = heightOffset * widthHeightRatio;

                    BackgroundControl.OffsetLeft = widthOffset;
                    BackgroundControl.OffsetRight = -widthOffset;
                    BackgroundControl.OffsetTop = heightOffset;
                    BackgroundControl.OffsetBottom = -heightOffset;
                };
            }

            if (CloseButton != null) CloseButton.Pressed += OnCloseButtonPressed;

            actualNode.Visible = false;

            IsVisible = false;
        }

        public void Dispose()
        {
            CloseButton.Pressed -= OnCloseButtonPressed;

            transparencyTween.Dispose();
            backgroundSizeTween.Dispose();
        }

        private void OnCloseButtonPressed()
        {
            if (IsVisible) IsVisible = false;
        }
    }
}
