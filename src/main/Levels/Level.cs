using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using Jumpvalley.Players;
using Jumpvalley.Music;
using Jumpvalley.Levels.Interactives;
using Jumpvalley.Levels.Interactives.Mechanics;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level that's playable in Jumpvalley.
    /// <br/>
    /// Each level contains four primary components: interactives, music, static objects, and start point.
    /// More details can be found on Jumpvalley's wiki article on <see href="https://github.com/UTheDev/jumpvalley/wiki/Level-Layout">Level Layout</see>.
    /// </summary>
    public partial class Level : Interactive, IDisposable
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";

        /// <summary>
        /// Indicates the current run state of the level.
        /// </summary>
        public enum RunState
        {
            /// <summary>
            /// Indicates that the level is not running
            /// </summary>
            Stopped = 0,

            /// <summary>
            /// Indicates that the level is still running (in the background) but is paused
            /// </summary>
            Paused = 1,

            /// <summary>
            /// Indicates that the level is actively running and not paused
            /// </summary>
            Playing = 2
        }

        /// <summary>
        /// Information about the level that's specified in the level's info file.
        /// This includes things like level ID and full name.
        /// </summary>
        public LevelInfoFile Info { get; private set; }

        /// <summary>
        /// The root node of the level
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The node containing the level's interactives
        /// </summary>
        public Node InteractivesNode { get; private set; }

        /// <summary>
        /// The list of the level's interactives as instances of the <see cref="InteractiveNode"/> class
        /// </summary>
        public List<InteractiveNode> Interactives { get; private set; }

        /// <summary>
        /// The node containing the level's music
        /// </summary>
        public Node Music { get; private set; }

        /// <summary>
        /// The level's music zones
        /// </summary>
        public List<MusicZone> MusicZones { get; private set; }

        /// <summary>
        /// The node containing the level's static objects
        /// </summary>
        public Node StaticObjects { get; private set; }

        /// <summary>
        /// The Node3D that defines the position and rotation that the player's character will be set to when they play the level from the very beginning
        /// </summary>
        public Node3D StartPoint { get; private set; }

        /// <summary>
        /// The level's current run state
        /// </summary>
        public RunState CurrentRunState { get; private set; }

        /// <summary>
        /// Set externally to help the level's code access whatever object is running the level itself
        /// </summary>
        public LevelRunner Runner = null;

        /// <summary>
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its info file
        /// </summary>
        /// <param name="node">The root node of the level to represent</param>
        public Level(LevelInfoFile info, Node root) : base(new Stopwatch())
        {
            Info = info;
            RootNode = root;

            InteractivesNode = root.GetNode(INTERACTIVES_NODE_NAME);
            Music = root.GetNode(MUSIC_NODE_NAME);
            StaticObjects = root.GetNode(STATIC_OBJECTS_NODE_NAME);

            Interactives = new List<InteractiveNode>();
            if (InteractivesNode != null)
            {
                foreach (Node node in InteractivesNode.GetChildren())
                {
                    if (node.HasMeta(InteractiveToolkit.INTERACTIVE_TYPE_METADATA_NAME))
                    {
                        string interactiveType = node.GetMeta(InteractiveToolkit.INTERACTIVE_TYPE_METADATA_NAME).As<string>();

                        if (interactiveType.Equals("Spinner"))
                        {
                            Interactives.Add(new Spinner(Clock, node));
                        }
                    }
                }
            }

            MusicZones = new List<MusicZone>();
            if (Music != null)
            {
                Node musicZonesNode = Music.GetNode("MusicZones");
                if (musicZonesNode != null)
                {
                    foreach (Node zoneNode in musicZonesNode.GetChildren())
                    {
                        MusicZones.Add(new MusicZone(zoneNode));
                    }
                }
            }

            CurrentRunState = RunState.Stopped;
        }

        /// <summary>
        /// Level initialization method that initializes the level after the constructor runs, in case such method is needed.
        /// <br/>
        /// By default, this method is only called once after the object's constructor runs.
        /// Initialize() typically shouldn't be called more than once for the same <see cref="Level"/> instance.
        /// </summary>
        public override void Initialize()
        {
            if (IsInitialized) return;

            foreach (InteractiveNode i in Interactives)
            {
                i.Initialize();
            }

            base.Initialize();
        }

        private void ToggleMusic(bool shouldPlay)
        {
            LevelRunner runner = Runner;
            if (runner != null)
            {
                Player player = runner.CurrentPlayer;

                if (player != null)
                {
                    MusicZonePlayer musicPlayer = player.CurrentMusicPlayer;
                    if (musicPlayer != null)
                    {
                        foreach (MusicZone zone in MusicZones)
                        {
                            if (shouldPlay)
                            {
                                musicPlayer.Add(zone);
                            }
                            else
                            {
                                musicPlayer.Remove(zone);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The level's start method. This method is called every time the user starts or restarts the level,
        /// and it's a great place to put code that will be run after initialization, but just before the level starts.
        /// </summary>
        public override void Start()
        {
            if (CurrentRunState == RunState.Playing) return;

            CurrentRunState = RunState.Playing;

            foreach (InteractiveNode i in Interactives)
            {
                i.Start();
            }

            // prepare the level's music
            ToggleMusic(true);

            base.Start();
        }

        /// <summary>
        /// The level's stop method. This method is called right after the user stops or exits the level.
        /// </summary>
        public override void Stop()
        {
            if (CurrentRunState == RunState.Stopped) return;

            CurrentRunState = RunState.Stopped;

            foreach (InteractiveNode i in Interactives)
            {
                i.Stop();
            }

            ToggleMusic(false);
            base.Stop();
        }

        /// <summary>
        /// Disposes of this <see cref="Level"/> instance. This method is a great place to free up resources being used by the level instance,
        /// especially right before the level itself gets freed from memory.
        /// </summary>
        public new void Dispose()
        {
            foreach (InteractiveNode i in Interactives)
            {
                i.Dispose();
            }

            base.Dispose();
        }
    }
}
