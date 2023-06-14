using Godot;

/// <summary>
/// Class that wraps the default Godot Tween class to make Godot tweening easier.
/// </summary>
public class WrappedTween: Node
{
    public Tween Tween;

    public WrappedTween(bool autoplay)
    {
        Tween = new Tween();

        // as of the current major version of Godot (v4), tweens autoplay by default
        // therefore, if the user doesn't want the tween to automatically play on creation,
        // immediately stop it
        if (!autoplay)
        {
            Tween.Stop();
        }
        
    }

    public WrappedTween(Tween tween)
    {
        Tween = tween;
    }
}