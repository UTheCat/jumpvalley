using Godot;
using System;
using System.Collections.Generic;

using Jumpvalley.Players;
using Jumpvalley.Music;
using Jumpvalley.Levels.Interactives;
using Jumpvalley.Levels.Interactives.Mechanics;
using Jumpvalley.Levels.Interactives.Mechanics.Teleporters;
using Jumpvalley.Timing;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level that's playable in Jumpvalley.
    /// <br/>
    /// Each level contains four primary components: interactives, music, static objects, and start point.
    /// More details can be found on Jumpvalley's wiki article on <see href="https://github.com/UTheCat/jumpvalley/wiki/Level-Layout">Level Layout</see>.
    /// </summary>
    public partial class Level : Interactive, IDisposable
    {
        public static readonly string INTERACTIVES_NODE_NAME = "Interactives";
        public static readonly string MUSIC_NODE_NAME = "Music";
        public static readonly string STATIC_OBJECTS_NODE_NAME = "StaticObjects";
        public static readonly string CHECKPOINTS_NODE_NAME = "Checkpoints";

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
        public LevelInfo Info { get; private set; }

        /// <summary>
        /// The root node of the level
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The node containing the level's interactives
        /// </summary>
        public Node InteractivesNode { get; private set; }

        /// <summary>
        /// The list of the level's interactives as instances of the <see cref="Interactive"/> class
        /// </summary>
        public List<Interactive> Interactives { get; private set; }

        /// <summary>
        /// Interactives as instances of the <see cref="Interactive"/> class that are stored
        /// in <see cref="CheckpointsNode"/> 
        /// </summary>
        public List<Interactive> CheckpointInteractives { get; private set; }

        /// <summary>
        /// The node containing the level's music
        /// </summary>
        public Node Music { get; private set; }

        /// <summary>
        /// The level's music zones
        /// </summary>
        public List<MusicZone> MusicZones { get; private set; }

        /// <summary>
        /// The level's primary music playlist
        /// </summary>
        public MusicGroup PrimaryPlaylist;

        /// <summary>
        /// The node containing the level's static objects
        /// </summary>
        public Node StaticObjects { get; private set; }

        /// <summary>
        /// The node where the level's checkpoints should be stored.
        /// <br/><br/>
        /// This can be used as a place to load interactives that should be loaded
        /// after interactives placed in <see cref="InteractivesNode"/>. 
        /// </summary>
        public Node CheckpointsNode { get; private set; }

        /// <summary>
        /// The object that contains the code handling the level's checkpoints.
        /// </summary>
        public CheckpointSet Checkpoints { get; private set; }

        /// <summary>
        /// The level's current run state
        /// </summary>
        public RunState CurrentRunState { get; private set; }

        /// <summary>
        /// The <see cref="LevelPackage"/> that this <see cref="Level"/> instance belongs to
        /// </summary>
        public LevelPackage Package = null;

        /// <summary>
        /// List containing the order in which interactive lists should load and unload
        /// </summary>
        private List<Interactive>[] interactiveLists;

        /// <summary>
        /// Constructs an instance of <see cref="Level"/> to represent a level corresponding to its info file
        /// </summary>
        /// <param name="info">Info about the level</param>
        /// <param name="root">The root node of the level to represent</param>
        /// <param name="lastElapsedTime">The most recent amount of elapsed running time that the level left off of</param>
        /// <param name="runner">The object that's running this level. Typically, this should be a <see cref="LevelRunner"/>.</param>  
        public Level(LevelInfo info, Node root, TimeSpan lastElapsedTime, object runner = null) : base(new OffsetStopwatch(lastElapsedTime), runner)
        {
            Info = info;
            RootNode = root;

            InteractivesNode = root.GetNode(INTERACTIVES_NODE_NAME);
            Music = root.GetNode(MUSIC_NODE_NAME);
            StaticObjects = root.GetNode(STATIC_OBJECTS_NODE_NAME);

            Interactives = new List<Interactive>();
            if (InteractivesNode != null)
            {
                AddInteractivesInternal(InteractivesNode, Interactives);
            }

            CheckpointInteractives = new List<Interactive>();
            CheckpointsNode = root.GetNodeOrNull(CHECKPOINTS_NODE_NAME);
            if (CheckpointsNode == null)
            {
                Checkpoints = null;
            }
            else
            {
                AddInteractivesInternal(CheckpointsNode, CheckpointInteractives);
                Checkpoints = new CheckpointSet(Clock, CheckpointsNode.GetNode(InteractiveNode.NODE_MARKER_NAME_PREFIX));
            }

            interactiveLists = [Interactives, CheckpointInteractives];

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

                Node primaryMusicNode = Music.GetNode("PrimaryMusic");
                if (primaryMusicNode != null)
                {
                    PrimaryPlaylist = new MusicGroup(primaryMusicNode);
                }
            }

            CurrentRunState = RunState.Stopped;
        }

        private void AddInteractivesInternal(Node parentNode, List<Interactive> list)
        {
            foreach (Node node in parentNode.GetChildren())
            {
                if (node.Name.ToString().StartsWith(InteractiveNode.NODE_MARKER_NAME_PREFIX))
                {
                    InitializeInteractive(node);
                }
                else
                {
                    // Recursive search to search a child node's children within the Interactives folder
                    AddInteractivesInternal(node, list);
                }
            }
        }

        /// <summary>
        /// Gets the player instance that this level is running under.
        /// Returns the player instance if found, null otherwise.
        /// <br/><br/>
        /// Player instance is obtained from this level's runner
        /// if it's of type <see cref="LevelRunner"/>. 
        /// </summary>
        public Player GetCurrentPlayer()
        {
            if (Runner is LevelRunner levelRunner && levelRunner != null)
            {
                return levelRunner.CurrentPlayer;
            }

            return null;
        }

        /// <summary>
        /// The level's method for initializing each of its <see cref="Interactive"/>s.
        /// <br/><br/>
        /// This method can be overriden to provide custom logic for initalizing an interactive
        /// (e.g. when a level has custom <see cref="Interactive"/> types).
        /// </summary>
        /// <param name="nodeMarker">The interactive's node marker</param>
        public virtual void InitializeInteractive(Node nodeMarker)
        {
            string interactiveType = InteractiveNode.GetTypeNameFromMarker(nodeMarker);

            Interactive interactive = null;

            if (interactiveType.Equals("Spinner"))
            {
                Spinner spinner = new Spinner(Clock, nodeMarker);
                interactive = spinner;
            }
            else if (interactiveType.Equals("Teleporter"))
            {
                Teleporter teleporter = new Teleporter(Clock, nodeMarker);
                interactive = teleporter;
            }
            else if (interactiveType.Equals("StartEndTeleporter"))
            {
                StartEndTeleporter teleporter = new StartEndTeleporter(Clock, nodeMarker);
                interactive = teleporter;
            }

            // If the interactive's type is recognized,
            // let it know that this level is running it,
            // and then add the interactive to the Interactives list
            if (interactive != null)
            {
                interactive.Runner = this;
                Interactives.Add(interactive);

                if (interactive is InteractiveNode interactiveNode)
                {
                    interactiveNode.ParentedToNodeMarker = true;
                }
            }
        }

        /// <summary>
        /// Sends the player's character to the checkpoint they're currently on
        /// </summary>
        public void SendPlayerToCurrentCheckpoint()
        {
            if (Checkpoints != null)
            {
                Player player = GetCurrentPlayer();
                if (player != null)
                {
                    CharacterBody3D character = player.Character;

                    if (character != null)
                    {
                        Checkpoints.SendToCurrentCheckpoint(character);
                    }
                }
            }
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

            foreach (List<Interactive> list in interactiveLists)
            {
                foreach (InteractiveNode i in list)
                {
                    i.Initialize();
                }
            }

            base.Initialize();
        }

        private void ToggleMusic(bool shouldPlay)
        {
            Player player = GetCurrentPlayer();

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

        /// <summary>
        /// The level's start method. This method is called every time the user starts or restarts the level,
        /// and it's a great place to put code that will be run after initialization, but just before the level starts.
        /// </summary>
        public override void Start()
        {
            if (CurrentRunState == RunState.Playing) return;

            CurrentRunState = RunState.Playing;

            foreach (List<Interactive> list in interactiveLists)
            {
                foreach (InteractiveNode i in list)
                {
                    i.Start();
                }
            }

            // prepare the level's music
            ToggleMusic(true);

            // resume the level's main clock
            Clock.Start();

            base.Start();
        }

        /// <summary>
        /// The level's stop method. This method is called right after the user stops or exits the level.
        /// </summary>
        public override void Stop()
        {
            if (CurrentRunState == RunState.Stopped) return;

            // So the level's timing stuff stays accurate, it might be best to stop the level's clock first.
            Clock.Stop();

            CurrentRunState = RunState.Stopped;

            foreach (List<Interactive> list in interactiveLists)
            {
                foreach (InteractiveNode i in list)
                {
                    i.Stop();
                }
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
            foreach (List<Interactive> list in interactiveLists)
            {
                foreach (InteractiveNode i in list)
                {
                    i.Dispose();
                }
            }

            base.Dispose();
        }
    }
}
