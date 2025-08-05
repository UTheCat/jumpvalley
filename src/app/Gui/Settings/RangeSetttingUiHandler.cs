using Godot;
using System;

namespace UTheCat.Jumpvalley.App.Gui.Settings
{
    /// <summary>
    /// Handles the user-interface for ranged-based settings in the settings menu
    /// </summary>
    public partial class RangeSettingUiHandler : IDisposable
    {
        private RangeSetting setting;
        private Slider slider;
        private LineEdit lineEdit;

        public RangeSettingUiHandler(RangeSetting setting, Node rangeSettingUi)
        {
            this.setting = setting;
            slider = rangeSettingUi.GetNode<Slider>("Slider");
            lineEdit = rangeSettingUi.GetNode<LineEdit>("LineEdit");

            lineEdit.PlaceholderText = $"{slider.MinValue}-{slider.MaxValue}";

            slider.ValueChanged += OnSliderValueChanged;
            lineEdit.TextChanged += OnLineEditValueChanged;
        }

        public void Dispose()
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
                slider.Value = newVal;
            }
        }
    }
}
