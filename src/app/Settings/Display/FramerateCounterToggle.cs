using JumpvalleyApp.Gui;

namespace JumpvalleyApp.Settings.Display
{
    public partial class FramerateCounterToggle : SettingBase
    {
        public FramerateCounter Counter;

        public FramerateCounterToggle()
        {
            Value = false;
            Id = "show_framerate";
            LocalizationId = "SETTINGS_SHOW_FRAMERATE";
            ActionMapKey = "toggle_framerate_visibility";
            Counter = null;    
        }

        public override void Update(object newValue)
        {
            base.Update(newValue);

            if (newValue is bool isVisible)
            {
                FramerateCounter counter = Counter;
                if (counter != null)
                {
                    counter.IsVisible = isVisible;
                }
            }
        }
    }
}
