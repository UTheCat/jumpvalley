using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.Levels
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
        /// Creates a new instance of <see cref="DifficultySet"/> with an initial list of difficulties. 
        /// </summary>
        /// <param name="difficulties">The initial list of difficulties</param>
        public DifficultySet(List<Difficulty> difficulties)
        {
            Difficulties = difficulties;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DifficultySet"/> with a blank list of difficulties.
        /// </summary>
        public DifficultySet() : this(new List<Difficulty>()) { }

        /// <summary>
        /// Returns the difficulty within the <see cref="Difficulties"/> list
        /// who has the highest difficulty rating less than or equal to the
        /// difficulty rating specified in the <paramref name="rating"/> parameter.
        /// </summary>
        /// <param name="rating">The difficulty rating</param>
        /// <returns>The corresponding difficulty</returns>
        public Difficulty GetDifficultyByRating(double rating)
        {
            Difficulty difficulty = null;
            double correspondingRating = 0;
            bool firstRatingReached = false;

            foreach (Difficulty d in Difficulties)
            {
                double diffRating = d.Rating;

                if ((firstRatingReached == false || diffRating > correspondingRating) && diffRating <= rating)
                {
                    firstRatingReached = true;
                    correspondingRating = diffRating;
                    difficulty = d;
                }
            }

            return difficulty;
        }
    }
}
