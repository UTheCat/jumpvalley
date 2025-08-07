using UTheCat.Jumpvalley.Core.Players.Camera;

namespace UTheCat.Jumpvalley.App.Settings.Gameplay
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
