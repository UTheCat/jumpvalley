using System;
using Godot;

using JumpvalleyGame.Settings;

namespace JumpvalleyGame.Gui.Settings
{
    /// <summary>
    /// Class that allows user input to change a setting
    /// </summary>
    public partial class SettingUiHandler : Node, IDisposable
    {
        public SettingBase Setting;
        public Node Gui;

        private string _actionMapKey;
        public string ActionMapKey
        {
            get => _actionMapKey;
            set
            {
                SetProcessInput(value != null);
                _actionMapKey = value;
            }
        }

        public bool DefaultInputProcessingEnabled;

        /// <summary>
        /// Function/method that disconnects user interaction with
        /// the setting's toggle control node from the actual modification
        /// of the setting's value.
        /// </summary>
        private Action togglePressedDisconnectFunction;

        public SettingUiHandler(SettingBase setting, Node gui)
        {
            if (setting == null) throw new ArgumentNullException(nameof(setting));
            if (gui == null) throw new ArgumentNullException(nameof(gui));

            Setting = setting;
            Gui = gui;

            object settingValue = setting.Value;
            if (settingValue is bool)
            {
                Button toggleButton = gui.GetNode<Button>("ToggleNode");
                if (toggleButton != null)
                {
                    void HandleToggle(bool newValue)
                    {
                        setting.Value = newValue;
                    }

                    toggleButton.Toggled += HandleToggle;
                    togglePressedDisconnectFunction = () =>
                    {
                        toggleButton.Toggled -= HandleToggle;
                    };
                }
            }

            ActionMapKey = null;
            DefaultInputProcessingEnabled = true;
        }

        public override void _Input(InputEvent @event)
        {
            if (!DefaultInputProcessingEnabled) return;

            string actionMapKey = ActionMapKey;
            if (actionMapKey != null && Input.IsActionJustPressed(actionMapKey))
            {
                SettingBase setting = Setting;
                if (setting != null)
                {
                    object value = setting.Value;
                    if (value is bool vBool)
                    {
                        setting.Value = !vBool;
                    }
                }
            }

            base._Input(@event);
        }

        public new void Dispose()
        {
            togglePressedDisconnectFunction();

            QueueFree();
            base.Dispose();
        }
    }
}
