using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

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
    public partial class Player: IDisposable
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
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), CurrentMusicPlayer);

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

            CurrentMusicPlayer.IsPlaying = true;

            //Testing.MusicPlayerTest mpTest = new Testing.MusicPlayerTest(CurrentMusicPlayer);
            //RootNode.AddChild(mpTest);
            //mpTest.StartTest();

            MeshSpinner spinner = new MeshSpinner(RootNode.GetNode<MeshInstance3D>("Lobby/SpinningMesh"), 1);
            RootNode.AddChild(spinner);

            // Juke's Towers of Hell physics (or somewhere close) except we're working with meters
            Mover.Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").As<float>(); //98.1f;
            Mover.JumpVelocity = 25f;
            Mover.Speed = 8f;

            Mover.Body = Character;

            // Allow the player to climb stuff
            Climber climber = new Climber(Character.GetNode<CollisionShape3D>("TorsoCollision"));
            climber.OnCanClimbChanged += (object _o, bool canClimb) =>
            {
                Mover.IsClimbing = canClimb;
            };

            Camera.FocusedNode = Character.GetNode<Node3D>("Head");
            Camera.Camera = RootNode.GetNode<Camera3D>("Camera");
            Camera.PanningSensitivity = 1;
            Camera.PanningSpeed = (float)(0.2 * Math.PI) * 0.33f;
            Camera.MinPitch = (float)(-0.45 * Math.PI);
            Camera.MaxPitch = (float)(0.45 * Math.PI);

            Camera.MaxZoomOutDistance = 15f;
            Camera.ZoomOutDistance = 5f;

            Mover.Camera = Camera;

            RotationLockControl rotationLockControl = new RotationLockControl(Mover, Camera);
            RootNode.AddChild(rotationLockControl);

            Mover.IsRunning = true;
                
            RootNode.AddChild(Mover);
            RootNode.AddChild(Camera);

            FullscreenControl fullscreenControl = new FullscreenControl(false);
            RootNode.AddChild(fullscreenControl);
            Disposables.Add(fullscreenControl);

            // Input with higher accuracy and less lag is preferred in Juke's Towers of Hell fangames,
            // since very small differences in input can have a large impact on gameplay.
            // This is why it's important to make the input refresh rate independent from display refresh rate.
            Input.UseAccumulatedInput = false;

            // misc testing stuff
            LevelLoadingTest levelLoadingTest = new LevelLoadingTest("res://levels/demo_platformer", RootNode.GetNode("Levels"));
            Disposables.Add(levelLoadingTest);
            levelLoadingTest.Start();

            RenderFramerateLimiter fpsLimiter = new RenderFramerateLimiter();
            fpsLimiter.IsRunning = true;
            RootNode.AddChild(fpsLimiter);

            Disposables.Add(fpsLimiter);
            Disposables.Add(rotationLockControl);
            Disposables.Add(climber);
            Disposables.Add(Mover);
            Disposables.Add(Camera);
            Disposables.Add(bottomBar);
            Disposables.Add(spinner);
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
