using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Controls
{
    /// <summary>
    /// Limits how many times the game screen is rendered within a given time interval
    /// <br/>
    /// Currently, the only option is to cap rendering framerate slightly below the rendering refresh rate.
    /// This is to test lowering input delay while maintaining smooth rendering with V-sync and Free-Sync enabled.
    /// </summary>
    public partial class RenderFramerateLimiter : Node, IDisposable
    {
        /// <summary>
        /// The minimum difference between the maximum rendering frames-per-second and the monitor's refresh rate
        /// </summary>
        public float MinFpsDifference = 0.5f;

        /// <summary>
        /// The maximum difference between the maximum rendering frames-per-second and the monitor's refresh rate
        /// </summary>
        //public float MaxFpsDifference = 1;

        /// <summary>
        /// The current maximum framerate enforced by this <see cref="RenderFramerateLimiter"/>
        /// </summary>
        public int MaxFps { get; private set; } = 60;

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
            Name = $"{nameof(RenderFramerateLimiter)}@{GetHashCode()}";
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
            if (refreshRate < 0)
            {
                return;
            }

            int maxFps = (int)Math.Floor((double)refreshRate);

            // Lower max frames-per-second by 1 in case the difference isn't great enough as specified by MinFpsDifference
            if (refreshRate - maxFps < MinFpsDifference)
            {
                maxFps -= 1;
            }

            if (MaxFps == maxFps) return;

            MaxFps = maxFps;
            Engine.MaxFps = Math.Max(0, maxFps);

            base._Process(delta);
        }
    }
}
