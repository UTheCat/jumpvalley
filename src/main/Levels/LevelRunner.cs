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

        public LevelRunner(Player player)
        {
            CurrentPlayer = player;
        }
    }
}
