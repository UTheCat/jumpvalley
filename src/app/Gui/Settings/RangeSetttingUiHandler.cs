using Godot;
using System;

namespace UTheCat.Jumpvalley.App.Gui.Settings
{
    /// <summary>
    /// Handles the user-interface for ranged-based settings in the settings menu
    /// </summary>
    public partial class RangeSettingUiHandler : SettingUiHandler, IDisposable
    {
        private RangeSetting setting;
        private Slider slider;
        private LineEdit lineEdit;

        public RangeSettingUiHandler(RangeSetting setting, Node rangeSettingUi) : base(setting, rangeSettingUi)
        {
            this.setting = setting;
            slider = rangeSettingUi.GetNode<Slider>("Slider");
            lineEdit = rangeSettingUi.GetNode<LineEdit>("LineEdit");

            setting.RangeInstance.Share(slider);

            lineEdit.PlaceholderText = $"{slider.MinValue}-{slider.MaxValue}";

            slider.ValueChanged += OnSliderValueChanged;
            lineEdit.TextChanged += OnLineEditValueChanged;
        }

        public new void Dispose()
        {
            slider.ValueChanged -= OnSliderValueChanged;
            lineEdit.TextChanged -= OnLineEditValueChanged;
        }

        private void OnSliderValueChanged(double val)
        {
            lineEdit.Text = val.ToString();
            setting.Value = val;
        }

        private void OnLineEditValueChanged(string sVal)
        {
            double newVal;
            if (double.TryParse(sVal, out newVal))
            {
                if (!slider.AllowLesser) newVal = Math.Max(slider.MinValue, newVal);
                if (!slider.AllowGreater) newVal = Math.Min(slider.MaxValue, newVal);
                slider.SetValueNoSignal(newVal);

                setting.Value = newVal;
            }
        }
    }
}
