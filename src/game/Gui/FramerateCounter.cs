using Godot;
using Jumpvalley.Animation;
using Jumpvalley.Tweening;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that handles the game's framerate counter GUI.
    /// </summary>
    public partial class FramerateCounter : AnimatedNode
    {
        private static readonly string FONT_COLOR_NAME = "font_color";
        private SceneTreeTween opacityTween;
        private Control gui;
        private Label framesPerSecondLabel;
        private Label msPerFrameLabel;

        public float LowFps;
        public float HighFps;
        public Color LowFpsColor;
        public Color HighFpsColor;

        public override bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                base.IsVisible = value;

                if (value)
                {
                    opacityTween.Speed = 1;
                }
                else
                {
                    opacityTween.Speed = -1;
                }

                opacityTween.Resume();
            }
        }

        private float _currentFps;
        public float CurrentFps
        {
            get => _currentFps;
            set
            {
                _currentFps = value;

                framesPerSecondLabel.Text = $"{value} FPS";
                msPerFrameLabel.Text = $"{1f/value} ms";

                Color labelColor = LowFpsColor.Lerp(HighFpsColor, (value - HighFps) / (HighFps - LowFps));
                framesPerSecondLabel.SelfModulate = labelColor;
                msPerFrameLabel.SelfModulate = labelColor;
            }
        }

        public FramerateCounter(Node actualNode) : base(actualNode)
        {
            LowFps = 30f;
            HighFps = 60f;
            LowFpsColor = Color.FromHsv(0f, 0.6f, 1f);
            HighFpsColor = Color.FromHsv(100f / 360f, 1f, 1f);

            if (actualNode is Control control)
            {
                gui = control;
                gui.Visible = false;

                opacityTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.Out, actualNode.GetTree())
                {
                    InitialValue = 0.0,
                    FinalValue = 1.0,
                    Speed = 1f
                };
                opacityTween.OnStep += (object _o, float frac) =>
                {
                    float opacity = (float)opacityTween.GetCurrentValue();
                    gui.Visible = opacity > 0;

                    Color modulate = gui.Modulate;
                    modulate.A = opacity;
                    gui.Modulate = modulate;
                };
            }
        }
    }
}
