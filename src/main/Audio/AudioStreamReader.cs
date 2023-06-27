using Godot;
using Jumpvalley.Music;
using System;
using System.IO;

namespace Jumpvalley.Audio
{
    /// <summary>
    /// Loads a file that's either imported as a resource (at one point or another) or somewhere on the filesystem.
    /// </summary>
    public partial class AudioStreamReader
    {
        public readonly string RESOURCE_PROTOCOL = "res://";

        public readonly string FILE_NOT_FOUND_ERR = "No file could be found at the given file path. Please double check that the file path is correct.";
        public readonly string INVALID_DATA_ERR = "The data format of the file is invalid. Please check that the audio's file format is either WAV, OGG, or MP3.";

        /// <summary>
        /// The result from reading the stream.
        /// </summary>
        public AudioStream Stream;

        /// <summary>
        /// The file path of the file that the data comes from.
        /// </summary>
        public string FilePath;

        /// <summary>
        /// Creates a new AudioStreamReader for the specified file path
        /// </summary>
        /// <param name="filePath">The file path to the audio file</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file path is invalid, or when no file could be found for the given file path.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Thrown when the file was found for the given file path, but the file format is invalid.
        /// </exception>
        public AudioStreamReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new FileNotFoundException("The file path given is null or empty. Please specify a file path.");
            }

            string fileExtension = Path.GetExtension(filePath);
            if (!(fileExtension.Equals(".wav") || fileExtension.Equals(".ogg") || fileExtension.Equals(".mp3")))
            {
                throw new InvalidDataException(INVALID_DATA_ERR);
            }

            FilePath = filePath;

            string protocol = filePath.Split("://")[0];
            if (protocol.Equals(RESOURCE_PROTOCOL))
            {
                // read from resource filesystem
                Resource res = GD.Load(filePath);

                if (res == null)
                {
                    throw new FileNotFoundException(FILE_NOT_FOUND_ERR);
                }

                if (res is AudioStreamWav || res is AudioStreamOggVorbis || res is AudioStreamMP3)
                {
                    Stream = (AudioStream)res;
                }
                else
                {
                    // In case we still get to this point after the file extension check
                    throw new InvalidDataException(INVALID_DATA_ERR);
                }
            }
            else
            {
                // read from external filesystem
                // (external includes the user's current device)
                using Godot.FileAccess file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    Error error = Godot.FileAccess.GetOpenError();

                    if (error == Error.FileNotFound)
                    {
                        throw new FileNotFoundException(FILE_NOT_FOUND_ERR);
                    }
                    else
                    {
                        throw new Exception($"Failed to open the corresponding {InfoFile.FILE_NAME} file. This is the message returned by FileAccess.GetOpenError(): {Godot.FileAccess.GetOpenError()}");
                    }
                }

                AudioStream stream = null;
                if (fileExtension.Equals(".wav"))
                {
                    AudioStreamWav sWav = new AudioStreamWav();

                    // This currently assumes that the format of a .wav aligns with the default
                    // properties of AudioStreamWav.
                    // We'll need to read the header of the .wav file in order to get the format of the .wav file,
                    // which is something that will get implemented at a later time.
                    sWav.Data = file.GetBuffer((long) file.GetLength());

                    stream = sWav;
                }
                else if (fileExtension.Equals(".ogg"))
                {
                    throw new InvalidDataException("Loading OGG formatted audio currently isn't supported. Sorry :(");

                    //AudioStreamOggVorbis sOgg = new AudioStreamOggVorbis();
                    //sOgg.Data = file.GetBuffer((long)file.GetLength());
                }
                else if (fileExtension.Equals(".mp3"))
                {
                    AudioStreamMP3 sMp3 = new AudioStreamMP3();
                    sMp3.Data = file.GetBuffer((long)file.GetLength());
                    stream = sMp3;
                }

                if (stream != null)
                {
                    stream.ResourcePath = filePath;
                    Stream = stream;
                }
            }
        }
    }
}
