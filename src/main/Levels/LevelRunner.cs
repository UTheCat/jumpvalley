using Jumpvalley.Players;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Class responsible for running and stopping levels.
    /// </summary>
    public partial class LevelRunner
    {
        /// <summary>
        /// The Player instance that this <see cref="LevelRunner"/> is running under.
        /// </summary>
        public Player CurrentPlayer { get; private set; }

        /// <summary>
        /// The level that the player is currently playing.
        /// </summary>
        public Level CurrentLevel;

        /// <summary>
        /// The level that's acting as the game's lobby.
        /// <br/>
        /// <br/>
        /// This is separate from <see cref="CurrentLevel"/> so
        /// both the lobby and the level that the player is currently playing can run at the same time.
        /// </summary>
        public Level Lobby;

        /// <summary>
        /// Creates a new instance of the game's level runner
        /// </summary>
        /// <param name="player">The player instance to run this level runner under</param>
        public LevelRunner(Player player)
        {
            CurrentPlayer = player;
        }
    }
}
