using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Jumpvalley.Levels;

namespace Jumpvalley.Testing
{
    /// <summary>
    /// Testing of the level loading prototype
    /// </summary>
    public partial class LevelLoadingTest: BaseTest, IDisposable
    {
        public string LevelDirPath;
        public LevelPackage Package;
        public Node RootNodeParent;

        public LevelLoadingTest(string levelPath, Node rootNodeParent)
        {
            LevelDirPath = levelPath;
            RootNodeParent = rootNodeParent;
        }

        public override void Start()
        {
            base.Start();

            Package = new LevelPackage(LevelDirPath);
            Package.TryLoadResourcePack();
            Package.LoadRootNode();
            Package.CreateLevelInstance();

            Level level = Package.LevelInstance;

            if (level == null)
            {
                throw new Exception("Level loading failed");
            }

            level.Start();
            RootNodeParent?.AddChild(level.RootNode);
        }

        public void Dispose()
        {
            Package?.Dispose();
        }
    }
}
