using System;
using UTheCat.Jumpvalley.App.Display;

namespace UTheCat.Jumpvalley.App.Settings.Display
{
    public partial class HiDpiAdapterToggle : SettingBase
    {
        public HiDpiAdapter Adapter { get; private set; }

        public HiDpiAdapterToggle(HiDpiAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter), $"Attempted to pass a {nameof(HiDpiAdapter)} that doesn't exist.");

            Adapter = adapter;

            Value = true;
            Id = "hidpi_adapter";
            LocalizationId = "SETTINGS_HIDPI_ADAPTER";
            ActionMapKey = null;
        }

        public override void Update(object newValue)
        {
            base.Update(newValue);

            if (newValue is bool enabled)
            {
                Adapter.Enabled = enabled;
            }
        }
    }
}
