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
        /// Jumpvalley's main set of difficulties. These are taken directly from Juke's Towers of Hell.
        /// <br/>
        /// Note that Jumpvalley may use a different set of difficulty ratings in the future.
        /// </summary>
        public readonly static List<Difficulty> PRIMARY = new List<Difficulty>
        {
            // Remember that when setting this, make sure that each difficulty's list index matches with the difficulty's numerical rating rounded down
            new Difficulty("Effortless", 0, Color.Color8(0, 206, 0)),
            new Difficulty("Easy", 1, Color.Color8(117, 243, 71)),
            new Difficulty("Medium", 2, Color.Color8(0, 206, 0)),
            new Difficulty("Hard", 3, Color.Color8(253, 124, 0)),
            new Difficulty("Difficult", 4, Color.Color8(255, 12, 4)),
            new Difficulty("Challenging", 5, Color.Color8(193, 0, 0)),
            new Difficulty("Intense", 6, Color.Color8(25, 40, 50)),
            new Difficulty("Remorseless", 7, Color.Color8(200, 0, 200)),
            new Difficulty("Insane", 8, Color.Color8(0, 0, 255)),
            new Difficulty("Extreme", 9, Color.Color8(3, 137, 255)),
            new Difficulty("Terrifying", 10, Color.Color8(0, 255, 255)),
            new Difficulty("Catastrophic", 11, Color.Color8(255, 255, 255)),

            // difficulties beyond sane limits
            new Difficulty("Horrific", 12, Color.Color8(140, 139, 238)),
            new Difficulty("Unreal", 13, Color.Color8(81, 0, 203))
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
