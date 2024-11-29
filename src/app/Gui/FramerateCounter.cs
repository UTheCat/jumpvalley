using System;
using Godot;
using Jumpvalley.Animation;
using Jumpvalley.Tweening;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Class that handles the app's framerate counter GUI.
    /// </summary>
    public partial class FramerateCounter : AnimatedNode, IDisposable
    {
        private SceneTreeTween opacityTween;
        private Control gui;
        private Label framesPerSecondLabel;
        private Label msPerFrameLabel;

        public double LowFps;
        public double HighFps;
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

        private double _currentFps;
        public double CurrentFps
        {
            get => _currentFps;
            set
            {
                _currentFps = value;

                framesPerSecondLabel.Text = $"{value.ToString("F1")} FPS";
                msPerFrameLabel.Text = $"{(1000.0/value).ToString("F1")} ms";

                Color labelColor = LowFpsColor.Lerp(HighFpsColor, (float)Math.Clamp(((value - LowFps) / (HighFps - LowFps)), 0.0, 1.0));
                framesPerSecondLabel.SelfModulate = labelColor;
                msPerFrameLabel.SelfModulate = labelColor;
            }
        }

        public FramerateCounter(Node actualNode) : base(actualNode)
        {
            LowFps = 30.0;
            HighFps = 60.0;
            LowFpsColor = Color.FromHsv(0f, 0.6f, 1f);
            HighFpsColor = Color.FromHsv(100f / 360f, 1f, 1f);

            if (actualNode is Control control)
            {
                gui = control;
                gui.Visible = false;

                framesPerSecondLabel = gui.GetNode<Label>("FramesPerSecond");
                msPerFrameLabel = gui.GetNode<Label>("MsPerFrame");

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

        public void Dispose()
        {
            opacityTween.Dispose();
        }
    }
}
