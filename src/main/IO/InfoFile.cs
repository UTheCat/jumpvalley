using System.Collections.Generic;

namespace Jumpvalley.IO
{
    /// <summary>
    /// Reads an Info file, a plain text file named "info.txt" that's primarily for defining metadata for a specific object, such as an audio file or a level.
    /// <br/><br/>
    /// The contents of such types of files should be formatted like this:
    /// <br/>
    /// <c>
    /// property name: property value
    /// </c>
    /// <br/>
    /// where each property is specified on its own line.
    /// <br/>
    /// For the property name, spaces should be replaced with underscores. The property name and property value should be split with a ": ", including the space and not including the quotes.
    /// <br/><br/>
    /// This text formatting is inspired by the way that the content of .osu files are formatted in Osu (the rhythm game).
    /// </summary>
    public partial class InfoFile
    {
        /// <summary>
        /// The full file name of an info file stored somewhere on a filesystem.
        /// </summary>
        public static readonly string FILE_NAME = "info.txt";

        public readonly string PROPERTY_SEPARATOR = "\n";

        public readonly string NAME_VALUE_SEPARATOR = ": ";

        /// <summary>
        /// The content of the info file. The keys of this dictionary are property names, and each key's value is the corresponding property value.
        /// </summary>
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        /// <summary>
        /// Reads an info file from its raw text and stores such data in the <see cref="Data"/> field.
        /// </summary>
        /// <param name="text"></param>
        public InfoFile(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();

                string[] properties = text.Split(PROPERTY_SEPARATOR);
                for (int i = 0; i < properties.Length; i++)
                {
                    string property = properties[i];

                    if (!string.IsNullOrEmpty(property))
                    {
                        string[] propertyComponents = property.Split(NAME_VALUE_SEPARATOR);

                        if (propertyComponents.Length == 2)
                        {
                            Data[propertyComponents[0]] = propertyComponents[1];
                        }
                    }
                }
            }
        }
    }
}
