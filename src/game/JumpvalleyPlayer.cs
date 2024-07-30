using Godot;
using System;
using System.Text.Json.Nodes;

using Jumpvalley.Gui;
using Jumpvalley.Levels;
using Jumpvalley.Logging;
using Jumpvalley.Music;
using Jumpvalley.Players;
using Jumpvalley.Players.Movement;

using JumpvalleyGame.Gui;
using JumpvalleyGame.Levels;
using JumpvalleyGame.Settings;
using JumpvalleyGame.Testing;
using Jumpvalley.Levels.Interactives.Mechanics;

namespace JumpvalleyGame
{
    /// <summary>
    /// Full implementation of the Player class specific to Jumpvalley itself
    /// </summary>
    public partial class JumpvalleyPlayer : Player, IDisposable
    {
        private static readonly string INITIALIZATION_LEVEL_META_NAME = "initialization_level";
        private static readonly string INITIALIZATION_LOBBY_META_NAME = "initialization_lobby";

        private ConsoleLogger logger;
        private JumpvalleySettings settings;

        public JumpvalleyPlayer(SceneTree tree, Node rootNode) : base(tree, rootNode)
        {
            logger = new ConsoleLogger(nameof(JumpvalleyPlayer));

            settings = new JumpvalleySettings();
        }

        public override void Start()
        {
            base.Start();

            // Handle music that's played in the main scene file
            Node rootNodeMusic = RootNode.GetNode("Music");
            MusicGroup primaryMusic = new MusicGroup(rootNodeMusic.GetNode("PrimaryMusic"));
            Node primaryMusicZones = rootNodeMusic.GetNode("MusicZones");

            CurrentMusicPlayer.BindedNode = Character;
            CurrentMusicPlayer.TransitionTime = 3;
            CurrentMusicPlayer.OverrideTransitionTime = true;
            CurrentMusicPlayer.VolumeScale = 1;
            CurrentMusicPlayer.OverrideLocalVolumeScale = true;
            CurrentMusicPlayer.OverrideSongStreamHandlingMode = true;

            CurrentMusicPlayer.AddPlaylist(primaryMusic);
            CurrentMusicPlayer.PrimaryPlaylist = primaryMusic;

            Disposables.Add(primaryMusic);

            foreach (Node zone in primaryMusicZones.GetChildren())
            {
                MusicZone musicZone = new MusicZone(zone);
                CurrentMusicPlayer.Add(musicZone);
                Disposables.Add(musicZone);
            }

            //CurrentMusicPlayer.IsPlaying = true;

            // Character bounding box
            OverallBoundingBoxObject characterBoundingBox = new OverallBoundingBoxObject(Clock, Character.GetNode("_InteractiveBoundingBox"));
            Disposables.Add(characterBoundingBox);

            // Set up character movement
            // Some values here are based on Juke's Towers of Hell physics (or somewhere close), except we're working with meters.
            // In-game gravity can be changed at runtime, so we need to account for that. See:
            // https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-physics-3d-default-gravity
            // for more details.
            Mover.Gravity = PhysicsServer3D.AreaGetParam(RootNode.GetViewport().FindWorld3D().Space, PhysicsServer3D.AreaParameter.Gravity).As<float>();
            Mover.JumpVelocity = 25f;
            Mover.Speed = 8f;
            Mover.Acceleration = 180f;
            Mover.AirAcceleration = 90f;
            Mover.Deceleration = 180f;
            Mover.AirDeceleration = 90f;

            Mover.Body = Character;

            // Set up the player's camera. This is done in between movement setup steps because
            // some of the movement stuff depends on the state of the player's camera.
            Camera.FocusedNode = Character.GetNode<Node3D>("CameraFocus");
            Camera.Camera = RootNode.GetNode<Camera3D>("Camera");
            Camera.PanningSensitivity = 1;
            Camera.PanningSpeed = (float)(0.2 * Math.PI) * 0.33f;
            Camera.MinPitch = (float)(-0.45 * Math.PI);
            Camera.MaxPitch = (float)(0.45 * Math.PI);

            Camera.MaxZoomOutDistance = 15f;
            Camera.ZoomOutDistance = 5f;

            Mover.Camera = Camera;

            // Set up shift-lock
            RotationLockControl rotationLockControl = new RotationLockControl(Mover, Camera);
            RootNode.AddChild(rotationLockControl);

            RootNode.AddChild(Mover);
            RootNode.AddChild(Camera);

            // Input with higher accuracy and less lag is preferred in Juke's Towers of Hell fangames,
            // since very small differences in input can have a large impact on gameplay.
            // This is why it's important to make the input refresh rate independent from display refresh rate.
            Input.UseAccumulatedInput = false;

            RenderFramerateLimiter fpsLimiter = new RenderFramerateLimiter();
            fpsLimiter.MinFpsDifference = 0;
            fpsLimiter.IsRunning = true;
            RootNode.AddChild(fpsLimiter);

            // Initialize GUI stuff
            BackgroundPanel bgPanel = new BackgroundPanel(PrimaryGui.GetNode<Control>("BackgroundPanel"));

            // Primary AnimatedNodeGroup
            BgPanelAnimatedNodeGroup animatedNodes = new BgPanelAnimatedNodeGroup(bgPanel)
            {
                CanOnlyShowOneNode = true
            };

            // Bottom bar
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), CurrentMusicPlayer)
            {
                AnimatedNodes = animatedNodes
            };

            // Level menu
            Control primaryLevelMenuNode = PrimaryGui.GetNode<Control>("PrimaryLevelMenu");
            PrimaryLevelMenu primaryLevelMenu = new PrimaryLevelMenu(primaryLevelMenuNode, Tree);
            animatedNodes.Add("primary_level_menu", primaryLevelMenu);

            Disposables.Add(primaryLevelMenu);
            Disposables.Add(primaryLevelMenuNode);

            // Music panel
            Control musicPanelNode = PrimaryGui.GetNode<Control>("MusicPanel");

            MusicPanel musicPanel = new MusicPanel(CurrentMusicPlayer, musicPanelNode, Tree);
            animatedNodes.Add("music_panel", musicPanel);

            //bottomBar.PrimaryMusicPanel = musicPanel;

            // Settings menu
            Control settingsMenuNode = PrimaryGui.GetNode<Control>("SettingsMenu");

            SettingsMenu settingsMenu = new SettingsMenu(settingsMenuNode, Tree, settings.Group);
            settingsMenu.Populate();
            animatedNodes.Add("settings_menu", settingsMenu);

            if (primaryLevelMenu != null)
            {
                primaryLevelMenu.CurrentSettingsMenu = settingsMenu;
            }

            Disposables.Add(settingsMenu);
            Disposables.Add(settingsMenuNode);

            // Apply saved settings configuration
            SettingsFile settingsFile = settings.File;
            settingsFile.Read();

            JsonNode node = settingsFile.Data;
            if (node != null)
            {
                settings.Group.ApplyJson(node.AsObject());
            }

            // Set up level-running stuff
            UserLevelRunner levelRunner = new UserLevelRunner(this, new LevelTimer(PrimaryGui.GetNode("LevelTimer")));
            RootNode.AddChild(levelRunner);

            // Load the initialization lobby (the lobby we want to load in when the game starts)
            if (RootNode.HasMeta(INITIALIZATION_LOBBY_META_NAME))
            {
                string lobbyPath = RootNode.GetMeta(INITIALIZATION_LOBBY_META_NAME).As<string>();
                if (!string.IsNullOrEmpty(lobbyPath))
                {
                    LevelPackage lobby = new LevelPackage(lobbyPath, levelRunner);
                    levelRunner.Lobby = lobby;
                    Disposables.Add(lobby);
                    lobby.LoadRootNode();
                    lobby.CreateLevelInstance();
                    lobby.StartLevel();
                    RootNode.AddChild(lobby.RootNode);

                    Level lobbyLevel = lobby.LevelInstance;
                    lobbyLevel.SendPlayerToCurrentCheckpoint();

                    MusicGroup lobbyPrimaryPlaylist = lobbyLevel.PrimaryPlaylist;
                    if (lobbyPrimaryPlaylist != null)
                    {
                        CurrentMusicPlayer.PrimaryPlaylist = lobbyPrimaryPlaylist;
                    }
                }
            }

            // Load the initialization level (the level we want to load in when the game starts)
            string levelsNodeName = "Levels";
            Node levelsNode = RootNode.GetNode(levelsNodeName);
            if (levelsNode != null)
            {
                if (levelsNode.HasMeta(INITIALIZATION_LEVEL_META_NAME))
                {
                    string levelPath = levelsNode.GetMeta(INITIALIZATION_LEVEL_META_NAME).As<string>();

                    if (!string.IsNullOrEmpty(levelPath))
                    {
                        LevelPackage levelPackage = new LevelPackage(levelPath, levelRunner);
                        levelRunner.CurrentLevelPackage = levelPackage;
                        Disposables.Add(levelPackage);
                        levelPackage.LoadRootNode();
                        levelPackage.CreateLevelInstance();
                        levelPackage.StartLevel();
                        levelsNode.AddChild(levelPackage.RootNode);

                        Level levelInstance = levelPackage.LevelInstance;
                        levelInstance.SendPlayerToCurrentCheckpoint();

                        MusicGroup levelPrimaryPlaylist = levelInstance.PrimaryPlaylist;
                        if (levelPrimaryPlaylist != null)
                        {
                            CurrentMusicPlayer.PrimaryPlaylist = levelPrimaryPlaylist;
                        }

                        LevelInfo levelInfo = levelPackage.Info;
                        Difficulty difficulty = levelInfo.LevelDifficulty;
                        logger.Print($"Now playing a level: {levelInfo.FullName} by {levelInfo.Creators} [{difficulty.Name} - {difficulty.Rating}]");
                    }
                }
            }
            else
            {
                logger.Print($"Failed to load a level at game initialization. The root node of the main scene is missing a node named '{levelsNodeName}'.");
            }

            // Start playing music.
            // This is done after we load the lobby and the initialization level just to keep things smooth.
            CurrentMusicPlayer.IsPlaying = true;

            // Allow the player's character to move.
            // This is done after we load the lobby and the initialization level so the player's character doesn't fall through the map.
            Mover.IsRunning = true;

            Disposables.Add(fpsLimiter);
            Disposables.Add(rotationLockControl);
            Disposables.Add(bottomBar);
            Disposables.Add(animatedNodes);
        }

        public new void Dispose()
        {
            // Save settings configuration to file
            SettingsFile settingsFile = settings.File;
            if (settingsFile != null)
            {
                settingsFile.Data = settings.Group.ToJsonObject();
                settingsFile.Write();
            }

            settings.Group.Dispose();
            base.Dispose();
        }
    }
}
