using Godot;
using System;
using System.Collections.Generic;

using Jumpvalley.Levels;
using Jumpvalley.Music;
using Jumpvalley.Players.Camera;
using Jumpvalley.Players.Controls;
using Jumpvalley.Players.Gui;
using Jumpvalley.Players.Movement;
using Jumpvalley.Testing;

namespace Jumpvalley.Players
{
    /// <summary>
    /// This class represents a player who is playing Jumpvalley.
    /// <br/>
    /// The class contains some of the basic components that allow Jumpvalley to function for the player, such as:
    /// <list type="bullet">
    /// <item>Their music player</item>
    /// <item>The Controller instance that allows them to control their character</item>
    /// <item>The Camera instance that allows them to control their camera</item>
    /// <item>Their primary GUI node</item>
    /// </list>
    /// </summary>
    public partial class Player : IDisposable
    {
        /// <summary>
        /// The scene tree that the player's game is under.
        /// </summary>
        public SceneTree Tree { get; private set; }

        /// <summary>
        /// The root node containing the nodes in the player's game.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The player's current music player
        /// </summary>
        public MusicZonePlayer CurrentMusicPlayer { get; private set; }

        /// <summary>
        /// The player's primary GUI root node
        /// </summary>
        public Control PrimaryGui { get; private set; }

        /// <summary>
        /// The player's current character instance
        /// </summary>
        public CharacterBody3D Character { get; private set; }

        /// <summary>
        /// The primary mover instance acting on behalf of the player's character
        /// </summary>
        public BaseMover Mover { get; private set; }

        /// <summary>
        /// The primary node that handles the player's camera
        /// </summary>
        public BaseCamera Camera { get; private set; }

        /// <summary>
        /// Objects that will get disposed of once the current Player instance gets Dispose()'d.
        /// </summary>
        public List<IDisposable> Disposables { get; private set; } = new List<IDisposable>();

        public Player(SceneTree tree, Node rootNode)
        {
            Tree = tree;
            RootNode = rootNode;

            CurrentMusicPlayer = new MusicZonePlayer();
            CurrentMusicPlayer.Name = "CurrentMusicPlayer";
            PrimaryGui = rootNode.GetNode<Control>("PrimaryGui");
            Character = rootNode.GetNode<CharacterBody3D>("Player");
            Mover = new KeyboardMover();
            Camera = new MouseCamera();

            rootNode.AddChild(CurrentMusicPlayer);
        }

        /// <summary>
        /// Start method for the game in terms of the player
        /// </summary>
        public virtual void Start()
        {
            // Handle music that's played in the main scene file
            Node rootNodeMusic = RootNode.GetNode("Music");
            MusicGroup primaryMusic = new MusicGroup(rootNodeMusic.GetNode("PrimaryMusic"));
            Node primaryMusicZones = rootNodeMusic.GetNode("MusicZones");

            CurrentMusicPlayer.BindedNode = Character;
            CurrentMusicPlayer.TransitionTime = 3;
            CurrentMusicPlayer.OverrideTransitionTime = true;
            CurrentMusicPlayer.PrimaryPlaylist = primaryMusic;

            Disposables.Add(primaryMusic);

            foreach (Node zone in primaryMusicZones.GetChildren())
            {
                MusicZone musicZone = new MusicZone(zone);
                CurrentMusicPlayer.Add(musicZone);
                Disposables.Add(musicZone);
            }

            //CurrentMusicPlayer.IsPlaying = true;

            // Set up character movement
            // Some values here are based on Juke's Towers of Hell physics (or somewhere close), except we're working with meters.
            // In-game gravity can be changed at runtime, so we need to account for that. See:
            // https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-physics-3d-default-gravity
            // for more details.
            Mover.Gravity = PhysicsServer3D.AreaGetParam(RootNode.GetViewport().FindWorld3D().Space, PhysicsServer3D.AreaParameter.Gravity).As<float>();
            Mover.JumpVelocity = 25f;
            Mover.Speed = 8f;

            Mover.Body = Character;

            // Set up the player's camera. This is done in between movement setup steps because
            // some of the movement stuff depends on the state of the player's camera.
            Camera.FocusedNode = Character.GetNode<Node3D>("Head");
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

            // Allow the player's character to move
            Mover.IsRunning = true;

            RootNode.AddChild(Mover);
            RootNode.AddChild(Camera);

            // Set up fullscreen toggling
            FullscreenControl fullscreenControl = new FullscreenControl(false);
            RootNode.AddChild(fullscreenControl);
            Disposables.Add(fullscreenControl);

            // Input with higher accuracy and less lag is preferred in Juke's Towers of Hell fangames,
            // since very small differences in input can have a large impact on gameplay.
            // This is why it's important to make the input refresh rate independent from display refresh rate.
            Input.UseAccumulatedInput = false;

            RenderFramerateLimiter fpsLimiter = new RenderFramerateLimiter();
            fpsLimiter.MinFpsDifference = 0;
            fpsLimiter.IsRunning = true;
            RootNode.AddChild(fpsLimiter);

            // Initialize GUI stuff
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), CurrentMusicPlayer);

            PackedScene primaryLevelMenuScene = ResourceLoader.Load<PackedScene>("res://gui/level_menu.tscn");
            if (primaryLevelMenuScene != null)
            {
                Control primaryLevelMenuNode = primaryLevelMenuScene.Instantiate<Control>();
                PrimaryLevelMenu primaryLevelMenu = new PrimaryLevelMenu(primaryLevelMenuNode, Tree);
                bottomBar.PrimaryLevelMenu = primaryLevelMenu;
                primaryLevelMenuScene.Dispose();

                PrimaryGui.AddChild(primaryLevelMenuNode);
                Disposables.Add(primaryLevelMenu);

                primaryLevelMenuScene.Dispose();
            }

            // Set up level-running stuff
            UserLevelRunner levelRunner = new UserLevelRunner(this, new LevelTimer(PrimaryGui.GetNode("LevelTimer")));

            // Load the lobby
            LevelPackage lobby = new LevelPackage("res://scenes/lobby", levelRunner);
            levelRunner.Lobby = lobby;
            Disposables.Add(lobby);
            lobby.LoadRootNode();
            lobby.CreateLevelInstance();
            lobby.StartLevel();
            RootNode.AddChild(lobby.RootNode);

            // Load the initialization level (the level we want to load in when the game starts)
            string levelsNodeName = "Levels";
            string initializationLevelMetadataName = "initialization_level";
            Node levelsNode = RootNode.GetNode(levelsNodeName);
            if (levelsNode != null)
            {
                if (levelsNode.HasMeta(initializationLevelMetadataName))
                {
                    string levelPath = levelsNode.GetMeta(initializationLevelMetadataName).As<string>();

                    if (!string.IsNullOrEmpty(levelPath))
                    {
                        LevelPackage levelPackage = new LevelPackage(levelPath, levelRunner);
                        levelRunner.CurrentLevelPackage = levelPackage;
                        Disposables.Add(levelPackage);
                        levelPackage.LoadRootNode();
                        levelPackage.CreateLevelInstance();
                        levelPackage.StartLevel();
                        levelsNode.AddChild(levelPackage.RootNode);
                    }
                }
            }
            else
            {
                Console.WriteLine($"[{nameof(Player)}] Failed to load a level at game initialization. The root node of the main scene is missing a node named '{levelsNodeName}'.");
            }

            // Start playing music.
            // This is done after we load the lobby and the initialization level just to keep things smooth.
            CurrentMusicPlayer.IsPlaying = true;

            Disposables.Add(fpsLimiter);
            Disposables.Add(rotationLockControl);
            Disposables.Add(Mover);
            Disposables.Add(Camera);
            Disposables.Add(bottomBar);
        }

        public void Dispose()
        {
            PrimaryGui?.Dispose();
            CurrentMusicPlayer?.Dispose();

            for (int i = 0; i < Disposables.Count; i++)
            {
                Disposables[i].Dispose();
            }

            //GC.SuppressFinalize(this);
        }
    }
}
