namespace UTheCat.Jumpvalley.Core.IO
{
    /// <summary>
    /// Provides some values and methods for performing operations related to file paths.
    /// </summary>
    public partial class PathUtil
    {
        /// <summary>
        /// The conventional directory separator on Windows,
        /// which is a backslash.
        /// </summary>
        public static readonly char WINDOWS_DIR_SEPARATOR = '\\';

        /// <summary>
        /// The conventional directory separator on Godot,
        /// which is a forward slash.
        /// </summary>
        public static readonly char GODOT_DIR_SEPARATOR = '/';

        /// <summary>
        /// Concatenates multiple paths into one using
        /// Godot's conventional file path format 
        /// (where the directory separator is a forward slash).
        /// </summary>
        /// <param name="paths">The file paths to combine</param>
        /// <returns>
        /// A file path that follows Godot's conventional file path format.
        /// </returns>
        public static string GodotCombine(params string[] paths)
        {
            string finalPath = "";

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];

                path.Replace(WINDOWS_DIR_SEPARATOR, GODOT_DIR_SEPARATOR);
                if (i < paths.Length - 1 && path.EndsWith(GODOT_DIR_SEPARATOR) == false)
                {
                    path += GODOT_DIR_SEPARATOR;
                }

                finalPath += path;
            }

            return finalPath;
        }
    }
}
