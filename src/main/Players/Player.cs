using System;
using System.Collections.Generic;
using Godot;

using Jumpvalley.Music;
using Jumpvalley.Players.Gui;

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
        /// Objects that will get disposed of once the current Player instance gets Dispose()'d.
        /// </summary>
        public List<IDisposable> Disposables { get; private set; } = new List<IDisposable>();

        public Player(SceneTree tree, Node rootNode)
        {
            Tree = tree;
            RootNode = rootNode;

            CurrentMusicPlayer = new MusicPlayer();
            CurrentMusicPlayer.Name = "CurrentMusicPlayer";
            PrimaryGui = (Control) rootNode.FindChild("PrimaryGui");

            rootNode.AddChild(CurrentMusicPlayer);
        }

        /// <summary>
        /// Start method for the game in terms of the player
        /// </summary>
        public virtual void Start()
        {
            BottomBar bottomBar = new BottomBar(PrimaryGui.GetNode("BottomBar"), CurrentMusicPlayer);

            Playlist testPlaylist = new Playlist();
            Song testSong = new Song(new SongPackage("res://addons/music/KORAII/Night_Echo"));

            testPlaylist.Add(testSong);
            CurrentMusicPlayer.PrimaryPlaylist = testPlaylist;
            CurrentMusicPlayer.IsPlaying = true;

            Disposables.Add(bottomBar);
            Disposables.Add(testPlaylist);
        }

        public void Dispose()
        {
            PrimaryGui?.Dispose();

            for (int i = 0; i < Disposables.Count; i++)
            {
                Disposables[i].Dispose();
            }

            CurrentMusicPlayer?.Dispose();

            //GC.SuppressFinalize(this);
        }
    }
}
