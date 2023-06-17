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
        private Stopwatch stopwatch = new Stopwatch();
        private Timer timer;

        // in seconds
        private double lastTimestamp = 0;

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
                                double timestamp = stopwatch.Elapsed.TotalSeconds;
                                Step(timestamp - lastTimestamp);
                                lastTimestamp = timestamp;
                            }
                        };

                        stopwatch.Start();
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

                        // commented out since this causes odd behavior with timestampping to occur
                        //resetStopwatch();

                        stopwatch.Stop();
                    }
                }
            }
        }

        public void Dispose()
        {
            Pause();
            resetStopwatch();
        }

        private void resetStopwatch()
        {
            stopwatch.Stop();
            stopwatch.Reset();
            lastTimestamp = 0;
        }
    }
}
