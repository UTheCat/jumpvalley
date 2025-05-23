﻿using Godot;

namespace UTheCat.Jumpvalley.Core.Levels
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
