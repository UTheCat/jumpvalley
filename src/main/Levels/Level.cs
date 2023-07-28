﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level that's playable in Jumpvalley.
    /// <br/>
    /// Each level contains three primary components: the interactives, the music, and the static objects.
    /// More details can be found on Jumpvalley's wiki article on <see href="https://github.com/UTheDev/jumpvalley/wiki/Level-Layout">Level Layout</see>.
    /// </summary>
    public partial class Level: IDisposable
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";

        /// <summary>
        /// The full name of the level. This is the name of the level that will actually be displayed to the user, and it can be different from the name of the level's root node.
        /// </summary>
        public string FullName = "";

        /// <summary>
        /// How difficult the level is.
        /// </summary>
        public Difficulty LevelDifficulty;

        /// <summary>
        /// The node containing the level's interactives
        /// </summary>
        public Node Interactives { get; private set; }

        /// <summary>
        /// The node containing the level's music
        /// </summary>
        public Node Music { get; private set; }

        /// <summary>
        /// The node containing the level's static objects
        /// </summary>
        public Node StaticObjects { get; private set; }

        /// <summary>
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its root node
        /// </summary>
        /// <param name="node">The root node of the level to represent</param>
        public Level(Node node)
        {
            FullName = node.GetMeta("full_name").AsString();

            // We'll need to retain the exact numerical difficulty
            double difficultyRating = node.GetMeta("difficulty").AsDouble();
            Difficulty difficulty = DifficultyPresets.GetPrimaryDifficultyFromRating(difficultyRating);
            LevelDifficulty = new Difficulty(difficulty.Name, difficultyRating, difficulty.Color);

            Interactives = node.GetNode(INTERACTIVES_NODE_NAME);
            Music = node.GetNode(MUSIC_NODE_NAME);
            StaticObjects = node.GetNode(STATIC_OBJECTS_NODE_NAME);
        }

        /// <summary>
        /// Level initialization method that initializes the level after the constructor runs, in case such method is needed.
        /// <br/>
        /// By default, this method is only called once after the object's constructor runs.
        /// Initialize() typically shouldn't be called more than once for the same <see cref="Level"/> instance.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// The level's start method. This method is called every time the user starts or restarts the level,
        /// and it's a great place to put code that will be run after initialization, but just before the level starts.
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// The level's stop method. This method is called right after the user stops or exits the level.
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// Disposes of this <see cref="Level"/> instance. This method is a great place to free up resources being used by the level instance,
        /// especially right before the level itself gets freed from memory.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}