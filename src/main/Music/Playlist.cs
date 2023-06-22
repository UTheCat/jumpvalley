using System;
using System.Collections.Generic;
using Godot;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a musical playlist that can hold multiple Songs
    /// </summary>
    public partial class Playlist : Node, IDisposable
    {
        /// <summary>
        /// Converts a volume percentage in the range of [0, 1] to the corresponding value in decibels and returns the result
        /// </summary>
        /// <returns></returns>
        public static float VolPercentToDecibels(float percent)
        {
            return Mathf.LinearToDb(percent);
        }

        /// <summary>
        /// Lowest audio volume (in the linear percentage form) that Godot's editor will let you set the volume of a sound to.
        /// <br/>
        /// Such volume shouldn't be audible to humans.
        /// </summary>
        public static float NonAudiblePercent { get; private set; } = Mathf.DbToLinear(-80f);

        /// <summary>
        /// The number of seconds that the volume transitioning lasts when uninterrupted
        /// </summary>
        public double TransitionTime = 0;

        private double _localVolumeScale;
        private double _linearVolume;
        private Song _currentSong;

        private int currentSongIndex = 0;
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

        public Song CurrentSong
        {
            get => _currentSong;
            set
            {
                _currentSong = value;
                OnSongChanged(new SongChangedArgs(value));
            }
        }

        private List<Song> list = new List<Song>();
        private AudioStreamPlayer streamPlayer;
        private Tween currentTween;

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
            LinearVolume = NonAudiblePercent;
            LocalVolumeScale = 1;
        }

        /// <summary>
        /// Adds a song to the playlist
        /// </summary>
        /// <param name="s">The song to add</param>
        public void Add(Song s)
        {
            if (!list.Contains(s))
            {
                list.Add(s);
            }
        }

        /// <summary>
        /// Removes a song from the playlist
        /// </summary>
        /// <param name="s">The song to remove</param>
        public void Remove(Song s)
        {
            list.Remove(s);
        }

        /*
        private void RemoveAudioStream()
        {
            if (streamPlayer != null)
            {
                RemoveChild(streamPlayer);
                streamPlayer.Dispose();
                streamPlayer = null;
            }
        }
        */

        private void CreateAudioStream()
        {
            if (streamPlayer == null)
            {
                streamPlayer = new AudioStreamPlayer();
                streamPlayer.VolumeDb = VolPercentToDecibels(NonAudiblePercent);
                AddChild(streamPlayer);
            }
        }

        private void DisconnectHandleSongFinished()
        {
            if (handleSongFinishedConnected)
            {
                streamPlayer.Finished -= HandleSongFinish;
                handleSongFinishedConnected = false;
            }
        }

        private void HandleSongFinish()
        {
            // to prevent stack overflow
            if (streamPlayer != null)
            {
                DisconnectHandleSongFinished();
            }

            currentSongIndex++;
            if (currentSongIndex >= list.Count)
            {
                currentSongIndex = 0;
            }

            SwitchToSong(currentSongIndex);
        }

        // switches to a song in the playlist by numerical index
        private void SwitchToSong(int index)
        {
            // we don't need to do anything here if there aren't any songs or if this song is already playing
            if (list.Count < 1 || (streamPlayer != null && index == currentSongIndex)) { return; }

            Song s = list[index];

            bool onlyOneSong = list.Count == 1;
            CreateAudioStream();
            s.IsLooping = onlyOneSong;
            s.OpenStream();
            streamPlayer.Stream = s.Stream;

            // If there's more than one song, switch to the next song on finish
            if (!onlyOneSong && streamPlayer != null && handleSongFinishedConnected == false)
            {
                handleSongFinishedConnected = true;
                streamPlayer.Finished += HandleSongFinish;
            }

            // take note of the song change
            CurrentSong = s;
        }

        private void KillCurrentTween()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
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

        public void Play()
        {
            if (streamPlayer == null)
            {
                //CreateAudioStream();

                // reset song index if there's nothing playing
                currentSongIndex = 0;
            }
            SwitchToSong(currentSongIndex);

            if (streamPlayer != null)
            {
                streamPlayer.Play();
                KillCurrentTween();
                currentTween = streamPlayer.CreateTween();

                currentTween.TweenMethod(
                    Callable.From<double>(SetLinearVolumeViaTween),
                    LinearVolume, 1, (float)TransitionTime
                );

                currentTween.Finished += () =>
                {
                    DisposeCurrentTween();
                };
            }
        }

        /// <summary>
        /// Stops the playback of the playlist immediately, skipping the fade out transition
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

            // free memory used by CurrentSong's stream
            Song song = CurrentSong;
            if (song != null)
            {
                song.CloseStream();
            }

            RaiseStoppedEvent();
        }

        public void Stop()
        {
            KillCurrentTween();
            currentTween = streamPlayer.CreateTween();

            currentTween.TweenMethod(
                Callable.From<double>(SetLinearVolumeViaTween),
                LinearVolume, NonAudiblePercent, (float)TransitionTime
            );
            currentTween.Finished += () =>
            {
                DisposeCurrentTween();
                StopImmediately();
            };
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

        public new void Dispose()
        {
            base.Dispose();
            StopImmediately();
        }
    }
}
