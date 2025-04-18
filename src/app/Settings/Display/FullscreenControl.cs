using Godot;

namespace UTheCat.Jumpvalley.App.Settings.Display
{
    public partial class FullscreenControl : SettingBase
    {
        public FullscreenControl()
        {
            Value = false;
            Id = "fullscreen";
            LocalizationId = "SETTINGS_FULLSCREEN";
            ActionMapKey = "fullscreen_toggle";
        }

        public override void Update(object newValue)
        {
            base.Update(newValue);

            if (newValue is bool fullscreenEnabled)
            {
                if (fullscreenEnabled)
                {
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                }
                else
                {
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                }
            }
        }
    }
}
