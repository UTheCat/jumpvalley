namespace JumpvalleyGame.Settings.Display
{
    /// <summary>
    /// Display settings for Jumpvalley
    /// </summary>
    public partial class DisplaySettings : SettingGroup
    {
        public DisplaySettings()
        {
            Id = "display";
            LocalizationId = "SETTINGS_GROUP_DISPLAY";

            SettingBase[] settings = [
                new FullscreenControl()
            ];

            foreach (SettingBase s in settings)
            {
                Add(s);
            }
        }
    }
}
