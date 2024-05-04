using System.Text.Json.Nodes;

namespace Jumpvalley.IO
{
    /// <summary>
    /// Class intended to help with using the data from info files written in JavaScript Object Notation (JSON).
    /// <br/><br/>
    /// Info files are files that contain metadata about an object (e.g. one that's on a filesystem).
    /// </summary>
    public partial class JsonInfoFile
    {
        /// <summary>
        /// The full file name of a JSON info file stored somewhere on a filesystem, including the file extension.
        /// </summary>
        public static readonly string FILE_NAME = "info.json";

        /// <summary>
        /// The JSON node containing the info file's data.
        /// This is the root node of the info file's JSON content.
        /// </summary>
        public JsonNode Data;

        /// <summary>
        /// Zero-parameter constructor for cases where extracting data from JSON text isn't needed.
        /// </summary>
        public JsonInfoFile() { }

        /// <summary>
        /// Reads a JSON info file from its raw text and stores such data in the <see cref="RawData"/> field.
        /// </summary>
        /// <param name="data">The root node of the info file's JSON content</param>
        public JsonInfoFile(JsonNode data)
        {
            Data = data;
        }
    }
}
