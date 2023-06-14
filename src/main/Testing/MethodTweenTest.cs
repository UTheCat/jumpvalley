using Godot;
using System;

/// <summary>
/// Test class for MethodTween
/// </summary>
public partial class MethodTweenTest : Button
{
    private MethodTween tween = new MethodTween(1, Tween.TransitionType.Sine, Tween.EaseType.InOut);

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
        AnchorBottom = 1 - scale;
        AnchorTop = scale;
        AnchorLeft = scale;
        AnchorRight = 1 - scale;
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