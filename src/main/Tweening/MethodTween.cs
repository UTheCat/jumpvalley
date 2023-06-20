using Godot;
using System;
using System.Diagnostics;

namespace Jumpvalley.Tweening
{
    /// <summary>
    /// Provides a way of tweening via Godot using method overrides.
    /// <br/>
    /// The advantage of using this over a normal Godot <see cref="Tween"/> is that the Tween doesn't have to be in the scene tree in order to operate.
    /// <br/>
    /// Another difference is that unlike a normal Godot <see cref="Tween"/>, this tween doesn't start automatically after being instantiated.
    /// Developers must call Resume() manually in order to get the tween running.
    /// <br/>
    /// Syntax is inspired by JavaFX's animation package.
    /// </summary>
    public partial class MethodTween : IDisposable
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

        /// <summary>
        /// Stopwatch used to keep track of the tweening timestamp
        /// </summary>
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// The last tweening timestamp that Step() was called with no arguments.
        /// </summary>
        private double lastTimestamp = 0;

        private bool _isPlaying = false;
        private float _currentFraction = 0;
        private double _elapsedTime = 0;

        /// <summary>
        /// Whether or not the tween is current playing/running.
        /// </summary>
        public virtual bool IsPlaying
        {
            get => _isPlaying;
            protected set
            {
                if (value == _isPlaying) return;

                _isPlaying = value;

                if (value)
                {
                    // raise the event first so that the timestamp at which the tween was resumed can be more accurately retrieved
                    RaiseOnResume();
                    stopwatch.Start();
                }
                else
                {
                    stopwatch.Stop();
                    RaiseOnPause();
                }
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
                    // get current fraction based on start and end values, along with easing modifiers
                    CurrentFraction = (float)Tween.InterpolateValue(0f, 1f, value, TransitionTime, TransitionType, EaseType);
                }
            }
        }

        /// <summary>
        /// Pauses the tween
        /// </summary>
        public virtual void Pause()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
            }
        }

        /// <summary>
        /// Starts or resumes the tween
        /// </summary>
        public virtual void Resume()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        /// <summary>
        /// Moves the current tweening position by a custom step in seconds.
        /// </summary>
        /// <param name="delta">The time in seconds to increment elapsed time in.</param>
        public void Step(double delta)
        {
            // CurrentFraction will also get set here
            ElapsedTime = Mathf.Clamp(ElapsedTime + (delta * Speed), 0.0, TransitionTime);

            // Indicate that a step has occurred
            RaiseOnStep(CurrentFraction);

            // If the animation hits either the start or end after being played already, pause it
            if (IsPlaying && ((ElapsedTime <= 0 && Speed < 0) || (ElapsedTime >= TransitionTime && Speed > 0)))
            {
                Pause();
                RaiseOnFinish();
            }
        }

        /// <summary>
        /// Calls <see cref="Step(double)"/> where the "double" argument is the number of seconds that
        /// <see cref="Step()"/> (with no arguments) was called.
        /// <br/>
        /// If this is the first time that <see cref="Step()"/> (with no arguments) was called since instantiation,
        /// the "double" argument will instead be the number of seconds since the tween started running.
        /// </summary>
        public void Step()
        {
            double timestamp = stopwatch.Elapsed.TotalSeconds;
            Step(timestamp - lastTimestamp);
            lastTimestamp = timestamp;
        }

        public virtual void Dispose()
        {
            Pause();
            ResetStopwatch();
        }

        /// <summary>
        /// Event that gets raised on each step of the tween until the animation pauses or finishes.
        /// <br/>
        /// The <see cref="float"/> argument of the event is the current fraction of the tween that has been completed so far, typically in the range of [0, 1].
        /// <br/>
        /// <example>
        /// Example usage:
        /// <code>
        /// MethodTween t = new MethodTween();
        /// 
        /// public void HandleTweenStep(object sender, float frac)
        /// {
        ///     Console.WriteLine($"The current fraction of the tween is {frac}");
        /// }
        /// 
        /// public void ConnectToTweenStep()
        /// {
        ///     t.OnStep += HandleTweenStep;
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public event EventHandler<float> OnStep;

        // Event raising function for OnStep
        protected void RaiseOnStep(float frac)
        {
            EventHandler<float> onStep = OnStep;
            if (onStep != null)
            {
                onStep(this, frac);
            }
        }

        /// <summary>
        /// Event raised when the tween resumes playback
        /// </summary>
        public event EventHandler OnResume;

        protected void RaiseOnResume()
        {
            OnResume?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the tween is paused during playback
        /// </summary>
        public event EventHandler OnPause;

        protected void RaiseOnPause()
        {
            OnPause?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the tween finishes playback.
        /// <br/>
        /// This occurs when <see cref="CurrentFraction"/> hits 1 when <see cref="Speed"/> is greater than 0, or when <see cref="CurrentFraction"/> hits 0 when <see cref="Speed"/> is less than 0.
        /// </summary>
        public event EventHandler OnFinish;

        protected void RaiseOnFinish()
        {
            OnFinish?.Invoke(this, EventArgs.Empty);
        }

        private void ResetStopwatch()
        {
            stopwatch.Stop();
            stopwatch.Reset();
            lastTimestamp = 0;
        }
    }
}
