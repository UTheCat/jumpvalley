using Godot;
using System;
using System.Text.Json.Nodes;

using UTheCat.Jumpvalley.Core.IO;

namespace UTheCat.Jumpvalley.Core.Music
{
    /// <summary>
    /// Represents a folder (such as one on the filesystem) that contains a song and its info metadata.
    /// </summary>
    public class SongPackage
    {
        private static readonly string INFO_FILE_NAME = IO.JsonInfoFile.FILE_NAME;

        /// <summary>
        /// The data of the corresponding "info.json" file
        /// </summary>
        public SongInfo Info;

        /// <summary>
        /// The file path to the song's audio file
        /// </summary>
        public string AudioPath;

        /// <summary>
        /// The directory path of this <see cref="SongPackage"/>
        /// </summary>
        public string DirPath;

        /// <summary>
        /// Creates a SongPackage instance for a given folder path
        /// </summary>
        /// <param name="path">The folder path</param>
        public SongPackage(string path)
        {
            DirPath = path;

            if (!string.IsNullOrEmpty(path))
            {
                // check if the directory can be opened first
                DirAccess dir = DirAccess.Open(path);
                if (dir == null)
                {
                    throw new Exception($"Directory access failed. This is the message returned by DirAccess.GetOpenError(): {DirAccess.GetOpenError()}");
                }

                //Console.WriteLine("Try opening info file");

                // if so, try opening the files inside
                using Godot.FileAccess infoFile = Godot.FileAccess.Open(PathUtil.GodotCombine(path, INFO_FILE_NAME), Godot.FileAccess.ModeFlags.Read);
                if (infoFile == null)
                {
                    throw new Exception($"Failed to open the corresponding {INFO_FILE_NAME} file. This is the message returned by Godot.FileAccess.GetOpenError(): {Godot.FileAccess.GetOpenError()}");
                }

                //Console.WriteLine("Try reading the info file");

                // retrieve the info from the info file
                string infoText = infoFile.GetAsText();
                Info = new SongInfo(JsonNode.Parse(infoText));

                AudioPath = Info.AudioPath;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Song"/> instance using the file paths
        /// specified in <see cref="DirPath"/> and <see cref="AudioPath"/>.
        /// The created <see cref="Song"/> instance will have its info
        /// set to this <see cref="SongPackage"/>'s current song info.  
        /// </summary>
        /// <returns></returns>
        public Song CreateSongInstance()
        {
            return new Song(PathUtil.GodotCombine(DirPath, AudioPath), Info);
        }
    }
}
