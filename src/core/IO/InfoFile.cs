using System;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.Core.IO
{
    /// <summary>
    /// Reads an Info file, a blob of text that's primarily for defining metadata for a specific object, such as an audio file or a level.
    /// The blob of text typically comes from a plain-text file named "info.txt".
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
    [Obsolete]
    public partial class InfoFile
    {
        /// <summary>
        /// The full file name of an info file stored somewhere on a filesystem, including the file extension.
        /// </summary>
        public static readonly string FILE_NAME = "info.txt";

        /// <summary>
        /// The string that separates two properties with the info file's text.
        /// </summary>
        public static readonly string PROPERTY_SEPARATOR = "\n";

        /// <summary>
        /// The string that separates a property's name from its value.
        /// </summary>
        public static readonly string NAME_VALUE_SEPARATOR = ": ";

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
                        property = property.Trim();

                        int nameValueSeparatorIndex = property.IndexOf(NAME_VALUE_SEPARATOR);

                        // Make sure NAME_VALUE_SEPARATOR is actually in the property string
                        if (nameValueSeparatorIndex != -1)
                        {
                            int propertyValueCharIndex = nameValueSeparatorIndex + NAME_VALUE_SEPARATOR.Length;

                            // Make sure there's content that comes before and after NAME_VALUE_SEPARATOR
                            if (nameValueSeparatorIndex > 0 && propertyValueCharIndex < property.Length)
                            {
                                // The part before NAME_VALUE_SEPARATOR should be the property name
                                // and the part after NAME_VALUE_SEPARATOR should be the property value
                                Data[property.Substring(0, nameValueSeparatorIndex)] = property.Substring(propertyValueCharIndex);
                            }
                        }
                    }
                }
            }
        }
    }
}
