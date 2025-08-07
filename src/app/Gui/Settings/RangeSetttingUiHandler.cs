using Godot;
using System;

namespace UTheCat.Jumpvalley.App.Gui.Settings
{
    /// <summary>
    /// Handles the user-interface for ranged-based settings in the settings menu
    /// </summary>
    public partial class RangeSettingUiHandler : SettingUiHandler, IDisposable
    {
        private static readonly Color LINE_EDIT_ERR_COLOR = Color.Color8(255, 124, 115);
        private RangeSetting setting;
        private Slider slider;
        private LineEdit lineEdit;
        private StyleBoxFlat lineEditFocusStyle;
        private Color lineEditFocusStyleOriginalBorderColor;
        private bool isCurrentTextFieldValueValid = false;

        public RangeSettingUiHandler(RangeSetting setting, Node rangeSettingUi) : base(setting, rangeSettingUi)
        {
            this.setting = setting;
            slider = rangeSettingUi.GetNode<Slider>("Slider");
            lineEdit = rangeSettingUi.GetNode<LineEdit>("LineEdit");
            lineEditFocusStyle = lineEdit.GetThemeStylebox("focus") as StyleBoxFlat;
            if (lineEditFocusStyle == null) throw new Exception("StyleBox of RangeSetting LineEdit node has to be a StyleBoxFlat");
            lineEditFocusStyleOriginalBorderColor = lineEditFocusStyle.BorderColor;

            setting.RangeInstance.Share(slider);

            lineEdit.Text = slider.Value.ToString();
            lineEdit.PlaceholderText = $"{slider.MinValue}-{slider.MaxValue}";

            slider.ValueChanged += OnSliderValueChanged;
            lineEdit.FocusExited += OnLineEditFocusLost;
            lineEdit.TextChanged += OnLineEditValueChanged;
        }

        public new void Dispose()
        {
            slider.ValueChanged -= OnSliderValueChanged;
            lineEdit.TextChanged -= OnLineEditValueChanged;
            lineEdit.FocusExited -= OnLineEditFocusLost;
        }

        private void OnSliderValueChanged(double val)
        {
            lineEdit.Text = val.ToString();
            setting.Value = val;
        }

        private void OnLineEditValueChanged(string sVal)
        {
            double newVal;

            // Because of floating-point precision errors, this conditional will have problems when range restriction is requested
            // and min or max have lots of decimals.
            // 
            // But when min and max are integers (as is the case for now), this should generally be okay.
            if (
                double.TryParse(sVal, out newVal)
                && (slider.AllowLesser || !(newVal < slider.MinValue))
                && (slider.AllowGreater || !(newVal > slider.MaxValue))
            )
            {
                isCurrentTextFieldValueValid = true;
                lineEditFocusStyle.BorderColor = lineEditFocusStyleOriginalBorderColor;

                slider.SetValueNoSignal(newVal);

                setting.Value = newVal;
            }
            else
            {
                isCurrentTextFieldValueValid = false;

                // Let the user know that input is invalid
                lineEditFocusStyle.BorderColor = LINE_EDIT_ERR_COLOR;
            }
        }

        private void OnLineEditFocusLost()
        {
            if (!isCurrentTextFieldValueValid)
            {
                lineEdit.Text = slider.Value.ToString();
                lineEditFocusStyle.BorderColor = lineEditFocusStyleOriginalBorderColor;
            }
        }
    }
}
