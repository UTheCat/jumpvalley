using System.Collections.Generic;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Class that groups multiple difficulties together.
    /// Useful when you want to have all the difficulties within a game
    /// in one place and be able to perform some calculations based off of them.
    /// </summary>
    public partial class DifficultySet
    {
        /// <summary>
        /// The list of difficulties within the set
        /// </summary>
        public List<Difficulty> Difficulties;

        /// <summary>
        /// Returns the difficulty within the <see cref="Difficulties"/> list
        /// who has the highest difficulty rating less than or equal to the
        /// difficulty rating specified in the <paramref name="rating"/> parameter.
        /// </summary>
        /// <param name="rating">The difficulty rating</param>
        /// <returns>The corresponding difficulty</returns>
        public Difficulty GetDifficultyByRating(double rating)
        {
            return null;
        }
    }
}
