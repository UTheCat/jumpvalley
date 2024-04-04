using System.IO;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Reads an info file for a Jumpvalley level
    /// </summary>
    public partial class LevelInfoFile : IO.InfoFile
    {
        /// <summary>
        /// The level's internal identifier. For example, you can make the identifier a shortened version of the full name.
        /// <br/>
        /// In order for level loading to work properly, this ID must match the directory name of the level within the resource filesystem.
        /// This is due to the current limitations of resource pack loading in Godot.
        /// <br/>
        /// For instance, if the level's ID is "demo", the contents of the level within the levels folder in the resource filesystem must be located at <c>res://levels/demo</c>
        /// </summary>
        public string Id;

        /// <summary>
        /// The full name of the level. This is the name of the level that will actually be displayed to the user, and it can be different from the name of the level's root node.
        /// </summary>
        public string FullName;

        /// <summary>
        /// The creators of the level.
        /// </summary>
        public string Creators;

        /// <summary>
        /// How difficult the level is.
        /// </summary>
        public Difficulty LevelDifficulty;

        /// <summary>
        /// The file path to the scene file that contains the level, including the file name and extension. This path should typically be relative to the level's root folder.
        /// </summary>
        public string ScenePath;

        public LevelInfoFile(string text) : base(text)
        {
            Data.TryGetValue("id", out Id);
            Data.TryGetValue("full_name", out FullName);
            Data.TryGetValue("creators", out Creators);
            Data.TryGetValue("scene_path", out ScenePath);

            string difficultyText;

            // If TryGetValue failed, it will return false
            if (!Data.TryGetValue("difficulty", out difficultyText))
            {
                throw new InvalidDataException("The level's info file doesn't specify the difficulty of the level, or the info file is improperly formatted.");
            }

            double difficultyRating;

            // If double.TryParse failed, it will return false
            if (!double.TryParse(difficultyText, out difficultyRating))
            {
                throw new InvalidDataException("Difficulty specified in the level's info file isn't a number.");
            }

            // We'll need to retain the exact numerical difficulty when setting LevelDifficulty
            Difficulty difficulty = DifficultyPresets.GetPrimaryDifficultyFromRating(difficultyRating);
            LevelDifficulty = new Difficulty(difficulty.Name, difficultyRating, difficulty.Color);
        }
    }
}
