using Jumpvalley.Players;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Subclass of <see cref="LevelRunner"/> that handles running levels located within a directory. 
    /// </summary>
    public partial class DirectoryLevelRunner: LevelRunner
    {
        /// <summary>
        /// File path to the directory containing the levels
        /// </summary>
        public string DirectoryPath { get; private set; }

        public DirectoryLevelRunner(Player player, string directoryPath): base(player)
        {

        }
    }
}
