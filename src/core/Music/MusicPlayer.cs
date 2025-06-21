using System;
using System.Collections.Generic;
using Godot;

namespace UTheCat.Jumpvalley.Core.Music
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
        /// <see cref="Playlist"/>s currently being handled by this <see cref="MusicPlayer"/>. Even though
        /// <see cref="MusicPlayer"/> handles <see cref="PrimaryPlaylist"/>, <see cref="PrimaryPlaylist"/> isn't inclued
        /// in this list by default.  
        /// </summary>
        public List<Playlist> Playlists { get; private set; }

        /// <summary>
        /// A list of playlists that are currently fading out.
        /// <br/>
        /// In case a playlist wants to be played again while it is fading out,
        /// this list can be useful for locating a playlist to stop it from fully fading out.
        /// </summary>
        private List<Playlist> fadingOutPlaylists = new List<Playlist>();

        private void RemovePlaylistInternal(Playlist playlist)
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

            RemovePlaylistInternal(playlist);
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
                    //RemovePlaylistInternal(playlist);
                }
                else
                {
                    //playlist.Stopped += HandlePlaylistStop;
                    playlist.TransitionTime = TransitionTime;
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

                    // if the user of this class requested to override transition time with the corresponding value set in this music player, make sure to do so
                    if (OverrideTransitionTime) value.TransitionTime = TransitionTime;

                    // connect to the new playlist's SongChanged event
                    value.SongChanged += HandlePlaylistSongChange;

                    /*
                    if (!value.IsInsideTree())
                    {
                        AddChild(value);
                    }
                    */

                    // in case this playlist wanting to be played is currently fading out, stop it from being removed
                    if (fadingOutPlaylists.Contains(value))
                    {
                        DisconnectHandlePlaylistStop(value);

                        // also signal to subscribers of SongChanged that the song currently in focus of the music player has changed back
                        HandlePlaylistSongChange(value, new SongChangedArgs(value.CurrentSong));
                    }

                    // play the new playlist (this is where MusicPlayer.SongChanged will get raised for the song change)
                    value.Play();
                }
            }
        }

        /// <summary>
        /// Whether or not the a playlist's volume transition time will get set to <see cref="MusicPlayer.TransitionTime"/> whenever it gets played or faded out by this music player.
        /// </summary>
        public bool OverrideTransitionTime = false;

        /// <summary>
        /// Playlists played by this music player will have their volume transition time set to the value of this variable whenever <see cref="OverrideTransitionTime"/> is set to true, 
        /// </summary>
        public double TransitionTime = 0;

        private bool _overrideLocalVolumeScale = false;

        /// <summary>
        /// Whether or not <see cref="Playlist.LocalVolumeScale"/> will get set to <see cref="VolumeScale"/> for <see cref="PrimaryPlaylist"/> and playlists in the <see cref="Playlist"/> list.
        /// Setting this to true automatically causes the playlists' LocalVolumeScale to be set in the aforementioned manner.
        /// </summary>
        public bool OverrideLocalVolumeScale
        {
            get => _overrideLocalVolumeScale;
            set
            {
                if (value) PerformLocalVolumeScaleOverride(_volumeScale);
            }
        }

        private double _volumeScale = 1;

        /// <summary>
        /// <see cref="PrimaryPlaylist"/> and playlists in the <see cref="Playlist"/> list will have their LocalVolumeScale set to the value of this variable whenever <see cref="OverrideLocalVolumeScale"/> is set to true, 
        /// </summary>
        public double VolumeScale
        {
            get => _volumeScale;
            set
            {
                _volumeScale = value;

                if (OverrideLocalVolumeScale) PerformLocalVolumeScaleOverride(value);
            }
        }

        /// <summary>
        /// Whether or not playlist handled by this music player
        /// should have their <see cref="Playlist.SongStreamHandlingMode"/>
        /// property overriden to the value of this music player's
        /// <see cref="SongStreamHandlingMode"/> variable. 
        /// </summary>
        public bool OverrideSongStreamHandlingMode = false;

        private Playlist.SongStreamHandlingModeFlags _songStreamHandlingMode;

        /// <summary>
        /// The song stream handling mode that the playlists handled
        /// by this music player should use if
        /// <see cref="OverrideSongStreamHandlingMode"/> is set to true. 
        /// </summary>
        public Playlist.SongStreamHandlingModeFlags SongStreamHandlingMode
        {
            get => _songStreamHandlingMode;
            set
            {
                _songStreamHandlingMode = value;

                if (OverrideSongStreamHandlingMode)
                {
                    Playlist currentPlaylist = CurrentPlaylist;
                    if (currentPlaylist != null)
                    {
                        currentPlaylist.SongStreamHandlingMode = value;
                    }

                    foreach (Playlist p in Playlists)
                    {
                        p.SongStreamHandlingMode = value;
                    }
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
                if (value != null) ApplyOverrides(value);
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
        /// Whether or not this <see cref="MusicPlayer"/> should add/remove this music player
        /// as a parent when the <see cref="AddPlaylist"/> and <see cref="RemovePlaylist"/>
        /// methods are called.
        /// </summary>
        public bool ShouldSetPlaylistParent;

        public MusicPlayer()
        {
            Playlists = new List<Playlist>();
            ShouldSetPlaylistParent = false;
        }

        private void PerformLocalVolumeScaleOverride(double vol)
        {
            if (PrimaryPlaylist != null) PrimaryPlaylist.LocalVolumeScale = vol;

            foreach (Playlist p in Playlists)
            {
                p.LocalVolumeScale = vol;
            }
        }

        /// <summary>
        /// Method that can be overriden to define how playlists added to this music player
        /// should have their properties overriden.
        /// By default, overrides defined in the <see cref="MusicPlayer"/> class are applied here.
        /// </summary>
        /// <param name="playlist">The playlist to apply overrides to</param>
        public virtual void ApplyOverrides(Playlist playlist)
        {
            if (OverrideTransitionTime)
            {
                playlist.TransitionTime = TransitionTime;
            }
            if (OverrideLocalVolumeScale)
            {
                playlist.LocalVolumeScale = VolumeScale;
            }
            if (OverrideSongStreamHandlingMode)
            {
                playlist.SongStreamHandlingMode = SongStreamHandlingMode;
            }
        }

        /// <summary>
        /// Removes a playlist from this <see cref="MusicPlayer"/>.
        /// This will cause this <see cref="MusicPlayer"/>
        /// to no longer make changes to the removed <see cref="Playlist"/>. 
        /// </summary>
        /// <param name="playlist"></param>
        public void RemovePlaylist(Playlist playlist)
        {
            if (Playlists.Contains(playlist))
            {
                if (ShouldSetPlaylistParent == true && playlist.GetParent() == this)
                {
                    RemoveChild(playlist);
                }

                Playlists.Remove(playlist);
            }
        }

        /// <summary>
        /// Adds a playlist to this <see cref="MusicPlayer"/>.
        /// This will let this <see cref="MusicPlayer"/> make some
        /// changes to the added playlist.
        /// </summary>
        /// <param name="playlist"></param>
        public void AddPlaylist(Playlist playlist)
        {
            if (Playlists.Contains(playlist)) return;

            Playlists.Add(playlist);

            ApplyOverrides(playlist);

            if (ShouldSetPlaylistParent == true && playlist.GetParent() == null)
            {
                AddChild(playlist);
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
