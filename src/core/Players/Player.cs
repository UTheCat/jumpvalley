using Godot;
using System;
using System.Collections.Generic;

using Jumpvalley.Levels.Interactives;
using Jumpvalley.Music;
using Jumpvalley.Players.Camera;
using Jumpvalley.Players.Movement;
using Jumpvalley.Timing;

namespace Jumpvalley.Players
{
    /// <summary>
    /// This class represents a player who is playing Jumpvalley or some other game that derives from it.
    /// <br/>
    /// The class contains some of the basic components that allow the game to function for the player, such as:
    /// <list type="bullet">
    /// <item>Their music player</item>
    /// <item>The Controller instance that allows them to control their character</item>
    /// <item>The Camera instance that allows them to control their camera</item>
    /// <item>Their primary GUI node</item>
    /// </list>
    /// </summary>
    public partial class Player : Interactive, IDisposable
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

        public Player(SceneTree tree, Node rootNode) : base(new OffsetStopwatch(TimeSpan.Zero))
        {
            Tree = tree;
            RootNode = rootNode;

            PrimaryGui = rootNode.GetNode<Control>("PrimaryGui");
            Character = rootNode.GetNode<CharacterBody3D>("Player");

            CurrentMusicPlayer = new MusicZonePlayer();
            CurrentMusicPlayer.Name = "CurrentMusicPlayer";
            Disposables.Add(CurrentMusicPlayer);

            Mover = new KeyboardMover();
            Disposables.Add(Mover);

            Camera = new MouseCamera();
            Disposables.Add(Camera);

            rootNode.AddChild(CurrentMusicPlayer);
        }

        /// <summary>
        /// Disposes of this <see cref="Player"/> instance
        /// and whatever objects are currently in the <see cref="Disposables"/> list. 
        /// </summary>
        public new void Dispose()
        {
            for (int i = 0; i < Disposables.Count; i++)
            {
                IDisposable obj = Disposables[i];
                obj.Dispose();
            }

            base.Dispose();
        }
    }
}
