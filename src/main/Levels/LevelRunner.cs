using Jumpvalley.Players;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Class responsible for running and stopping levels.
    /// </summary>
    public partial class LevelRunner
    {
        /// <summary>
        /// The Player instance that the LevelRunner is associated with
        /// </summary>
        public Player CurrentPlayer { get; private set; }

        /// <summary>
        /// The level that the player is currently playing.
        /// </summary>
        public Level CurrentLevel { get; private set; }

        public LevelRunner(Player player)
        {
            CurrentPlayer = player;
        }
    }
}
