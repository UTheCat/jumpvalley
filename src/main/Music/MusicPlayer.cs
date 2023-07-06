using System;
using System.Collections.Generic;
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
    public partial class MusicPlayer : Node, IDisposable
    {
        private Playlist _currentPlaylist;
        private Playlist _primaryPlaylist;
        private bool _isPlaying;

        /// <summary>
        /// A list of playlists that are currently fading out.
        /// <br/>
        /// In case a playlist wants to be played again while it is fading out,
        /// this list can be useful for locating a playlist to stop it from fully fading out.
        /// </summary>
        private List<Playlist> fadingOutPlaylists = new List<Playlist>();

        private void RemovePlaylist(Playlist playlist)
        {
            if (playlist.GetParent() == this)
            {
                RemoveChild(playlist);
            }
        }

        private void DisconnectHandlePlaylistStop(Playlist playlist)
        {
            playlist.Stopped -= HandlePlaylistStop;
            fadingOutPlaylists.Remove(playlist);
        }

        private void HandlePlaylistStop(object o, EventArgs _e)
        {
            Playlist playlist = (Playlist)o;

            // disconnect after this event handler has been called
            DisconnectHandlePlaylistStop(playlist);

            RemovePlaylist(playlist);
        }

        /// <summary>
        /// Stops the specified playlist and removes it from the MusicPlayer's list of children
        /// </summary>
        /// <param name="playlist"></param>
        private void StopPlaylist(Playlist playlist, bool immediateStop = false)
        {
            if (playlist != null)
            {
                if (immediateStop)
                {
                    playlist.StopImmediately();
                    RemovePlaylist(playlist);
                }
                else
                {
                    playlist.Stopped += HandlePlaylistStop;
                    fadingOutPlaylists.Add(playlist);
                    playlist.Stop();
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
            protected set
            {
                if (_currentPlaylist == value) return;

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

                    // in case this playlist wanting to be played is currently fading out, stop it from being removed
                    if (fadingOutPlaylists.Contains(value))
                    {
                        DisconnectHandlePlaylistStop(value);

                        // also signal to subscribers of SongChanged that the song currently in focus of the music player has changed back
                        HandlePlaylistSongChange(value, new SongChangedArgs(value.CurrentSong));
                    }

                    // play the new playlist (this is where MusicPlayer.SongChanged will get raised for the song change)
                    if (OverrideTransitionTime)
                    {
                        value.TransitionTime = TransitionTime;
                    }

                    value.Play();
                }
            }
        }

        /// <summary>
        /// Whether or not the a playlist's volume transition time will get set to <see cref="MusicPlayer.TransitionTime"/> whenever it gets played by this music player.
        /// </summary>
        public bool OverrideTransitionTime = false;

        /// <summary>
        /// Playlists played by this music player will have their volume transition time set to the value of this variable whenever <see cref="OverrideTransitionTime"/> is set to true, 
        /// </summary>
        public double TransitionTime = 0;

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

        /// <summary>
        /// Updates the playback status of the music
        /// </summary>
        protected virtual void RefreshPlayback()
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

        public new void Dispose()
        {
            StopPlaylist(CurrentPlaylist, true);
            QueueFree();
            base.Dispose();
        }
    }
}
