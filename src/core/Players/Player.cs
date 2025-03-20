using Godot;
using System;
using System.Collections.Generic;

using UTheCat.Jumpvalley.Core.Levels.Interactives;
using UTheCat.Jumpvalley.Core.Music;
using UTheCat.Jumpvalley.Core.Players.Camera;
using UTheCat.Jumpvalley.Core.Players.Movement;
using UTheCat.Jumpvalley.Core.Timing;

namespace UTheCat.Jumpvalley.Core.Players
{
    /// <summary>
    /// This class represents a player who is playing Jumpvalley or some other app that derives from it.
    /// This class inherits from the <see cref="Interactive"/> class since it contains some functionality that
    /// assists with initializing, starting, stopping, and disposing the app components functioning
    /// on behalf of the player.
    /// <br/>
    /// <br/>
    /// This class contains some of the basic components that allow the app to function for the player, such as:
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
        /// The scene tree that the current application instance is running under.
        /// This <see cref="Player"/> instance should "correspond to" the application instance.
        /// </summary>
        public SceneTree Tree { get; private set; }

        /// <summary>
        /// The root node containing the app's components as Godot nodes.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The player's current music player
        /// </summary>
        public MusicPlayer CurrentMusicPlayer { get; protected set; }

        /// <summary>
        /// The player's primary GUI root node
        /// </summary>
        public Node PrimaryGui { get; protected set; }

        /// <summary>
        /// The player's current character instance
        /// </summary>
        public CharacterBody3D Character { get; protected set; }

        /// <summary>
        /// The primary mover instance acting on behalf of the player's character
        /// </summary>
        public BaseMover Mover { get; protected set; }

        /// <summary>
        /// The primary node that handles the player's camera
        /// </summary>
        public BaseCamera Camera { get; protected set; }

        /// <summary>
        /// Objects that will get disposed of once the current Player instance gets Dispose()'d.
        /// </summary>
        public List<IDisposable> Disposables { get; private set; }

        public Player(SceneTree tree, Node rootNode) : base(new OffsetStopwatch(TimeSpan.Zero))
        {
            Disposables = new List<IDisposable>();
            Tree = tree;
            RootNode = rootNode;       

            // These should be modifiable by classes inheriting from Jumpvalley's core Player class
            PrimaryGui = null;
            Character = null;
            CurrentMusicPlayer = null;
            Mover = null;
            Camera = null;
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
