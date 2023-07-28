using System.IO;
using System.Reflection.Metadata;

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
    /// Currently, the properties read for info.txt are:
    /// <list type="bullet">
    /// <item>name: The name of the song</item>
    /// <item>artists: The artists that made the song</item>
    /// <item>album: The album the song belongs to</item>
    /// <item>file_name: The full name of the file, including its file extension.</item>
    /// </list>
    /// This text formatting is inspired by the way that the content of .osu files are formatted in Osu (the rhythm game).
    /// </summary>
    public partial class SongInfoFile
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
        /// The full name of the file, including its file extension
        /// </summary>
        public string FileName = null;

        /// <summary>
        /// Creates an instance of SongInfoFile from raw formatted text
        /// </summary>
        /// <param name="text">The raw formatted text</param>
        public SongInfoFile(string text)
        {
            SetVariablesFromRawText(text);
        }

        public SongInfoFile(FileInfo file)
        {
            using (StreamReader stream = file.OpenText())
            {
                SetVariablesFromRawText(stream.ReadToEnd());
            }
        }

        private void SetVariablesFromRawText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string[] properties = text.Split("\n");

                for (int i = 0; i < properties.Length; i++)
                {
                    string[] parts = properties[i].Split(": ");
                    if (parts.Length == 2)
                    {
                        string pName = parts[0].Trim(); // property name
                        string pValue = parts[1].Trim(); // property value
                        if (pName.Equals("name"))
                        {
                            Name = pValue;
                        }
                        else if (pName.Equals("artists"))
                        {
                            Artists = pValue;
                        }
                        else if (pName.Equals("album"))
                        {
                            Album = pValue;
                        }
                        else if (pName.Equals("file_name"))
                        {
                            FileName = pValue;
                        }    
                    }
                }
            }
        }
    }
}
