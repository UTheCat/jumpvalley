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
        /// The numerical difficulty ratings assigned to each difficulty name
        /// come from <see href="https://docs.google.com/spreadsheets/d/1KNJ344lsZEmTU9P6eeFInckUbT5dkaWhRcxv_Yy2ZS0">Parrot's Celeste Clears List</see>.
        /// </summary>
        public readonly static List<Difficulty> PRIMARY = new List<Difficulty>
        {
            
        };

        /// <returns>
        /// The difficulty in the <see cref="PRIMARY"/> list associated with the given numerical rating specified in the <paramref name="rating"/> parameter.
        /// </returns>
        public static Difficulty GetPrimaryDifficultyFromRating(double rating)
        {
            return PRIMARY[
                Mathf.FloorToInt(Mathf.Clamp(rating, 0, PRIMARY.Count))
                ];
        }
    }
}
