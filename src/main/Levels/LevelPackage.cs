using Godot;
using Jumpvalley.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level package, a filesystem folder that contains the components of a level.
    /// <br/>
    /// Each level package includes:
    /// <list type="bullet">
    /// <item>The level's info file</item>
    /// <item>The Godot package (PCK file) that contains the level's contents</item>
    /// </list>
    /// </summary>
    public partial class LevelPackage
    {
        /// <summary>
        /// The level's info file
        /// </summary>
        public LevelInfoFile Info { get; private set; }

        /// <summary>
        /// The directory path of the level package
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Constructs a LevelPackage object for a given level package's directory
        /// </summary>
        /// <param name="path">The directory path of the level package</param>
        public LevelPackage(string path)
        {
            Path = path;

            using FileAccess infoFile = FileAccess.Open($"{path}/{InfoFile.FILE_NAME}", FileAccess.ModeFlags.Read);
            if (infoFile == null)
            {
                throw new Exception($"Failed to open the corresponding {InfoFile.FILE_NAME} file. This is the message returned by FileAccess.GetOpenError(): {FileAccess.GetOpenError()}");
            }

            Info = new LevelInfoFile(infoFile.GetAsText());
        }

        /// <summary>
        /// Attempts to load the resource pack specified within the level's info file if it exists
        /// </summary>
        /// <returns>
        /// <c>true</c> if the loading of the resource pack succeeded and <c>false</c> otherwise
        /// </returns>
        public bool TryLoadResourcePack()
        {
            string resourcePackPath = $"{Path}/{Info.ResourcePackName}";
            if (FileAccess.FileExists(resourcePackPath))
            {
                // LoadResourcePack will also only return true if the loading of the resource pack succeeded
                return ProjectSettings.LoadResourcePack(resourcePackPath, false);
            }

            return false;
        }
    }
}
