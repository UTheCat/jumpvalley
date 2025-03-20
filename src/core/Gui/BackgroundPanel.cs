using Godot;

using UTheCat.Jumpvalley.Core.Animation;
using UTheCat.Jumpvalley.Core.Tweening;

namespace UTheCat.Jumpvalley.Core.Gui
{
    /// <summary>
    /// Handler for the BackgroundPanel,
    /// a panel (or some other Godot Control) that acts as a background for a currently visible menu.
    /// </summary>
    public partial class BackgroundPanel : AnimatedNode
    {
        private Control panel;
        private SceneTreeTween opacityTween;

        public override bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                base.IsVisible = value;

                if (value)
                {
                    opacityTween.Speed = 1f;
                }
                else
                {
                    opacityTween.Speed = -1f;
                }

                opacityTween.Resume();
            }
        }

        public float _opacity;

        /// <summary>
        /// The opacity of the background panel to reach when it's visible
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                opacityTween.FinalValue = value;
            }
        }

        public BackgroundPanel(Control node) : base(node)
        {
            panel = node;

            node.Visible = false;
            Color initialModulate = node.Modulate;
            initialModulate.A = 0f;
            node.Modulate = initialModulate;

            opacityTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.Out, node.GetTree())
            {
                InitialValue = 0
            };
            opacityTween.OnStep += (object o, float frac) =>
            {
                float opacity = (float)opacityTween.GetCurrentValue();
                panel.Visible = opacity > 0;
                Color modulate = panel.Modulate;
                modulate.A = opacity;
                panel.Modulate = modulate;
            };

            Opacity = 0.25f;
            IsVisible = false;
        }
    }
}
