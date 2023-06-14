using Godot;

/// <summary>
/// Provides a way of tweening via Godot using method overrides.
/// <br/>
/// The advantage of using this over a normal Godot <see cref="Tween"/> is that the Tween doesn't have to be in the scene tree in order to operate.
/// <br/>
/// Syntax is inspired by JavaFX's animation package.
/// </summary>
public partial class MethodTween: Node
{
    /// <summary>
    /// Returns a linear interpolation between an initial value and a final value for a given fraction.
    /// </summary>
    /// <param name="initVal">The initial value</param>
    /// <param name="finalVal">The final value</param>
    /// <param name="frac">The fraction of the interpolation, typically in the range of [0, 1].</param>
    /// <returns></returns>
    public static double Lerp(double initVal, double finalVal, double frac)
    {
        return initVal + (finalVal - initVal) * frac;
    }

    public MethodTween(double transitionTime, Tween.TransitionType transitionType, Tween.EaseType easeType)
    {
        // tween shouldn't run after being instantiated unless the dev wants it to
        SetProcess(false);

        TransitionTime = transitionTime;
        TransitionType = transitionType;
        EaseType = easeType;
    }

    public MethodTween() : this(0, Tween.TransitionType.Linear, Tween.EaseType.InOut) { }

    /// <summary>
    /// The duration of the entire tween in seconds
    /// </summary>
    public double TransitionTime;

    /// <summary>
    /// The transition type (parent function that will be doing the easing) as defined in Godot's <see cref="Tween.TransitionType"/>
    /// </summary>
    public Tween.TransitionType TransitionType;

    /// <summary>
    /// The easing type
    /// <br/>
    /// This controls how the tween speeds up and slows down over time. See Godot's documentation for <see cref="Tween.EaseType"/> for more details.
    /// </summary>
    public Tween.EaseType EaseType;

    /// <summary>
    /// The initial value of the tween
    /// </summary>
    public double InitialValue = 0;

    /// <summary>
    /// The final value of the tween
    /// </summary>
    public double FinalValue = 0;

    /// <summary>
    /// Returns the current value of the tween. This value is calculated by performing linear interpolation between <see cref="InitialValue"/> and <see cref="EndingValue"/> with the fraction between the value being <see cref="CurrentFraction"/>
    /// </summary>
    /// <returns>said result</returns>
    public double GetCurrentValue()
    {
        return Lerp(InitialValue, FinalValue, CurrentFraction);
    }

    /// <summary>
    /// The speed of the tweening where 1 represents normal speed, 0 represents freezing, and -1 represents backwards at normal speed.
    /// </summary>
    public float Speed = 1;

    private bool _isPlaying = false;
    private float _currentFraction = 0;
    private double _elapsedTime = 0;

    /// <summary>
    /// Whether or not the tween is current playing/running.
    /// </summary>
    public bool IsPlaying
    {
        get => _isPlaying;
        private set
        {
            _isPlaying = value;
        }
    }

    /// <summary>
    /// The current fraction of the tween that has been completed so far, typically in the range of [0, 1].
    /// </summary>
    public float CurrentFraction
    {
        get => _currentFraction;
        private set
        {
            _currentFraction = value;
        }
    }

    /// <summary>
    /// The elapsed time that the tween has been running in seconds. This speed at which this value changes is affected by <see cref="Speed"/>
    /// </summary>
    public double ElapsedTime
    {
        get => _elapsedTime;
        private set
        {
            _elapsedTime = value;

            // update CurrentFraction based on ElapsedTime
            if (TransitionTime <= 0)
            {
                // if the transition completes immediately, it makes sense just to set CurrentFraction to 1 immediately
                CurrentFraction = 1f;
            }
            else
            {
                CurrentFraction = (float) (value / TransitionTime);
            }
        }
    }

    /// <summary>
    /// Interpolation function meant to be overriden by developers. As the tween is running, this method is called on each step until the animation pauses or finishes.
    /// </summary>
    /// <param name="frac">The current fraction of the tween that has been completed so far, typically in the range of [0, 1].</param>
    public void Interpolate(float frac) { }

    /// <summary>
    /// Pauses the tween
    /// </summary>
    public void Pause()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            SetProcess(false);
        }
    }

    /// <summary>
    /// Starts or resumes the tween
    /// </summary>
    public void Resume()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
            SetProcess(true);
        }
    }

    public override void _Process(double delta)
    {
        if (IsPlaying && Speed != 0)
        {
            // CurrentFraction will also get set here
            ElapsedTime = Mathf.Clamp(ElapsedTime + (delta * Speed), 0.0, TransitionTime);

            // Indicate that a step has occurred
            Interpolate(CurrentFraction);

            // If the animation hits either the start or end after being played already, pause it
            if ((ElapsedTime <= 0 && Speed < 0) || (ElapsedTime >= TransitionTime && Speed > 0))
            {
                Pause();
            }
        }
    }
}
