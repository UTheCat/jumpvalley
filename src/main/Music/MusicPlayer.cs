using System;
using Godot;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Plays music that is split into playlists (see <see cref="Playlist"/>)
    /// <br/>
    /// This class applies a fade transition when the user wants to switch between playlists.
    /// <br/>
    /// Each instance of a MusicPlayer can only play one song at a time at most.
    /// </summary>
    public partial class MusicPlayer : Node
    {
        /*
        private static void StopPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                playlist.Stop();
            }
        }
        */

        private Playlist _currentPlaylist;
        private Playlist _primaryPlaylist;
        private bool _isPlaying;

        /// <summary>
        /// Stops the specified playlist and removes it from the MusicPlayer's list of children
        /// </summary>
        /// <param name="playlist"></param>
        private void StopPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                playlist.Stop();
                
                if (playlist.GetParent() == this)
                {
                    RemoveChild(playlist);
                }
            }
        }

        // ignore the sender argument as the actual sender should be this instance of MusicPlayer
        private void HandlePlaylistSongChange(object _sender, SongChangedArgs args)
        {
            OnMusicPlayerSongChanged(args);
        }

        /// <summary>
        /// The playlist that's currently being played by the MusicPlayer
        /// </summary>
        public Playlist CurrentPlaylist
        {
            get => _currentPlaylist;
            private set
            {
                // If a different playlist is requesting to be played, make sure to stop the current one before changing it
                if (value == null)
                {
                    Playlist oldPlaylist = _currentPlaylist;
                    _currentPlaylist = null;

                    // tell subscribers of MusicPlayer.SongChanged that there's no song playing anymore
                    // (this is done automatically by stopping the playlist)
                    StopPlaylist(oldPlaylist);
                    if (oldPlaylist != null)
                    {
                        oldPlaylist.SongChanged -= HandlePlaylistSongChange;
                        //_currentPlaylist.Stop();
                        //RemoveChild(_currentPlaylist);
                    }
                    oldPlaylist = value;
                }
                else
                {
                    // First, disconnect HandlePlaylistSongChange() from the old playlist. This is because we don't want SongChanged to be raised twice at the same time for the same song change.
                    if (_currentPlaylist != null)
                    {
                        _currentPlaylist.SongChanged -= HandlePlaylistSongChange;

                        StopPlaylist(_currentPlaylist);
                        //RemoveChild(_currentPlaylist);
                    }

                    _currentPlaylist = value;

                    // connect to the new playlist's SongChanged event
                    value.SongChanged += HandlePlaylistSongChange;

                    if (!value.IsInsideTree())
                    {
                        AddChild(value);
                    }

                    // play the new playlist (this is where MusicPlayer.SongChanged will get raised for the song change)
                    value.Play();
                }
            }
        }

        /// <summary>
        /// The MusicPlayer's primary playlist. If set, this playlist will be played whenever <see cref="IsPlaying"/> is set to true.
        /// </summary>
        public Playlist PrimaryPlaylist
        {
            get => _primaryPlaylist;
            set
            {
                _primaryPlaylist = value;
                RefreshPlayback();
            }
        }

        /// <summary>
        /// Whether or not the playlist is currently playing any music
        /// <br/>
        /// Setting this value to false will stop the music (with the fade transition).
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                RefreshPlayback();
            }
        }

        // playback update function
        private void RefreshPlayback()
        {
            if (IsPlaying)
            {
                CurrentPlaylist = PrimaryPlaylist;
            }
            else
            {
                CurrentPlaylist = null;
            }
        }

        /// <summary>
        /// Event that's raised when the song currently being played by this instance of MusicPlayer changes.
        /// <br/>
        /// This event is meant to wrap individual raisings of Playlist's SongChanged event in order to make it easier for developers to adapt to song changes regardless of what <see cref="Playlist"/> is being played.
        /// </summary>
        public event EventHandler<SongChangedArgs> SongChanged;

        // Invocation method for MusicPlayer's SongChanged
        protected void OnMusicPlayerSongChanged(SongChangedArgs args)
        {
            EventHandler<SongChangedArgs> raisedEvent = SongChanged;
            if (raisedEvent != null)
            {
                raisedEvent(this, args);
            }
        }
    }
}
