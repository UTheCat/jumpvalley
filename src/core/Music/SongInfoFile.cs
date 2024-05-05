namespace Jumpvalley.Music
{
    /// <summary>
    /// Reads the associated "info.txt" file for a song.
    /// <br/>
    /// The contents of such types of files should be formatted like this:
    /// <br/>
    /// <c>
    /// property name: property value
    /// </c>
    /// <br/>
    /// where each property is specified on its own line.
    /// <br/>
    /// Currently, the properties read for an song's info.txt file are:
    /// <list type="bullet">
    /// <item>name: The name of the song</item>
    /// <item>artists: The artists that made the song</item>
    /// <item>album: The album the song belongs to</item>
    /// <item>audio_path: The file path to the audio file, including its file name and extension</item>
    /// </list>
    /// This text formatting is inspired by the way that the content of .osu files are formatted in Osu (the rhythm game).
    /// </summary>
    [System.Obsolete]
    public partial class SongInfoFile: IO.InfoFile
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

        /// <summary>
        /// Creates an instance of SongInfoFile from raw formatted text
        /// </summary>
        /// <param name="text">The raw formatted text</param>
        public SongInfoFile(string text): base(text)
        {
            // This works since C# calls the base constructor first
            // See this article for more details on constructor order:
            // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/constructors
            Data.TryGetValue("name", out Name);
            Data.TryGetValue("artists", out Artists);
            Data.TryGetValue("album", out Album);
            Data.TryGetValue("audio_path", out AudioPath);
        }
    }
}
    