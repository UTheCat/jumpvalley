using System;

using Jumpvalley.Levels;
using Jumpvalley.Players.Gui;

namespace Jumpvalley.Players
{
    /// <summary>
    /// Jumpvalley's primary level runner that's intended to be running on behalf of the user who's playing the game.
    /// </summary>
    public partial class UserLevelRunner : LevelRunner, IDisposable
    {
        /// <summary>
        /// Level timer GUI operator in charge of displaying how long the user's current run has lasted.
        /// The run mentioned here is for the level that the user is currently playing.
        /// </summary>
        public LevelTimer LevelTimerOperator { get; private set; }

        public UserLevelRunner(Player player, LevelTimer levelTimerOperator) : base(player)
        {
            Name = $"{nameof(UserLevelRunner)}_{GetHashCode()}";

            LevelTimerOperator = levelTimerOperator;
            SetProcess(true);
        }

        public new void Dispose()
        {
            SetProcess(false);
            base.Dispose();
        }

        public override void _Process(double delta)
        {
            LevelPackage levelPackage = CurrentLevelPackage;
            if (levelPackage != null)
            {
                Level level = levelPackage.LevelInstance;
                if (level != null)
                {
                    LevelTimerOperator.ElapsedTime = level.Clock.OffsetElapsedTime.TotalSeconds;
                }
            }

            base._Process(delta);
        }
    }
}
