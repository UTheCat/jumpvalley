using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

using Jumpvalley.Music;
using Jumpvalley.Players.Gui;
using Jumpvalley.Players.Movement;

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
        public MusicPlayer CurrentMusicPlayer { get; private set; }

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
        /// Objects that will get disposed of once the current Player instance gets Dispose()'d.
        /// </summary>
        public List<IDisposable> Disposables { get; private set; } = new List<IDisposable>();

        public Player(SceneTree tree, Node rootNode)
        {
            Tree = tree;
            RootNode = rootNode;

            CurrentMusicPlayer = new MusicPlayer();
            CurrentMusicPlayer.Name = "CurrentMusicPlayer";
            PrimaryGui = rootNode.GetNode<Control>("PrimaryGui");
            Character = rootNode.GetNode<CharacterBody3D>("Player");
            Mover = new KeyboardMover();

            rootNode.AddChild(CurrentMusicPlayer);
        }

        /// <summary>
        /// Start method for the game in terms of the player
        /// </summary>
        public virtual void Start()
        {
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), CurrentMusicPlayer);

            Node testSongNode = RootNode.GetNode("Music/Primary/Song");

            Playlist testPlaylist = new Playlist();
            Song testSong = new Song(new SongPackage((string)testSongNode.GetMeta("directory_path")));

            testPlaylist.Add(testSong);
            CurrentMusicPlayer.PrimaryPlaylist = testPlaylist;
            CurrentMusicPlayer.IsPlaying = true;

            //Testing.MusicPlayerTest mpTest = new Testing.MusicPlayerTest(CurrentMusicPlayer);
            //RootNode.AddChild(mpTest);
            //mpTest.StartTest();

            BoxSpinner spinner = new BoxSpinner((CsgBox3D)RootNode.GetNode("Map/CSGBox3D"), 1);
            RootNode.AddChild(spinner);

            // Juke's Towers of Hell physics (or somewhere close)
            Mover.Gravity = 196.19997f * (25 / 7) * 0.2f;
            Mover.JumpVelocity = 50f * (25 / 7) * 0.2f;
            Mover.Speed = 16f * (25 / 7) * 0.2f;

            Mover.Body = Character;
            Mover.SetPhysicsProcess(true);
            RootNode.AddChild(Mover);

            Disposables.Add(Mover);
            Disposables.Add(bottomBar);
            Disposables.Add(testPlaylist);
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
