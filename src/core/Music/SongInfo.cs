using System.Text.Json.Nodes;
using Jumpvalley.IO;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Class that holds some metadata about a song file.
    /// </summary>
    public partial class SongInfo : JsonInfoFile
    {
        /// <summary>
        /// The name of the song
        /// </summary>
        public string Name = null;

        /// <summary>
        /// The artists that made the song
        /// </summary>
        public string Artists = null;

        /// <summary>
        /// The album the song belongs to
        /// </summary>
        public string Album = null;

        /// <summary>
        /// The file path to the audio file, including its file name and extension
        /// </summary>
        public string AudioPath = null;

        public SongInfo(JsonNode data) : base(data)
        {
            Name = data["name"]?.GetValue<string>();
            Artists = data["artists"]?.GetValue<string>();
            Album = data["album"]?.GetValue<string>();
            AudioPath = data["audioPath"]?.GetValue<string>();
        }
    }
}
