using Godot;
using System.Collections.Generic;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class contains some difficulty sets that can be used to label the difficulty of a level.
    /// <br/><br/>
    /// Currently, the difficulty sets here have one of these characteristics:
    /// <list type="bullet">
    /// <item>
    /// <description>It's directly from a game or community</description>
    /// </item>
    /// <item>
    /// <description>It's from a game or community, but it's been modified.</description>
    /// </item>
    /// </list>
    /// </summary>
    public partial class DifficultyPresets
    {
        /// <summary>
        /// Failsafe difficulty to default to, just in case
        /// </summary>
        public readonly static Difficulty FALLBACK = new Difficulty();

        /// <summary>
        /// Jumpvalley's main set of difficulties.
        /// <br/><br/>
        /// The names of difficulties in this set come from the <see href="https://www.celestegame.com/">Celeste</see> community.
        /// </summary>
        public readonly static DifficultySet PRIMARY_DIFFICULTIES = new DifficultySet(
            new List<Difficulty>()
            {
                new Difficulty("Beginner", 0.0, new Color(231f / 255f, 0.41f, 1f)),
                new Difficulty("Intermediate", 15.0, new Color(198f / 255f, 0.44f, 1f)),
                new Difficulty("Advanced", 30.0, new Color(140f / 255f, 0.47f, 0.88f)),
                new Difficulty("Expert", 45.0, new Color(95f / 255f, 0.53f, 1f)),
                new Difficulty("Grandmaster", 60.0, new Color(51f / 255f, 0.47f, 1f)),
                new Difficulty("GM+1", 80.0, new Color(36f / 255f, 0.55f, 1f)),
                new Difficulty("GM+2", 90.0, new Color(8f / 255f, 0.6f, 1f)),
                new Difficulty("GM+3", 95.0, new Color(332f / 255f, 0.92f, 1f)),
            }
        );
    }
}
