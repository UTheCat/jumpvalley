using Godot;
using System.Collections.Generic;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Difficulty presets taken from difficulty-rating systems already established by various communities
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
        /// These come from the <see href="https://www.celestegame.com/">Celeste</see> community.
        /// The numerical difficulty ratings assigned to each difficulty name come from
        /// the Difficulty Reference Chart in <see href="https://docs.google.com/spreadsheets/d/1KNJ344lsZEmTU9P6eeFInckUbT5dkaWhRcxv_Yy2ZS0">Parrot's Celeste Clears List</see>.
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
