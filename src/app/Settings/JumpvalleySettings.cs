using System;
using UTheCat.Jumpvalley.App.Settings.Audio;
using UTheCat.Jumpvalley.App.Settings.Display;
using UTheCat.Jumpvalley.App.Settings.Gameplay;

namespace UTheCat.Jumpvalley.App.Settings
{
    /// <summary>
    /// Main list of user settings for Jumpvalley
    /// </summary>
    public partial class JumpvalleySettings : IDisposable
    {
        public SettingGroup Group { get; private set; }
        public SettingsFile File { get; private set; }

        public JumpvalleySettings()
        {
            SettingGroup group = new SettingGroup();

            group.AddSettingGroup(new DisplaySettings());
            group.AddSettingGroup(new GameplaySettings());
            group.AddSettingGroup(new AudioSettings());
            group.ShouldDisplayTitle = false;

            Group = group;

            File = new SettingsFile();
        }

        public void Dispose()
        {
            Group.Dispose();
        }
    }
}
