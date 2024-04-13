using System;
using System.Diagnostics;
using System.Timers;

namespace Jumpvalley.Tweening
{
    /// <summary>
    /// Class that runs a tween by incrementing steps at a specified time interval.
    /// <br/>
    /// Note that while the time interval should mostly be fixed, there will be slight differences in the actual time between each step.
    /// <br/>
    /// To account for this, the <see cref="Stopwatch"/> class is used to run the tween based on a timestamp. Therefore, the aforementioned differences shouldn't add up and cause the tween to finish noticeably later than expected for longer tweens.
    /// </summary>
    public partial class IntervalTween : MethodTween, IDisposable
    {
        private Timer timer;

        public IntervalTween(double transitionTime, Godot.Tween.TransitionType transitionType, Godot.Tween.EaseType easeType) : base(transitionTime, transitionType, easeType) { }

        public IntervalTween() : base() { }

        /// <summary>
        /// The number of seconds between each step
        /// </summary>
        public double Interval = 0.001;

        public override bool IsPlaying
        {
            get => base.IsPlaying;
            protected set
            {
                base.IsPlaying = value;

                if (value)
                {
                    if (timer == null)
                    {
                        // play the tween
                        timer = new Timer();

                        timer.Interval = Interval * 1000;
                        timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                        {
                            if (IsPlaying)
                            {
                                Step();
                            }
                        };

                        timer.Start();
                    }
                }
                else
                {
                    // stop the tween
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                        timer = null;
                    }
                }
            }
        }
    }
}
