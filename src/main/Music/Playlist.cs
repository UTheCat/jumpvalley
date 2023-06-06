using System.Collections.Generic;
using Godot;

/// <summary>
/// Represents a musical playlist that can hold multiple Songs
/// </summary>
public partial class Playlist
{
    /// <summary>
    /// The number of seconds that the volume transitioning lasts when uninterrupted
    /// </summary>
    public double TransitionTime = 0;

    private List<Song> list = new List<Song>();
    private AudioStreamPlayer musicPlayer = null;

    public void Add(Song s)
    {
        if (!list.Contains(s))
        {
            list.Add(s);
        }
    }

    public void Remove(Song s)
    {
        list.Remove(s);
    }

    public void Play()
    {

    }

    public void Stop()
    {

    }
}