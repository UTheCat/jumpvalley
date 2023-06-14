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

            AnchorBottom = scale;
            AnchorTop = scale;
            AnchorLeft = scale;
            AnchorRight = scale;
        };

        // tween on button press
        Pressed += tween.Resume;
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

        AnchorBottom = InitialScale;
        AnchorTop = InitialScale;
        AnchorLeft = InitialScale;
        AnchorRight = InitialScale;
    }
}