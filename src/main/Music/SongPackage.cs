//using System;
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
        /// The name of the song's audio file
        /// </summary>
        public string SongFile;

        /// <summary>
        /// Creates a SongPackage instance for a given folder path
        /// </summary>
        /// <param name="path">The folder path</param>
        public SongPackage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // Some like to end the paths with an extra "/" or "\". Account for this.
                if (!(path.EndsWith("/") || path.EndsWith("\\")))
                {
                    path += "/";
                }

                // Try to read the contents of the attribution file in the package.
                StreamReader attributionStreamReader = null;
                try
                {
                    using (attributionStreamReader = new StreamReader(path + "attribution.txt"))
                    {
                        AttributionData = attributionStreamReader.ReadToEnd();
                    }
                }
                catch (System.Exception e)
                {
                    // Attribution files can be excluded from a song package although it's recommended that the user put one anyway.
                    if ((e is FileNotFoundException))
                    {
                        if (attributionStreamReader != null)
                        {
                            attributionStreamReader.Dispose();
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
