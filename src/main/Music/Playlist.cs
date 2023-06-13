using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Represents a musical playlist that can hold multiple Songs
/// </summary>
public partial class Playlist : Node
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
            updateVolumeViaLinear();
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
            updateVolumeViaLinear();
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
    private AudioStreamPlayer musicPlayer;
    private Tween currentTween;

    private void updateVolumeViaLinear()
    {
        if (musicPlayer != null)
        {
            musicPlayer.VolumeDb = (float)Mathf.LinearToDb(LinearVolume * LocalVolumeScale);
            //Console.WriteLine("Current volume (gain): " + musicPlayer.VolumeDb);
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

    private void removeAudioStream()
    {
        if (musicPlayer != null)
        {
            RemoveChild(musicPlayer);
            musicPlayer.Dispose();
            musicPlayer = null;
        }
    }

    private void createAudioStream()
    {
        if (musicPlayer == null)
        {
            musicPlayer = new AudioStreamPlayer();
            musicPlayer.VolumeDb = VolPercentToDecibels(NonAudiblePercent);
            AddChild(musicPlayer);
        }
    }

    private void switchToSong(Song s)
    {
        createAudioStream();
        s.IsLooping = list.Count == 1;
        s.OpenStream();
        musicPlayer.Stream = s.Stream;

        /*
        Resource streamRes = GD.Load(s.FilePath); // stream resource
        string path = s.FilePath;

        // make the playlist loop the same song over again if only one song is present
        bool canLoop = list.Count == 1;
        */

        /*
        if (streamRes.GetType() == typeof(AudioStreamWav))
        {

        }

        */

        /*
        const string ERROR_MSG = "The format of the song file is invalid. The file extension must be .wav, .ogg, or .mp3.";

        if ((streamRes is AudioStream) == false)
        {
            throw new System.Exception(ERROR_MSG);
        }

        if (streamRes.GetType() == typeof(AudioStreamWav))
        {
            AudioStreamWav stream = (AudioStreamWav)streamRes;
            //stream.ResourcePath = path;

            if (canLoop)
            {
                stream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
            }
            else
            {
                stream.LoopMode = AudioStreamWav.LoopModeEnum.Disabled;
            }

            musicPlayer.Stream = stream;
        }
        else if (path.EndsWith(".ogg"))
        {
            AudioStreamOggVorbis stream = (AudioStreamOggVorbis)streamRes;
            //stream.ResourcePath = path;
            stream.Loop = canLoop;
            musicPlayer.Stream = stream;
        }
        else if (path.EndsWith(".mp3"))
        {
            AudioStreamMP3 stream = (AudioStreamMP3)streamRes;
            //stream.ResourcePath = path;
            stream.Loop = canLoop;
            musicPlayer.Stream = stream;
        }
        else
        {
            throw new System.Exception(ERROR_MSG);
        }
        */

        // take note of the song change
        CurrentSong = s;
    }

    private void killCurrentTween()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
            currentTween = null;
        }
    }

    private void disposeCurrentTween()
    {
        if (currentTween == null) { return; };

        currentTween.Dispose();
        currentTween = null;
    }

    private void setLinearVolumeViaTween(double vol)
    {
        LinearVolume = vol;
        //Console.WriteLine("Set linear volume to " + vol);
    }

    public void Play()
    {
        createAudioStream();
        switchToSong(list[0]);
        musicPlayer.Play();
        killCurrentTween();
        currentTween = musicPlayer.CreateTween();
        /*
        currentTween.TweenProperty(
            musicPlayer,
            "volume_db",
            VolPercentToDecibels(1),
            TransitionTime
        );
        */

        currentTween.TweenMethod(
            Callable.From<double>(setLinearVolumeViaTween),
            LinearVolume, 1, (float)TransitionTime
        );

        currentTween.Finished += () =>
        {
            disposeCurrentTween();
        };
    }

    private void stopImmediately()
    {
        musicPlayer.Stop();
        musicPlayer.Stream = null;
        musicPlayer.Dispose();
        musicPlayer = null;

        // free memory used by CurrentSong's stream
        Song song = CurrentSong;
        if (song != null)
        {
            song.CloseStream();
        }
    }

    public void Stop()
    {
        killCurrentTween();
        currentTween = musicPlayer.CreateTween();

        /*
        currentTween.TweenProperty(
            musicPlayer,
            "volume_db",
            VolPercentToDecibels(NonAudiblePercent),
            TransitionTime
        );
        */
        currentTween.TweenMethod(
            Callable.From<double>(setLinearVolumeViaTween),
            LinearVolume, NonAudiblePercent, (float)TransitionTime
        );
        currentTween.Finished += () =>
        {
            disposeCurrentTween();
            stopImmediately();
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
}
