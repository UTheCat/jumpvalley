using System.Text.Json.Nodes;

using Jumpvalley.IO;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// Class containing some info about a level
    /// </summary>
    public partial class LevelInfo : JsonInfoFile
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

        public LevelInfo() { }

        public LevelInfo(string json) : base(json)
        {
            JsonNode data = JsonNode.Parse(json);

            Id = data["id"]?.GetValue<string>();
            FullName = data["fullName"]?.GetValue<string>();
            Creators = data["creators"]?.GetValue<string>();
            ScenePath = data["scenePath"]?.GetValue<string>();

            JsonNode difficultyNode = data["difficulty"];
            if (difficultyNode == null)
            {
                LevelDifficulty = null;
            }
            else
            {
                // In a level's JSON info file, the difficulty is stored as a number.
                double difficultyRating = difficultyNode.GetValue<double>();

                // We'll need to retain the exact numerical difficulty when setting LevelDifficulty
                Difficulty difficulty = DifficultyPresets.GetPrimaryDifficultyFromRating(difficultyRating);
                LevelDifficulty = new Difficulty(difficulty.Name, difficultyRating, difficulty.Color);
            }
        }
    }
}
