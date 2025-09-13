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
        private static readonly float MENU_HIDE_ANIM_HEIGHT_REDUCTION = 40f;

        // Both of these durations are in seconds
        private static readonly double OPACITY_ANIM_DURATION = 0.2;
        private static readonly double SIZE_ANIM_DURATION = 0.4;

        /// <summary>
        /// Tween handling the opacity of the menu's items, including its background panel
        /// </summary>
        private SceneTreeTween opacityTween;

        /// <summary>
        /// Tween handling the appearance of the menu's background panel
        /// </summary>
        private SceneTreeTween backgroundSizeTween;

        /// <summary>
        /// Current size of the level menu
        /// </summary>
        private Vector2 currentSize = Vector2.Zero;

        /// <summary>
        /// Size of the actual level menu when this level menu handler was instantiated
        /// </summary>
        private Vector2 originalSize = Vector2.Zero;

        /// <summary>
        /// Height of the actual level menu when this level menu handler was instantiated
        /// </summary>
        private float originalHeight = 0f;

        private int lastWindowHeight = 0;

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
        /// The scroll container (which is a child of <see cref="ItemsControl"/>)
        /// that spans the entire size of <see cref="ItemsControl"/>.
        /// </summary>
        public ScrollContainer ItemsScrollContainer { get; private set; }

        /// <summary>
        /// Node which is a child of <see cref="ItemsScrollContainer"/>.
        /// <br/><br/>
        /// If the ability to scroll through the level menu's items is desired,
        /// the level menu's items should be placed in this container.
        /// </summary>
        public BoxContainer ScrollableItemsBoxContainer { get; private set; }

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
                    opacityTween.Speed = 1;

                    if (backgroundSizeTween != null)
                    {
                        backgroundSizeTween.Speed = 1;
                    }
                }
                else
                {
                    opacityTween.Speed = -1;

                    if (backgroundSizeTween != null)
                    {
                        backgroundSizeTween.Speed = -1;
                    }
                }

                opacityTween.Resume();
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
            CloseButton = actualNode.GetNodeOrNull<Button>("CloseButton");

            ItemsControl = actualNode.GetNodeOrNull<Control>("Items");
            ItemsScrollContainer = ItemsControl?.GetNodeOrNull<ScrollContainer>("ScrollContainer");
            ScrollableItemsBoxContainer = ItemsScrollContainer?.GetNodeOrNull<BoxContainer>("BoxContainer");

            Vector2 nodeSize = actualNode.Size;
            originalSize = nodeSize;
            originalHeight = nodeSize.Y;
            widthHeightRatio = nodeSize.X / nodeSize.Y;

            opacityTween = new SceneTreeTween(OPACITY_ANIM_DURATION, Tween.TransitionType.Linear, Tween.EaseType.Out, tree);
            opacityTween.InitialValue = 0;
            opacityTween.FinalValue = 1;
            opacityTween.OnStep += (object o, double frac) =>
            {
                actualNode.Visible = frac > 0.0;
                Color modulate = actualNode.Modulate;
                modulate.A = (float)frac;
                actualNode.Modulate = modulate;
            };

            if (BackgroundControl != null)
            {
                backgroundSizeTween = new SceneTreeTween(SIZE_ANIM_DURATION, Tween.TransitionType.Quad, Tween.EaseType.Out, tree);

                // While tween is running, the height of BackgroundControl is set to
                // its original height - backgroundSizeTween.GetCurrentValue() * 0.5.
                // The width of BackgroundControl is adjusted accordingly with whatever
                // widthHeightRatio is calculated to be.
                backgroundSizeTween.InitialValue = MENU_HIDE_ANIM_HEIGHT_REDUCTION;
                backgroundSizeTween.FinalValue = 0.0;
                backgroundSizeTween.OnStep += (object o, double _frac) =>
                {
                    SetMenuSizeOffset((float)backgroundSizeTween.GetCurrentValue() - GetHeightReductionDueToWindowHeight());
                };
            }

            if (CloseButton != null) CloseButton.Pressed += OnCloseButtonPressed;

            actualNode.Visible = false;

            IsVisible = false;
        }

        public new void Dispose()
        {
            CloseButton.Pressed -= OnCloseButtonPressed;

            opacityTween.Dispose();
            backgroundSizeTween.Dispose();
        }

        private void OnCloseButtonPressed()
        {
            if (IsVisible) IsVisible = false;
        }

        /// <summary>
        /// Returns a vector whose X component is the width reduction of the level menu due to window size
        /// and whose Y component is the height reduction of the level menu due to window size.
        /// </summary>
        private float GetHeightReductionDueToWindowHeight(float windowHeight)
        {
            return windowHeight < originalHeight ? originalHeight - windowHeight : 0f;
        }

        private float GetHeightReductionDueToWindowHeight()
        {
            return GetHeightReductionDueToWindowHeight(DisplayServer.WindowGetSize().Y);
        }

        private void SetMenuSizeOffset(float heightOffset)
        {
            heightOffset *= 0.5f;
            float widthOffset = heightOffset * widthHeightRatio;

            BackgroundControl.OffsetLeft = widthOffset;
            BackgroundControl.OffsetRight = -widthOffset;
            BackgroundControl.OffsetTop = heightOffset;
            BackgroundControl.OffsetBottom = -heightOffset;
        }

        public override void _Process(double delta)
        {
            Vector2I windowSize = DisplayServer.WindowGetSize();
            int height = windowSize.Y;

            if (lastWindowHeight == 0 || height != lastWindowHeight)
            {
                lastWindowHeight = height;

                if (!backgroundSizeTween.IsPlaying) SetMenuSizeOffset((float)backgroundSizeTween.GetCurrentValue() - GetHeightReductionDueToWindowHeight(height));
            }
        }
    }
}
