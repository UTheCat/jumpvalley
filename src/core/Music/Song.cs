using Godot;
using System.IO;

using Jumpvalley.Audio;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a single song associated with a file, along with some metadata
    /// </summary>
    public partial class Song : System.IDisposable
    {
        private bool _isLooping = false;

        /// <summary>
        /// Resource path of the stream that's requesting to be or currently open.
        /// <br/>
        /// This is kept track of to ensure that OpenStream() doesn't set the value of <see cref="Stream"/> to an AudioStream with an invalid resource path.
        /// </summary>
        private string streamResPath = null;

        /// <summary>
        /// Info about the song
        /// </summary>
        public SongInfo Info { get; private set; }

        public Song(SongInfo info)
        {
            Info = info;
        }

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
                UpdateLoop();
            }
        }

        // update function for IsLooping
        private void UpdateLoop()
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
        /// Attempts to open up an AudioStream for the audio file located at <see cref="Info.AudioPath"/> 
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file at the given file path couldn't be found or when the file path is invalid.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Thrown when the file was found, but the data format of the file is invalid.
        /// </exception>
        public void OpenStream()
        {
            if (Stream == null)
            {
                // This should be set before trying to load the corresponding stream so that once the stream is loaded, we can check if the
                // the stream has the correct resource path
                streamResPath = Info.AudioPath;

                // try loading the file
                AudioStreamReader audioStreamReader = new AudioStreamReader(streamResPath);
                AudioStream audioStream = audioStreamReader.Stream;

                if (streamResPath == null || !streamResPath.Equals(audioStream.ResourcePath))
                {
                    //audioStream.Free();
                    audioStream.Dispose();

                    return;
                }

                Stream = audioStream;
                UpdateLoop();

                /*
                Resource resource = GD.Load(streamResPath);

                if (resource == null)
                {
                    throw new FileNotFoundException("The file path of the song is invalid. Please make sure that such file path is correct and is an absolute file path.");
                }

                // update Stream variable
                if (resource is AudioStreamWav || resource is AudioStreamOggVorbis || resource is AudioStreamMP3)
                {
                    AudioStream audioStream = (AudioStream)resource;

                    // If the resource path of the loaded AudioStream doesn't match the file path, cancel the current operation.
                    // This can happen because CloseStream() was called while the resource was loading.
                    if (streamResPath == null || !streamResPath.Equals(audioStream.ResourcePath))
                    {
                        audioStream.Free();
                        audioStream.Dispose();

                        return;
                    }

                    Stream = audioStream;
                    UpdateLoop();
                }
                else
                {
                    throw new InvalidDataException("The data format of the song file is invalid. Please check that the song's file format is either WAV, OGG, or MP3.");
                }
                */
            }
        }

        /// <summary>
        /// Closes the AudioStream associated with this Song instance
        /// </summary>
        public void CloseStream()
        {
            streamResPath = null;

            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }

        /// <summary>
        /// Constructs an attribution string in the following format:
        /// <br/>
        /// [song artist(s)] - [song name]
        /// </summary>
        /// <returns>The attribution string</returns>
        public string GetAttributionString()
        {
            return $"{Info.Artists} - {Info.Name}";
        }

        public void Dispose()
        {
            CloseStream();
        }
    }
}
