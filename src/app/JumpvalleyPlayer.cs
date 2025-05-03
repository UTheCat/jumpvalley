using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

using UTheCat.Jumpvalley.Core.Gui;
using UTheCat.Jumpvalley.Core.Levels;
using UTheCat.Jumpvalley.Core.Levels.Interactives.Mechanics;
using UTheCat.Jumpvalley.Core.Logging;
using UTheCat.Jumpvalley.Core.Music;
using UTheCat.Jumpvalley.Core.Players;
using UTheCat.Jumpvalley.Core.Tweening;

using UTheCat.Jumpvalley.App.Display;
using UTheCat.Jumpvalley.App.Gui;
using UTheCat.Jumpvalley.App.Levels;
using UTheCat.Jumpvalley.App.Players.Camera;
using UTheCat.Jumpvalley.App.Players.Movement;
using UTheCat.Jumpvalley.App.Settings;
using UTheCat.Jumpvalley.App.Settings.Display;

namespace UTheCat.Jumpvalley.App
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
        private FramerateCounter framerateCounter;

        public JumpvalleyPlayer(SceneTree tree, Node rootNode) : base(tree, rootNode)
        {
            PrimaryGui = rootNode.GetNode("PrimaryGui");
            Character = rootNode.GetNode<CharacterBody3D>("Character");
            CurrentMusicPlayer = new MusicZonePlayer();
            Mover = new KeyboardMover();
            Camera = new UserInputCamera();

            logger = new ConsoleLogger(nameof(JumpvalleyPlayer));
            settings = new JumpvalleySettings();
            framerateCounter = null;

            Disposables.AddRange(
                [
                    settings,
                    PrimaryGui,
                    Character,
                    CurrentMusicPlayer,
                    Mover,
                    Camera
                ]
            );
        }

        public override void Start()
        {
            base.Start();

            // The app's main setting group
            SettingGroup mainSettingGroup = settings.Group;

            // HiDPI adapter
            HiDpiAdapter hiDpiAdapter = new HiDpiAdapter(Tree.Root)
            {
                // We'll use 2 as the max content scale factor for now.
                // There might be a better solution that gets implemented in the future.
                MaxContentScaleFactor = 2f
            };
            Disposables.Add(hiDpiAdapter);

            // HiDPI adapter toggle setting
            mainSettingGroup.GetNode<SettingGroup>("display")?.Add(
                new HiDpiAdapterToggle(hiDpiAdapter)
            );

            // Handle music that's played in the main scene file
            Node rootNodeMusic = RootNode.GetNode("Music");
            MusicGroup primaryMusic = new MusicGroup(rootNodeMusic.GetNode("PrimaryMusic"));
            Node primaryMusicZones = rootNodeMusic.GetNode("MusicZones");

            if (!(CurrentMusicPlayer is MusicZonePlayer)) throw new Exception($"{nameof(CurrentMusicPlayer)} isn't a MusicZonePlayer.");

            MusicZonePlayer musicPlayer = (MusicZonePlayer)CurrentMusicPlayer;
            musicPlayer.BindedNode = Character;
            musicPlayer.TransitionTime = 3;
            musicPlayer.OverrideTransitionTime = true;
            musicPlayer.VolumeScale = 1;
            musicPlayer.OverrideLocalVolumeScale = true;
            musicPlayer.OverrideSongStreamHandlingMode = true;

            musicPlayer.AddPlaylist(primaryMusic);
            musicPlayer.PrimaryPlaylist = primaryMusic;

            Disposables.Add(primaryMusic);

            foreach (Node zone in primaryMusicZones.GetChildren())
            {
                MusicZone musicZone = new MusicZone(zone);
                musicPlayer.Add(musicZone);
                Disposables.Add(musicZone);
            }

            //musicPlayer.IsPlaying = true;

            // Character bounding box
            OverallBoundingBoxObject characterBoundingBox = new OverallBoundingBoxObject(Clock, Character.GetNode("_InteractiveBoundingBox"));
            Disposables.Add(characterBoundingBox);

            // Set up character movement
            // Some values here are based on Eternal Towers of Hell physics (or somewhere close), except we're working with meters.
            // In-app gravity can be changed at runtime, so we need to account for that. See:
            // https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-physics-3d-default-gravity
            // for more details.
            Mover.Gravity = PhysicsServer3D.AreaGetParam(RootNode.GetViewport().FindWorld3D().Space, PhysicsServer3D.AreaParameter.Gravity).As<float>();
            Mover.JumpVelocity = 25f;
            Mover.Speed = 8f;
            Mover.Acceleration = 360f; //180f;
            Mover.AirAcceleration = 180f; //90f;
            Mover.Deceleration = 360f; //180f;
            Mover.AirDeceleration = 180f; //90f;
            Mover.Rotator.Speed = 12f;

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

            // Set up fast-turn
            FastTurnControl fastTurnControl = new FastTurnControl(Mover, Camera);
            RootNode.AddChild(fastTurnControl);

            RootNode.AddChild(Mover);
            RootNode.AddChild(Camera);

            // Input with higher accuracy and less lag is preferred in Eternal Towers of Hell fangames,
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
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), musicPlayer)
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

            MusicPanel musicPanel = new MusicPanel(musicPlayer, musicPanelNode, Tree);
            animatedNodes.Add("music_panel", musicPanel);

            //bottomBar.PrimaryMusicPanel = musicPanel;

            // Framerate counter
            framerateCounter = new FramerateCounter(PrimaryGui.GetNode<Control>("FramerateCounter"));
            FramerateCounterToggle framerateCounterToggle = settings.Group.GetNode<FramerateCounterToggle>("display/show_framerate");
            if (framerateCounterToggle != null)
            {
                framerateCounterToggle.Counter = framerateCounter;
            }
            
            Disposables.Add(framerateCounter);

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

            // Fast turn indicator
            FastTurnIndicator fastTurnIndicator = new FastTurnIndicator(Mover, PrimaryGui.GetNode<Control>("FastTurnIndicator"));
            Disposables.Add(fastTurnIndicator);

            // Camera turn indicator
            CameraTurnIndicator cameraTurnIndicator = new CameraTurnIndicator(Camera as UserInputCamera, PrimaryGui.GetNode<Control>("CameraTurnIndicator"));
            Disposables.Add(cameraTurnIndicator);

            // Intro panel
            Panel introPanelNode = PrimaryGui.GetNode<Panel>("IntroPanel");
            introPanelNode.Visible = true;

            SceneTreeTween introPanelFade = new SceneTreeTween(0.5, Tween.TransitionType.Linear, Tween.EaseType.Out, Tree)
            {
                InitialValue = 1.0,
                FinalValue = 0.0
            };
            introPanelFade.OnStep += (object _o, float frac) =>
            {
                float opacity = (float)introPanelFade.GetCurrentValue();

                Color modulate = introPanelNode.Modulate;
                modulate.A = opacity;
                introPanelNode.Modulate = modulate;
            };
            introPanelFade.OnFinish += (object _o, EventArgs _e) =>
            {
                introPanelNode.Visible = false;
                Disposables.Remove(introPanelFade);
                introPanelFade.Dispose();
            };
            Disposables.Add(introPanelFade);
            Disposables.Add(introPanelNode);

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

            List<DirectionalLight3D> levelDirectionalLightList = new List<DirectionalLight3D>();

            LevelPackage StartLevel(string levelPath, Node levelNodeParent)
            {
                if (!string.IsNullOrEmpty(levelPath))
                {
                    LevelPackage levelPackage = new LevelPackage(levelPath, levelRunner);
                    levelRunner.CurrentLevelPackage = levelPackage;
                    Disposables.Add(levelPackage);
                    levelPackage.LoadRootNode();
                    levelPackage.CreateLevelInstance();
                    levelPackage.StartLevel();

                    Level levelInstance = levelPackage.LevelInstance;

                    Godot.Environment levelEnvironment = levelInstance.LevelEnvironment;
                    if (levelEnvironment != null)
                    {
                        Camera.Camera.Environment = levelEnvironment;
                    }

                    DirectionalLight3D levelDirectionalLight = levelInstance.LevelDirectionalLight;
                    if (levelDirectionalLight != null)
                    {
                        DirectionalLight3D defaultDirectionalLight = RootNode.GetNodeOrNull<DirectionalLight3D>("DefaultDirectionalLight");

                        if (defaultDirectionalLight != null)
                        {
                            defaultDirectionalLight.Visible = false;
                        }

                        if (!levelDirectionalLightList.Contains(levelDirectionalLight))
                        {
                            levelDirectionalLightList.Add(levelDirectionalLight);
                        }

                        foreach (DirectionalLight3D light in levelDirectionalLightList)
                        {
                            if (light != levelDirectionalLight)
                            {
                                light.Visible = false;
                            }
                        }
                    }

                    levelNodeParent.AddChild(levelPackage.RootNode);

                    levelInstance.SendPlayerToCurrentCheckpoint();

                    MusicGroup levelPrimaryPlaylist = levelInstance.PrimaryPlaylist;
                    if (levelPrimaryPlaylist != null)
                    {
                        musicPlayer.AddPlaylist(levelPrimaryPlaylist);
                        musicPlayer.PrimaryPlaylist = levelPrimaryPlaylist;
                    }

                    return levelPackage;
                }

                return null;
            }

            // Load the initialization lobby (the lobby we want to load in when the app starts)
            if (RootNode.HasMeta(INITIALIZATION_LOBBY_META_NAME))
            {
                string lobbyPath = RootNode.GetMeta(INITIALIZATION_LOBBY_META_NAME).As<string>();
                StartLevel(lobbyPath, RootNode);
            }

            // Load the initialization level (the level we want to load in when the app starts)
            string levelsNodeName = "Levels";
            Node levelsNode = RootNode.GetNode(levelsNodeName);
            if (levelsNode != null)
            {
                if (levelsNode.HasMeta(INITIALIZATION_LEVEL_META_NAME))
                {
                    string levelPath = levelsNode.GetMeta(INITIALIZATION_LEVEL_META_NAME).As<string>();

                    LevelPackage levelPackage = StartLevel(levelPath, levelsNode);
                    if (levelPackage != null)
                    {
                        LevelInfo levelInfo = levelPackage.Info;
                        Difficulty difficulty = DifficultyPresets.PRIMARY_DIFFICULTIES.GetDifficultyByRating(levelInfo.LevelDifficulty);
                        logger.Print($"Now playing a level: {levelInfo.FullName} by {levelInfo.Creators} [{difficulty.Name} - {difficulty.Rating}]");
                    }
                }
            }
            else
            {
                logger.Print($"Failed to load a level at app initialization. The root node of the main scene is missing a node named '{levelsNodeName}'.");
            }

            // Start playing music.
            // This is done after we load the lobby and the initialization level just to keep things smooth.
            if (CurrentMusicPlayer.GetParent() == null) RootNode.AddChild(CurrentMusicPlayer);
            musicPlayer.IsPlaying = true;

            // Allow the player's character to move.
            // This is done after we load the lobby and the initialization level so the player's character doesn't fall through the map.
            Mover.IsRunning = true;

            Disposables.Add(fpsLimiter);
            Disposables.Add(fastTurnControl);
            Disposables.Add(bottomBar);
            Disposables.Add(animatedNodes);

            introPanelFade.Resume();
        }

        private void SaveSettings()
        {
            SettingsFile settingsFile = settings.File;
            if (settingsFile != null)
            {
                settingsFile.Data = settings.Group.ToJsonObject();
                settingsFile.Write();
            }
        }

        public new void Dispose()
        {
            // Save settings configuration to file
            SaveSettings();

            base.Dispose();
        }

        public override void _Process(double delta)
        {
            FramerateCounter counter = framerateCounter;
            if (counter != null)
            {
                counter.CurrentFps = Engine.GetFramesPerSecond();
            }
        }

        public override void _Notification(int what)
        {
            if (what == NotificationApplicationPaused)
            {
                // Settings have to save when NOTIFICATION_APPLICATION_PAUSED
                // is received in order for them to save on Android and iOS
                // without the user pressing "Exit App" in the app's menu.
                SaveSettings();
            }
        }
    }
}
