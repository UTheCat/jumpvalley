using Godot;
using System;

using UTheCat.Jumpvalley.Core.Tweening;

namespace UTheCat.Jumpvalley.App.Testing
{
    /// <summary>
    /// Test class for MethodTween using the IntervalTween subclass
    /// </summary>
    public partial class MethodTweenTest : Button
    {
        private SceneTreeTween tween = new SceneTreeTween(1, Tween.TransitionType.Quad, Tween.EaseType.InOut, null);

        public float InitialScale;
        public float FinalScale;

        public MethodTweenTest(float initialScale, float finalScale)
        {
            InitialScale = initialScale;
            FinalScale = finalScale;
            AddThemeFontSizeOverride("font_size", 20);
            Reset();
            UpdateText();

            tween.OnStep += (object _o, double _frac) =>
            {
                //Console.WriteLine($"MethodTween updated fraction: {frac}");

                float scale = (float)tween.GetCurrentValue();
                SetAnchorsViaScale(scale);
                UpdateText();
            };

            tween.OnResume += (object _o, EventArgs _e) =>
            {
                Console.WriteLine("Tween resumed");
            };

            tween.OnPause += (object _o, EventArgs _e) =>
            {
                Console.WriteLine("Tween paused");
            };

            tween.OnFinish += (object _o, EventArgs _e) =>
            {
                Console.WriteLine("Tween finished");
            };

            // tween on button press
            Pressed += () =>
            {
                Console.WriteLine("start tween");
                
                if (tween.IsPlaying)
                {
                    tween.Pause();
                } else
                {
                    tween.Tree = GetTree();
                    tween.Resume();
                }
            };
        }

        public void SetAnchorsViaScale(float scale)
        {
            AnchorBottom = scale;
            AnchorTop = 1 - scale;
            AnchorLeft = 1 - scale;
            AnchorRight = scale;
        }

        public void UpdateText()
        {
            Text = "Click to tween me!"
            + $"\nCurrentFraction: {tween.CurrentFraction}"
            + $"\nGetCurrentValue() result: {tween.GetCurrentValue()}"
            + $"\nElapsedTime: {tween.ElapsedTime}";
        }

        public void Reset()
        {
            tween.InitialValue = InitialScale;
            tween.FinalValue = FinalScale;

            SetAnchorsViaScale(InitialScale);
        }
    }
}
