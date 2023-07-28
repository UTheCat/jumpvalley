using Godot;
using System;
//using System.IO;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a folder (such as one on the filesystem) that contains a song and its info metadata.
    /// </summary>
    public class SongPackage
    {
        private static readonly string INFO_FILE_NAME = IO.InfoFile.FILE_NAME;

        /// <summary>
        /// Data as raw text that comes from "info.txt" within the folder
        /// </summary>
        public string InfoData = "";

        /// <summary>
        /// The data of the corresponding "info.txt" file
        /// </summary>
        public SongInfoFile InfoFile = null;

        /// <summary>
        /// The name of the song's audio file
        /// </summary>
        public string SongFileName;

        /// <summary>
        /// The directory path of this <see cref="SongPackage"/> with trailing slashes and trailing backslashes removed
        /// </summary>
        public string Path = "";

        /// <summary>
        /// Creates a SongPackage instance for a given folder path
        /// </summary>
        /// <param name="path">The folder path</param>
        public SongPackage(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Some like to end the paths with an extra "/" or "\". Account for this.
                if (path.EndsWith("/") || path.EndsWith("\\"))
                {
                    path = path.Substring(0, path.Length - 1);
                }

                Path = path;

                //Console.WriteLine("Try opening directory: " + path);

                // check if the directory can be opened first
                DirAccess dir = DirAccess.Open(path);
                if (dir == null)
                {
                    throw new Exception($"Directory access failed. This is the message returned by DirAccess.GetOpenError(): {DirAccess.GetOpenError()}");
                }

                path += "/";

                //Console.WriteLine("Try opening info file");

                // if so, try opening the files inside
                using Godot.FileAccess infoFile = Godot.FileAccess.Open(path + INFO_FILE_NAME, Godot.FileAccess.ModeFlags.Read);
                if (infoFile == null)
                {
                    throw new Exception($"Failed to open the corresponding {INFO_FILE_NAME} file. This is the message returned by FileAccess.GetOpenError(): {Godot.FileAccess.GetOpenError()}");
                }

                //Console.WriteLine("Try reading the info file");

                // retrieve the info from the info file
                string infoText = infoFile.GetAsText();
                InfoData = infoText;
                InfoFile = new SongInfoFile(infoText);

                SongFileName = InfoFile.FileName;

                //Console.WriteLine("Constructor finished, song file name is: " + SongFileName);

                // This bit of code is commented out since it won't work with Godot-based file system access,
                // particularly with "res://" and "user://"
                /*
                // Add the removed slash back for convenience in the next bit of code
                path += "/";

                // Try to read the contents of the info file in the package.
                StreamReader infoStreamReader = null;
                try
                {
                    using (infoStreamReader = new StreamReader(path + "info.txt"))
                    {
                        InfoData = infoStreamReader.ReadToEnd();
                        SongInfoFile = new SongInfoFile(InfoData);
                    }
                }
                catch (System.Exception e)
                {
                    // Info files can be excluded from a song package although it's recommended that the user put one anyway.
                    if (e is FileNotFoundException)
                    {
                        if (infoStreamReader != null)
                        {
                            infoStreamReader.Dispose();
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                */
            }
        }
    }
}
