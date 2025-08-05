using UTheCat.Jumpvalley.Core.Players.Camera;

using UTheCat.Jumpvalley.App.Settings.Gameplay;

namespace UTheCat.Jumpvalley.App.Settings.Display
{
    /// <summary>
    /// Display settings for Jumpvalley
    /// </summary>
    public partial class GameplaySettings : SettingGroup
    {
        public BaseCamera PlayerCamera = null;

        public GameplaySettings()
        {
            Id = "gameplay";
            LocalizationId = "SETTINGS_GROUP_GAMEPLAY";
        }

        public void Initialize()
        {
            SettingBase[] settings = [
                new CameraSensitivitySetting(PlayerCamera)
            ];

            foreach (SettingBase s in settings)
            {
                Add(s);
            }
        }
    }
}
