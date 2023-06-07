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
    /// The number of seconds that the volume transitioning lasts when uninterrupted
    /// </summary>
    public double TransitionTime = 0;

    private List<Song> list = new List<Song>();
    private AudioStreamPlayer musicPlayer;
    private Tween currentTween;

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
            musicPlayer.VolumeDb = VolPercentToDecibels(0);
            AddChild(musicPlayer);
        }
    }

    private void switchToSong(Song s)
    {
        createAudioStream();
        //AudioStream stream;
        string path = s.FilePath;

        // make the playlist loop the same song over again if only one song is present
        bool canLoop = list.Count == 1;

        if (path.EndsWith(".wav"))
        {
            AudioStreamWav stream = new AudioStreamWav();
            stream.ResourcePath = path;

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
            AudioStreamOggVorbis stream = new AudioStreamOggVorbis();
            stream.ResourcePath = path;
            stream.Loop = canLoop;
            musicPlayer.Stream = stream;
        }
        else if (path.EndsWith(".mp3"))
        {
            AudioStreamMP3 stream = new AudioStreamMP3();
            stream.ResourcePath = path;
            stream.Loop = canLoop;
            musicPlayer.Stream = stream;
        }
        else
        {
            throw new System.Exception("The format of the song file is invalid. The file extension must be .wav, .ogg, or .mp3.");
        }
    }

    private void killCurrentTween()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
            currentTween = null;
        }
    }

    public void Play()
    {
        createAudioStream();
        switchToSong(list[0]);
        musicPlayer.Play();
        killCurrentTween();
        currentTween = musicPlayer.CreateTween();
        currentTween.TweenProperty(
            musicPlayer,
            "volume_db",
            VolPercentToDecibels(1),
            TransitionTime
        );
        currentTween.Finished += () =>
        {
            currentTween = null;
        };
    }

    public void Stop()
    {
        killCurrentTween();
        currentTween = musicPlayer.CreateTween();
        currentTween.TweenProperty(
            musicPlayer,
            "volume_db",
            VolPercentToDecibels(0),
            TransitionTime
        );
        currentTween.Finished += () =>
        {
            currentTween = null;
            musicPlayer.Stop();
            musicPlayer.Stream.Dispose();
            musicPlayer.Stream = null;
            musicPlayer.Dispose();
            musicPlayer = null;
        };
    }
}