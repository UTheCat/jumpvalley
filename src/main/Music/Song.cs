using Godot;
using System.IO;

/// <summary>
/// Represents a single song associated with a file, along with some metadata
/// </summary>
public partial class Song
{
    private bool _isLooping = false;

    public Song(string filePath, string name, string artists, string album)
    {
        FilePath = filePath;
        Name = name;
        Artists = artists;
        Album = album;
    }

    public Song() { }

    /// <summary>
    /// The actual audio stream that contains the sound data for playback by an <see cref="AudioStreamPlayer"/>
    /// </summary>
    public AudioStream Stream
    {
        get; private set;
    }

    /// <summary>
    /// Whether or not the song is looping
    /// </summary>
    public bool IsLooping
    {
        get => _isLooping;
        set
        {
            _isLooping = value;
            updateLoop();
        }
    }

    /// <summary>
    /// The file path to the song
    /// </summary>
    public string FilePath = null;

    /// <summary>
    /// The name of the song
    /// </summary>
    public string Name = null;

    /// <summary>
    /// The artists that made the song
    /// </summary>
    public string Artists = null;

    /// <summary>
    /// The album the song belongs to
    /// </summary>
    public string Album = null;

    // update function for IsLooping
    private void updateLoop()
    {
        if (Stream is AudioStreamWav sWav)
        {
            if (IsLooping)
            {
                sWav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
            }
            else
            {
                sWav.LoopMode = AudioStreamWav.LoopModeEnum.Disabled;
            }
        }
        else if (Stream is AudioStreamOggVorbis sOgg)
        {
            sOgg.Loop = IsLooping;
        }
        else if (Stream is AudioStreamMP3 sMp3)
        {
            sMp3.Loop = IsLooping;
        }
    }

    /// <summary>
    /// Attempts to open up an AudioStream for the given <see cref="FilePath"/>
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the file under the given <see cref="FilePath"/> couldn't be found or when the file path is invalid.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// Thrown when the file was found, but the data format of the file is invalid.
    /// </exception>
    public void OpenStream()
    {
        // close the previous stream if there is one
        CloseStream();

        // try loading the file
        Resource resource = GD.Load(FilePath);

        if (resource == null)
        {
            throw new FileNotFoundException("The file path of the song is invalid. Please make sure that such file path is correct and is an absolute file path.");
        }

        // update Stream variable
        if (resource is AudioStreamWav || resource is AudioStreamOggVorbis || resource is AudioStreamMP3)
        {
            Stream = (AudioStream)resource;
            updateLoop();
        }
        else
        {
            throw new InvalidDataException("The data format of the song file is invalid. Please check that the song's file format is either WAV, OGG, or MP3.");
        }
    }
    
    /// <summary>
    /// Closes the AudioStream associated with this Song instance
    /// </summary>
    public void CloseStream()
    {
        if (Stream != null)
        {
            Stream.Dispose();
            Stream = null;
        }
    }
}
