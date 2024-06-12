using System;
using System.Text.Json.Nodes;
using Godot;
using Jumpvalley.Logging;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class responsible for loading and saving to <c>settings.json</c>,
    /// the file on the user's filesystem where the user's settings configuration
    /// is saved (as a JSON file).
    /// </summary>
    public partial class SettingsFile
    {
        public static readonly string FILE_NAME = "settings.json";

        private ConsoleLogger logger;

        public string SettingsFileLocation;

        public JsonNode Data;

        public SettingsFile()
        {
            logger = new ConsoleLogger(nameof(SettingsFile));

            SettingsFileLocation = $"user://{FILE_NAME}";
        }

        public void Read()
        {
            logger.Print("Now attempting to read settings file");

            string fileLocation = SettingsFileLocation;

            if (FileAccess.FileExists(fileLocation))
            {
                FileAccess file = FileAccess.Open(fileLocation, FileAccess.ModeFlags.Read);

                if (file == null) throw new Exception(FileAccess.GetOpenError().ToString());

                string sData = file.GetAsText();

                file.Close();
                file.Dispose();

                Data = JsonNode.Parse(sData);
            }
            else
            {
                logger.Print($"{fileLocation} doesn't exist. A new settings file at this location should be created when Write() is called");
            }
        }
    }
}
