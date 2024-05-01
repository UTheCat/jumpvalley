using System.Text.Json;

namespace Jumpvalley.IO
{
    /// <summary>
    /// Class that handles info files written in JavaScript Object Notation (JSON).
    /// <br/><br/>
    /// Info files are files that contain metadata about an object (e.g. one that's on a filesystem).
    /// </summary>
    public partial class JsonInfoFile
    {
        /// <summary>
        /// The full file name of a JSON info file stored somewhere on a filesystem, including the file extension.
        /// </summary>
        public static readonly string FILE_NAME = "info.json";

        public JsonInfoFile(string json)
        {

        }
    }
}
