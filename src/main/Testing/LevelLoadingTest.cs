using System;
using Godot;
using Jumpvalley.Levels;
using Jumpvalley.Players;
using Jumpvalley.Players.Gui;

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
        public SceneTree RootNodeTree;
        public Level CurrentLevel;
        public LevelTimer LevelTimerOperator;
        public Player CurrentPlayer;

        public LevelLoadingTest(string levelPath, Node rootNodeParent, SceneTree rootNodeTree, Node levelTimerGui, Player player)
        {
            LevelDirPath = levelPath;
            RootNodeParent = rootNodeParent;
            RootNodeTree = rootNodeTree;
            LevelTimerOperator = new LevelTimer(levelTimerGui);
            CurrentPlayer = player;
        }

        private void UpdateLevelTimer()
        {
            Level level = CurrentLevel;
            if (level != null)
            {
                LevelTimer timer = LevelTimerOperator;
                if (timer != null)
                {
                    timer.ElapsedTime = level.Clock.ElapsedMilliseconds / 1000d;
                }
            }
        }

        private bool _isProcessStepConnected = false;

        public bool IsProcessStepConnected
        {
            get => _isProcessStepConnected;
            private set
            {
                if (value == _isProcessStepConnected) return;

                _isProcessStepConnected = value;

                if (value)
                {
                    RootNodeTree.ProcessFrame += UpdateLevelTimer;
                }
                else
                {
                    RootNodeTree.ProcessFrame -= UpdateLevelTimer;
                }
            }
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

            CurrentLevel = level;
            level.Runner = new LevelRunner(CurrentPlayer);

            Package.StartLevel();
            RootNodeParent?.AddChild(level.RootNode);

            // This is needed since at the moment, the Level class does not do this by default
            level.Clock.Start();

            LevelInfoFile levelInfo = level.Info;
            Difficulty difficulty = levelInfo.LevelDifficulty;
            Console.WriteLine($"Now playing: {levelInfo.FullName} by {levelInfo.Creators} [{difficulty.Name} - {difficulty.Rating}]");

            IsProcessStepConnected = true;
        }

        public void Dispose()
        {
            IsProcessStepConnected = false;
            Package?.Dispose();
        }
    }
}
