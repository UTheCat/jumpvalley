using Godot;
using Jumpvalley.IO;
using System;

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
    public partial class LevelPackage : IDisposable
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
        /// If the level's root node has been loaded, this will point to it.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The level instance associated with this level
        /// </summary>
        public Level LevelInstance { get; private set; }

        /// <summary>
        /// The level runner that's running this level package.
        /// </summary>
        public LevelRunner Runner;

        /// <summary>
        /// Constructs a LevelPackage object for a given level package's directory
        /// </summary>
        /// <param name="path">The directory path of the level package</param>
        /// <param name="runner">The level runner that's running this level package</param>
        public LevelPackage(string path, LevelRunner runner)
        {
            Path = path;

            using FileAccess infoFile = FileAccess.Open($"{path}/{InfoFile.FILE_NAME}", FileAccess.ModeFlags.Read);
            if (infoFile == null)
            {
                throw new Exception($"Failed to open the corresponding {InfoFile.FILE_NAME} file. This is the message returned by FileAccess.GetOpenError(): {FileAccess.GetOpenError()}");
            }

            Info = new LevelInfoFile(infoFile.GetAsText());
            Runner = runner;
        }

        /// <summary>
        /// Attempts to load the level's main scene.
        /// If this operation is successful, <see cref="RootNode"/> will be set to the scene's root node, and this method will return true.
        /// Otherwise, this method will return false.
        /// </summary>
        public bool LoadRootNode()
        {
            if (RootNode != null) throw new InvalidOperationException("There's already a root node loaded. Please unload it first.");

            PackedScene packedScene = GD.Load<PackedScene>($"{Path}/{Info.ScenePath}");

            if (packedScene == null)
            {
                // TO-DO: return status codes for errors regarding the loading of the level's scene
                return false;
            }

            RootNode = packedScene.Instantiate();
            packedScene.Dispose();

            return true;
        }

        /// <summary>
        /// Unloads and disposes of the level's root node.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when LevelInstance isn't null.
        /// This is to prevent errors that can occur when the root node is disposed while the level is still active.
        /// </exception>
        public void DisposeRootNode()
        {
            if (LevelInstance != null) throw new InvalidOperationException("Root node cannot be disposed while LevelInstance is not null.");

            if (RootNode != null)
            {
                RootNode.QueueFree();
                RootNode.Dispose();
                RootNode = null;
            }
        }

        /// <summary>
        /// Creates a level instance for the current <see cref="RootNode"/> and assigns it to <see cref="LevelInstance"/>
        /// </summary>
        public void CreateLevelInstance()
        {
            if (LevelInstance == null && RootNode != null)
            {
                Level level = new Level(Info, RootNode, TimeSpan.Zero);
                level.Package = this;
                level.Runner = Runner;

                LevelInstance = level;
            }
        }

        /// <summary>
        /// Disposes of <see cref="LevelInstance"/> if it's not null.
        /// </summary>
        public void DisposeLevelInstance()
        {
            if (LevelInstance != null)
            {
                LevelInstance.Dispose();
                LevelInstance = null;
            }
        }

        /// <summary>
        /// Initializes the level assigned to <see cref="LevelInstance"/> (if that hasn't been done yet), and then starts it.
        /// </summary>
        public void StartLevel()
        {
            if (LevelInstance != null)
            {
                if (!LevelInstance.IsInitialized)
                {
                    LevelInstance.Initialize();
                }

                LevelInstance.Start();
            }
        }

        /// <summary>
        /// Stops the level assigned to <see cref="LevelInstance"/>
        /// </summary>
        public void StopLevel()
        {
            LevelInstance?.Stop();
        }

        /// <summary>
        /// Disposes of this level package, including the level instance associated with it.
        /// </summary>
        public void Dispose()
        {
            StopLevel();
            DisposeLevelInstance();
            DisposeRootNode();
        }
    }
}
