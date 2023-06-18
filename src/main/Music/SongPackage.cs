using System.IO;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a folder (such as one on the filesystem) that contains a song and its attribution metadata.
    /// </summary>
    public class SongPackage
    {
        /// <summary>
        /// Data as raw text that comes from "attribution.txt" within the folder
        /// </summary>
        public string AttributionData = "";

        /// <summary>
        /// The song's audio file
        /// </summary>
        public string SongFile;

        /// <summary>
        /// Creates a SongPackage instance for a given folder path
        /// </summary>
        /// <param name="path">The folder path</param>
        public SongPackage(string path)
        {

        }
    }
}
