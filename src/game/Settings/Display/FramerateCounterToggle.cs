using JumpvalleyGame.Gui;

namespace JumpvalleyGame.Settings.Display
{
    public partial class FramerateCounterToggle : SettingBase
    {
        public FramerateCounter Counter;

        public FramerateCounterToggle()
        {
            Value = false;
            Id = "show_framerate_counter";
            LocalizationId = "SETTINGS_SHOW_FRAMERATE_COUNTER";
            ActionMapKey = "toggle_framerate_counter_visibility";
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
