using JumpvalleyApp.Settings.Display;

namespace JumpvalleyApp.Settings
{
    /// <summary>
    /// Main list of user settings for Jumpvalley
    /// </summary>
    public partial class JumpvalleySettings
    {
        public SettingGroup Group;
        public SettingsFile File;

        public JumpvalleySettings()
        {
            SettingGroup group = new SettingGroup();

            group.AddSettingGroup(new DisplaySettings());
            group.ShouldDisplayTitle = false;

            Group = group;

            File = new SettingsFile();
        }
    }
}
