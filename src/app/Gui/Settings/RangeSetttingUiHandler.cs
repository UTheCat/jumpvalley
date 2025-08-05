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
        }

        public void Dispose()
        {

        }

        private void OnSliderValueChanged(double val)
        {

        }

        private void OnLineEditValueChanged(string val)
        {
            
        }
    }
}
