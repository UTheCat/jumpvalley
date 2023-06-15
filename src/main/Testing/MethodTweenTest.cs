using Godot;
using System;

/// <summary>
/// Test class for MethodTween using the IntervalTween subclass
/// </summary>
public partial class MethodTweenTest : Button
{
    private IntervalTween tween = new IntervalTween(1, Tween.TransitionType.Quad, Tween.EaseType.InOut);

    public float InitialScale;
    public float FinalScale;

    public MethodTweenTest(float initialScale, float finalScale)
    {
        InitialScale = initialScale;
        FinalScale = finalScale;
        AddThemeFontSizeOverride("font_size", 20);
        Reset();
        UpdateText();

        tween.OnStep += (object _o, float frac) =>
        {
            Console.WriteLine($"MethodTween updated fraction: {frac}");

            float scale = (float)tween.GetCurrentValue();
            SetAnchorsViaScale(scale);
            UpdateText();
        };

        // tween on button press
        Pressed += () =>
        {
            Console.WriteLine("start tween");
            tween.Resume();
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