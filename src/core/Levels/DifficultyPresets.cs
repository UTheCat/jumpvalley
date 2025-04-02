using Godot;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.Levels
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
                new Difficulty("Beginner", 0.0, Color.FromHsv(90f / 360f, 0.6f, 1f)),
                new Difficulty("Intermediate", 20.0, Color.FromHsv(60f / 360f, 0.6f, 1f)),
                new Difficulty("Advanced", 40.0, Color.FromHsv(30f / 360f, 0.6f, 1f)),
                new Difficulty("Expert", 60.0, Color.FromHsv(0f / 360f, 0.6f, 1f)),
                new Difficulty("Grandmaster", 80.0, Color.FromHsv(330f / 360f, 0.6f, 1f)),
                new Difficulty("GM+1", 100.0, Color.FromHsv(300f / 360f, 0.6f, 1f)),
                new Difficulty("GM+2", 120.0, Color.FromHsv(270f / 360f, 0.6f, 1f)),
                new Difficulty("GM+3", 140.0, Color.FromHsv(240f / 360f, 0.6f, 1f))
            }
        );
    }
}
