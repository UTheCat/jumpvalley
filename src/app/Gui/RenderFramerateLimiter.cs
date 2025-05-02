using Godot;
using System;

namespace UTheCat.Jumpvalley.App.Gui
{
    /// <summary>
    /// Limits how many times the app is rendered within a given time interval
    /// <br/>
    /// Currently, the only option is to cap rendering framerate slightly below the rendering refresh rate.
    /// This is to test lowering input delay while maintaining smooth rendering with V-sync and Free-Sync enabled.
    /// </summary>
    public partial class RenderFramerateLimiter : Node, IDisposable
    {
        /// <summary>
        /// The minimum difference between the maximum rendering frames-per-second and the monitor's refresh rate
        /// </summary>
        public float MinFpsDifference = 0f;

        /// <summary>
        /// The maximum difference between the maximum rendering frames-per-second and the monitor's refresh rate
        /// </summary>
        //public float MaxFpsDifference = 1;

        /// <summary>
        /// The current maximum framerate enforced by this <see cref="RenderFramerateLimiter"/>
        /// </summary>
        public int MaxFps { get; private set; } = 120;

        public bool _isRunning = false;

        /// <summary>
        /// Whether or not this <see cref="RenderFramerateLimiter"/> should be updating the maximum rendering frames-per-second at the moment
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                SetProcess(value);
            }
        }

        /// <summary>
        /// Creates and enables a new <see cref="RenderFramerateLimiter"/>
        /// </summary>
        public RenderFramerateLimiter()
        {
            SetProcess(false);
        }

        /// <summary>
        /// Disables and disposes of this <see cref="RenderFramerateLimiter"/>
        /// </summary>
        public new void Dispose()
        {
            IsRunning = false;
            QueueFree();
            base.Dispose();
        }

        public override void _Process(double delta)
        {
            float refreshRate = DisplayServer.ScreenGetRefreshRate();

            // If ScreenGetRefreshRate fails, it returns -1
            if (refreshRate < 0) return;

            int maxFps = (int)Math.Round((refreshRate - MinFpsDifference) * 2.0);

            if (MaxFps == maxFps) return;

            MaxFps = maxFps;
            Engine.MaxFps = Math.Max(0, maxFps);

            base._Process(delta);
        }
    }
}
