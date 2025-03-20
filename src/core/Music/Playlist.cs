using System;
using System.Collections.Generic;
using Godot;

using UTheCat.Jumpvalley.Core.Tweening;

namespace UTheCat.Jumpvalley.Core.Music
{
    /// <summary>
    /// Represents a musical playlist that can hold multiple Songs
    /// </summary>
    public partial class Playlist : Node, IDisposable
    {
        /// <summary>
        /// Enumerator that specifies how a <see cref="Playlist"/>
        /// should approach opening and closing the audio streams
        /// of its <see cref="Song"/> instances. 
        /// </summary>
        public enum SongStreamHandlingModeFlags
        {
            /// <summary>
            /// Specifies that the <see cref="Playlist"/> shouldn't open or close
            /// the audio streams of <see cref="Song"/> instances it handles 
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Specifies that the <see cref="Playlist"/> should
            /// open a song's audio stream when it's added to the playlist,
            /// and close a song's audio stream when it's removed from the playlist.
            /// </summary>
            AddAndRemove = 1
        }

        /// <summary>
        /// Converts a volume percentage in the range of [0, 1] to the corresponding value in decibels and returns the result
        /// </summary>
        /// <returns></returns>
        public static float VolPercentToDecibels(float percent)
        {
            return Mathf.LinearToDb(percent);
        }

        /// <summary>
        /// Lowest audio volume (in the linear form) that Godot's editor will let you set the volume of a sound to.
        /// <br/>
        /// Such volume shouldn't be audible to humans.
        /// </summary>
        public static float NonAudibleVolume { get; private set; } = Mathf.DbToLinear(-80f);

        /// <summary>
        /// The number of seconds that the volume transitioning lasts when uninterrupted
        /// </summary>
        public double TransitionTime = 0;

        private double _localVolumeScale;
        private double _linearVolume;
        private Song _currentSong;

        private int currentSongIndex = 0;
        private int recentSongIndex = 0;
        private bool handleSongFinishedConnected = false;

        /// <summary>
        /// Multiplier for linear volume that's typically in the range of [0, 1]
        /// <br/>
        /// Can be used to mute or unmute the playlist without affecting the original value of <see cref="LinearVolume"/>
        /// </summary>
        public double LocalVolumeScale
        {
            get => _localVolumeScale;
            set
            {
                _localVolumeScale = value;
                UpdateVolumeViaLinear();
            }
        }

        /// <summary>
        /// Volume of the playlist's music in a linear fashion
        /// </summary>
        public double LinearVolume
        {
            get => _linearVolume;
            set
            {
                _linearVolume = value;
                UpdateVolumeViaLinear();
            }
        }

        /// <summary>
        /// The song that's currently being played in this playlist
        /// </summary>
        public Song CurrentSong
        {
            get => _currentSong;
            private set
            {
                _currentSong = value;
                OnSongChanged(new SongChangedArgs(value));
            }
        }

        /// <summary>
        /// How the playlist is currently opening and closing audio streams
        /// of its songs.
        /// </summary>
        public SongStreamHandlingModeFlags SongStreamHandlingMode;

        /// <summary>
        /// The list of <see cref="Song"/>s being played by this playlist.
        /// </summary>
        protected List<Song> SongList = new List<Song>();

        private AudioStreamPlayer streamPlayer;
        private SceneTreeTween currentTween;

        private void UpdateVolumeViaLinear()
        {
            if (streamPlayer != null)
            {
                streamPlayer.VolumeDb = (float)Mathf.LinearToDb(LinearVolume * LocalVolumeScale);
                //Console.WriteLine("Current volume (gain): " + streamPlayer.VolumeDb);
            }
        }

        public Playlist()
        {
            LinearVolume = NonAudibleVolume;
            LocalVolumeScale = 1;
            SongStreamHandlingMode = SongStreamHandlingModeFlags.AddAndRemove;
        }

        /// <summary>
        /// Adds a song to the playlist
        /// </summary>
        /// <param name="s">The song to add</param>
        public void Add(Song s)
        {
            if (!SongList.Contains(s))
            {
                SongList.Add(s);

                if (SongStreamHandlingMode == SongStreamHandlingModeFlags.AddAndRemove)
                {
                    s.OpenStream();
                }
            }
        }

        /// <summary>
        /// Removes a song from the playlist.
        /// <br/>
        /// <br/>
        /// If the song that gets removed is being played,
        /// the playlist will move onto the next song if there are still songs in the playlist after removal,
        /// or the playlist will stop immediately.
        /// </summary>
        /// <param name="s">The song to remove</param>
        public void Remove(Song s)
        {
            SongList.Remove(s);

            // It makes sense to stop the song if the song being removed
            // is the one being played.
            if (s == CurrentSong)
            {
                if (SongList.Count > 0)
                {
                    // This will still work if there's now only one song in the playlist
                    PlayNextSong();
                }
                else
                {
                    StopImmediately();
                }
            }

            if (SongStreamHandlingMode == SongStreamHandlingModeFlags.AddAndRemove)
            {
                s.CloseStream();
            }
        }

        private void CreateAudioStreamPlayer()
        {
            if (streamPlayer == null)
            {
                streamPlayer = new AudioStreamPlayer();
                streamPlayer.VolumeDb = VolPercentToDecibels(NonAudibleVolume);
                AddChild(streamPlayer);
            }
        }

        private void DisconnectHandleSongFinished()
        {
            if (handleSongFinishedConnected)
            {
                streamPlayer.Finished -= PlayNextSong;
                handleSongFinishedConnected = false;
            }
        }

        private void PlayNextSong()
        {
            // to prevent stack overflow
            if (streamPlayer != null)
            {
                DisconnectHandleSongFinished();
            }

            currentSongIndex++;
            if (currentSongIndex >= SongList.Count)
            {
                currentSongIndex = 0;
            }

            SwitchToSong(currentSongIndex);
        }

        // switches to a song in the playlist by numerical index
        private void SwitchToSong(int index)
        {
            // we don't need to do anything here if there aren't any songs or if this song is already playing
            if (SongList.Count < 1 || (streamPlayer != null && index == recentSongIndex)) { return; }

            Song s = SongList[index];

            bool onlyOneSong = SongList.Count == 1;
            CreateAudioStreamPlayer();
            s.IsLooping = onlyOneSong;
            //s.OpenStream();
            streamPlayer.Stream = s.Stream;

            // If there's more than one song, switch to the next song on finish
            if (onlyOneSong == false && streamPlayer != null && handleSongFinishedConnected == false)
            {
                handleSongFinishedConnected = true;
                streamPlayer.Finished += PlayNextSong;
            }

            // take note of the song change
            CurrentSong = s;
            recentSongIndex = index;

            // play the song
            if (!streamPlayer.Playing)
            {
                streamPlayer.Play();
            }
        }

        private void KillCurrentTween()
        {
            if (currentTween != null)
            {
                currentTween.Dispose();
                currentTween = null;
            }
        }

        private void DisposeCurrentTween()
        {
            if (currentTween == null) { return; };

            currentTween.Dispose();
            currentTween = null;
        }

        private void SetLinearVolumeViaTween(double vol)
        {
            LinearVolume = vol;
            //Console.WriteLine("Set linear volume to " + vol);
        }

        /// <summary>
        /// This method plays the first song in the playlist (the song in index 0) if no song in the playlist
        /// is currently being played.
        /// Otherwise, the playback of the current song is resumed and the linear volume of this playlist
        /// transitions back to (<see cref="LinearVolume"/> * <see cref="LocalVolumeScale"/>).
        /// </summary>
        public void Play()
        {
            if (streamPlayer == null)
            {
                // reset song index if there's nothing playing
                currentSongIndex = 0;
            }
            SwitchToSong(currentSongIndex);

            if (streamPlayer != null)
            {
                /*
                if (!streamPlayer.Playing)
                {
                    streamPlayer.Play();
                }
                */

                if (currentTween == null)
                {
                    currentTween = new SceneTreeTween(TransitionTime, Tween.TransitionType.Linear, Tween.EaseType.Out, GetTree())
                    {
                        InitialValue = NonAudibleVolume,
                        FinalValue = 1
                    };

                    currentTween.OnStep += (object o, float _frac) =>
                    {
                        SceneTreeTween t = (SceneTreeTween)o;
                        SetLinearVolumeViaTween(t.GetCurrentValue());
                    };
                }

                // fade in the current song
                DisconnectTweenFinish();
                currentTween.Speed = 1f;
                currentTween.Resume();

                /*
                KillCurrentTween();
                currentTween = new SceneTreeTween();

                currentTween.TweenMethod(
                    Callable.From<double>(SetLinearVolumeViaTween),
                    LinearVolume, 1, (float)TransitionTime
                );

                currentTween.Finished += () =>
                {
                    DisposeCurrentTween();
                };
                */
            }
        }

        /// <summary>
        /// Stops the playback of the playlist immediately,
        /// skipping the fade out transition that gradually makes the playlist inaudible.
        /// </summary>
        public void StopImmediately()
        {
            if (streamPlayer != null)
            {
                streamPlayer.Stop();
                streamPlayer.Stream = null;

                DisconnectHandleSongFinished();

                streamPlayer.Dispose();
                streamPlayer = null;
            }
            else
            {
                handleSongFinishedConnected = false;
            }

            if (currentTween != null)
            {
                currentTween.Dispose();
                currentTween = null;
            }

            // free memory used by CurrentSong's stream
            /*
            Song song = CurrentSong;
            if (song != null)
            {
                song.CloseStream();
            }
            */

            RaiseStoppedEvent();
        }

        /// <summary>
        /// This method gradually transitions the playlist's volume to be inaudible,
        /// and then stops the playlist once that transition is done.
        /// </summary>
        public void Stop()
        {
            /*
            KillCurrentTween();
            currentTween = streamPlayer.CreateTween();

            currentTween.TweenMethod(
                Callable.From<double>(SetLinearVolumeViaTween),
                LinearVolume, NonAudibleVolume, (float)TransitionTime
            );
            */

            if (currentTween != null)
            {
                ConnectTweenFinish();
                currentTween.Speed = -1f;

                // fade out the current song
                currentTween.Resume();
            }
        }

        private bool tweenFinishConnected = false;

        private void ConnectTweenFinish()
        {
            if (!tweenFinishConnected)
            {
                tweenFinishConnected = true;
                currentTween.OnFinish += HandleTweenFinish;
            }
        }

        private void DisconnectTweenFinish()
        {
            if (tweenFinishConnected)
            {
                currentTween.OnFinish -= HandleTweenFinish;
                tweenFinishConnected = false;
            }
        }

        protected void HandleTweenFinish(object _o, EventArgs _e)
        {
            // currentTween's speed is less than 0 only when the song is fading out
            if (currentTween != null && currentTween.Speed < 0f)
            {
                DisconnectTweenFinish();
                DisposeCurrentTween();
                StopImmediately();
            }
        }

        /// <summary>
        /// Stops and disposes of this Playlist.
        /// </summary>
        public new void Dispose()
        {
            StopImmediately();
            QueueFree();
            base.Dispose();
        }

        /// <summary>
        /// Called when the currently playing song changes.
        /// </summary>
        public event EventHandler<SongChangedArgs> SongChanged;

        // Invocation method for SongChanged
        protected void OnSongChanged(SongChangedArgs args)
        {
            // just in case
            EventHandler<SongChangedArgs> songChangedEvent = SongChanged;

            // if no one is currently listening to the event, songChangedEvent will be null
            if (songChangedEvent != null)
            {
                songChangedEvent(this, args);
            }
        }

        /// <summary>
        /// Called when the playlist stops the playback of its music.
        /// If a fade transition is applied to fade out the currently playing song, this event will get called
        /// after the transition has completed.
        /// </summary>
        public event EventHandler Stopped;

        protected void RaiseStoppedEvent()
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
