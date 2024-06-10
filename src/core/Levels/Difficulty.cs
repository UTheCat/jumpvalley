using Godot;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Represents a difficulty, a set of values that judge how difficult a level is.
    /// </summary>
    public partial class Difficulty
    {
        /// <summary>
        /// The name of the difficulty
        /// </summary>
        public string Name;

        /// <summary>
        /// The difficulty's numerical measurement.
        /// <br/><br/>
        /// For example, a level with a difficulty rating of 23.5
        /// would be yellow-intermediate if you were using
        /// the <see href="https://www.celestegame.com/">Celeste</see> difficulty rating system
        /// (specifically, the system found in the Difficulty Reference Chart in
        /// <see href="https://docs.google.com/spreadsheets/d/1KNJ344lsZEmTU9P6eeFInckUbT5dkaWhRcxv_Yy2ZS0">Parrot's Celeste Clears List</see>).
        /// </summary>
        public double Rating;

        /// <summary>
        /// The color that represents the difficulty.
        /// </summary>
        public Color Color;

        /// <summary>
        /// Creates a new difficulty with the given name and numerical rating
        /// </summary>
        /// <param name="name">The difficulty's name</param>
        /// <param name="rating">The difficulty's numerical rating</param>
        public Difficulty(string name, double rating, Color color)
        {
            Name = name;
            Rating = rating;
            Color = color;
        }

        public Difficulty() : this("", 0.0, new Color(0.5f, 0.5f, 0.5f)) { }
    }
}
