using Godot;

using UTheCat.Jumpvalley.Core.Music;

namespace UTheCat.Jumpvalley.App.Testing
{
    /// <summary>
    /// Tests for loading a song via song package
    /// </summary>
    public partial class SongPackageTest: BaseTest
    {
        private string[] dirList;
        private SceneTree tree;

        public MusicPlayer MusicPlayer { get; private set; }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="sceneTree">The Godot scene tree to play music in</param>
        /// <param name="directoryList">The list of directories to play</param>
        public SongPackageTest(SceneTree sceneTree, string[] directoryList) {
            dirList = directoryList;
            tree = sceneTree;
        }

        public override void Start()
        {

        }
    }
}
