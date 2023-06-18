using System.IO;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Reads the associated "attribution.txt" file for a song.
    /// <br/>
    /// The contents of such types of files should be formatted like this:
    /// <br/>
    /// <c>
    /// property name: property value
    /// </c>
    /// <br/>
    /// where each property is specified on its own line.
    /// <br/>
    /// Currently, the properties read for attribution.txt are:
    /// <list type="bullet">
    /// <item>name: The name of the song</item>
    /// <item>artists: The artists that made the song</item>
    /// <item>album: The album the song belongs to</item>
    /// </list>
    /// This text formatting is inspired by the way that the content of .osu files are formatted in Osu (the rhythm game).
    /// </summary>
    public partial class AttributionFile
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
        /// Creates an instance of AttributionFile from raw formatted text
        /// </summary>
        /// <param name="text">The raw formatted text</param>
        public AttributionFile(string text)
        {
            SetVariablesFromRawText(text);
        }

        public AttributionFile(FileInfo file)
        {
            using (StreamReader stream = file.OpenText())
            {
                SetVariablesFromRawText(stream.ReadToEnd());
            }
        }

        private void SetVariablesFromRawText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                string[] properties = text.Split("\n");

                for (int i = 0; i < properties.Length; i++)
                {
                    string[] parts = properties[i].Split(": ");
                    if (parts.Length == 2)
                    {
                        string pName = parts[0]; // property name
                        string pValue = parts[1]; // property value
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
                    }
                }
            }
        }
    }
}
