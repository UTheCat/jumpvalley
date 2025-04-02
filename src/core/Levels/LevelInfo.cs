using System.Text.Json.Nodes;

using UTheCat.Jumpvalley.Core.IO;

namespace UTheCat.Jumpvalley.Core.Levels
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
        /// <br/><br/>
        /// As far as <see cref="LevelInfo"/> is concerned, this is just a number.
        /// It's up to the developer or user to assign meaning to this number.
        /// </summary>
        public double LevelDifficulty;

        /// <summary>
        /// The file path to the scene file that contains the level, including the file name and extension. This path should typically be relative to the level's root folder.
        /// </summary>
        public string ScenePath;

        public LevelInfo() { }

        public LevelInfo(JsonNode data) : base(data)
        {
            Id = data["id"]?.GetValue<string>();
            FullName = data["fullName"]?.GetValue<string>();
            Creators = data["creators"]?.GetValue<string>();
            ScenePath = data["scenePath"]?.GetValue<string>();

            JsonNode difficultyNode = data["difficulty"];
            LevelDifficulty = (difficultyNode == null) ? 0.0 : difficultyNode.GetValue<double>();
        }
    }
}
