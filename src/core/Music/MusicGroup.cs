using Godot;
using System;
using System.Collections.Generic;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Handles a <see cref="Playlist"/> that's in the form of nodes in a scene tree.
    /// <br/>
    /// Each MusicGroup node must have a node as a direct child named "Music" that lists the directory (folder) paths of songs in its metadata.
    /// <br/><br/>
    /// There are two ways to specify songs that should be played:
    /// <br/><br/>
    /// <b>songs array in metadata</b>
    /// <br/>
    /// The metadata (as mentioned above) has an array named `songs` that specifies the songs to play.
    /// The array's C# type should be <see cref="Godot.Collections.Array"/>.
    /// <br/><br/>
    /// <b>Incremental songN metadata entries</b>
    /// <br/>
    /// Multiple metadata entries are specified in this format:
    /// <br/>
    /// Entry name: <c>songN</c> where N represents the numerical index of the song package directory starting from 1. Songs are played in numerical order.
    /// <br/>
    /// Entry value: The absolute path to the song's directory. (The value's type must be string.)
    /// </summary>
    public partial class MusicGroup : Playlist, IDisposable
    {
        /// <summary>
        /// The prefix of the song metadata entry name within <see cref="MusicListNode"/>
        /// </summary>
        public static readonly string SONG_METADATA_ENTRY_PREFIX = "song";

        /// <summary>
        /// Name of the metadata entry storing the songs array
        /// </summary>
        public static readonly string SONGS_META_NAME = "songs";

        /// <summary>
        /// The name of the node that contains the list of songs as metadata
        /// </summary>
        public static readonly string MUSIC_LIST_NODE_NAME = "Music";

        /// <summary>
        /// The actual node that the MusicGroup is handling
        /// </summary>
        public Node ActualNode { get; private set; }

        /// <summary>
        /// The node that uses node metadata to list the directory paths of song packages
        /// </summary>
        public Node MusicListNode { get; private set; }

        /// <summary>
        /// The song packages being played by this <see cref="MusicGroup"/>
        /// </summary>
        public List<SongPackage> SongPackages { get; private set; } = new List<SongPackage>();

        /// <summary>
        /// Creates an instance of <see cref="MusicGroup"/> to represent a specified node
        /// </summary>
        /// <param name="node">The node to represent</param>
        public MusicGroup(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("Specified " + nameof(node) + " is null. Please specify an actual node.");
            }

            ActualNode = node;
            MusicListNode = node.GetNode(MUSIC_LIST_NODE_NAME);

            node.AddChild(this);

            // Add songs from package paths specified in songs array
            if (MusicListNode.HasMeta(SONGS_META_NAME))
            {
                Godot.Collections.Array songs = MusicListNode.GetMeta(SONGS_META_NAME).As<Godot.Collections.Array>();

                if (songs != null)
                {
                    foreach (Variant song in songs)
                    {
                        if (song.VariantType == Variant.Type.String)
                        {
                            string packagePath = song.As<string>();
                            if (!string.IsNullOrEmpty(packagePath))
                            {
                                SongPackages.Add(new SongPackage(packagePath));
                            }
                        }
                    }
                }
            }
            else
            {
                // Add the song packages (legacy behavior)
                int packageIndex = 1;
                while (true)
                {
                    string metaName = SONG_METADATA_ENTRY_PREFIX + packageIndex;

                    if (!MusicListNode.HasMeta(metaName))
                    {
                        break;
                    }

                    Variant meta = MusicListNode.GetMeta(metaName);
                    string songDir = meta.As<string>();

                    if (string.IsNullOrEmpty(songDir))
                    {
                        break;
                    }

                    SongPackages.Add(new SongPackage(songDir));
                    packageIndex++;
                }
            }

            // Add the corresponding songs to the playlist
            for (int i = 0; i < SongPackages.Count; i++)
            {
                Add(SongPackages[i].CreateSongInstance());
            }
        }

        /// <summary>
        /// Disposes of this <see cref="MusicGroup"/>, including all of the <see cref="Song"/> instances associated with it.
        /// </summary>
        public new void Dispose()
        {
            for (int i = 0; i < SongList.Count; i++)
            {
                SongList[i].Dispose();
            }

            base.Dispose();
        }
    }
}
