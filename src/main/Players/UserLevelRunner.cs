using Jumpvalley.Levels;

namespace Jumpvalley.Players
{
    /// <summary>
    /// Jumpvalley's primary level runner that's intended to be running on behalf of the user who's playing the game.
    /// </summary>
    public partial class UserLevelRunner : LevelRunner
    {
        public UserLevelRunner(Player player) : base(player)
        {
            
        }
    }
}