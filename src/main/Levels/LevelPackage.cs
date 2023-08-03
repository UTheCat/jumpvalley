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
        public enum ResourcePackLoadStatus
        {
            /// <summary>
            /// Indicates that the Godot package was loaded successfully.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Indicates that the Godot package under the given file name could not be found.
            /// </summary>
            FileNotFound = 1,

            /// <summary>
            /// Indicates that the Godot package under the given file name was found, but loading it failed.
            /// </summary>
            LoadingFailed = 2
        }

        /// <summary>
        /// The level's info file
        /// </summary>
        public LevelInfoFile Info { get; private set; }

        /// <summary>
        /// The directory path of the level package
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// If the level's root node has been loaded, this will point to it.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The level instance associated with this level
        /// </summary>
        public Level LevelInstance { get; private set; }

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
        /// A status code indicating the results of the attempt to load the resource pack
        /// </returns>
        public ResourcePackLoadStatus TryLoadResourcePack()
        {
            string resourcePackPath = $"{Path}/{Info.ResourcePackName}";
            if (!FileAccess.FileExists(resourcePackPath))
            {
                return ResourcePackLoadStatus.FileNotFound;
            }

            // LoadResourcePack will also only return true if the loading of the resource pack succeeded
            if (ProjectSettings.LoadResourcePack(resourcePackPath, false))
            {
                return ResourcePackLoadStatus.Success;
            }

            return ResourcePackLoadStatus.LoadingFailed;
        }

        /// <summary>
        /// Attempts to load the level's main scene. If successful, <see cref="RootNode"/> will be set to the scene's root node.
        /// </summary>
        public void LoadRootNode()
        {
            if (RootNode == null)
            {
                PackedScene packedScene = GD.Load<PackedScene>($"res://levels/{Info.Id}/{Info.SceneFileName}");

                if (packedScene == null)
                {
                    // TO-DO: return status codes for errors regarding the loading of the level's scene
                    return;
                }

                RootNode = packedScene.Instantiate();
                packedScene.Dispose();
            }
        }

        /// <summary>
        /// Creates a level instance for the current <see cref="RootNode"/> and assigns it to <see cref="LevelInstance"/>
        /// </summary>
        public void CreateLevelInstance()
        {
            if (LevelInstance == null && RootNode != null)
            {
                LevelInstance = new Level(Info, RootNode);
            }
        }
    }
}
