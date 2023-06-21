using System;
using Godot;
using Jumpvalley.Music;

namespace Jumpvalley.Player
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

        public Player(SceneTree tree, Node rootNode)
        {
            Tree = tree;
            RootNode = rootNode;

            CurrentMusicPlayer = new MusicPlayer();
            CurrentMusicPlayer.Name = "CurrentMusicPlayer";
            PrimaryGui = (Control) rootNode.FindChild("PrimaryGui");

            rootNode.AddChild(CurrentMusicPlayer);
        }

        public void Dispose()
        {
            PrimaryGui?.Dispose();
            CurrentMusicPlayer?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
