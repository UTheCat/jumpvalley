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

        private bool isDisposed;

        /// <summary>
        /// Function/method that disconnects user interaction with
        /// the setting's toggle control node from the actual modification
        /// of the setting's value.
        /// </summary>
        private Action togglePressedDisconnectFunction;

        /// <summary>
        /// Function/method that updates the toggle node that the user would
        /// interact with. This is useful for updating the toggle node
        /// when the setting's value has been modified in a way that
        /// doesn't use the toggle node.
        /// </summary>
        private Action toggleNodeUpdateFunction;

        public SettingUiHandler(SettingBase setting, Node gui)
        {
            if (setting == null) throw new ArgumentNullException(nameof(setting));
            if (gui == null) throw new ArgumentNullException(nameof(gui));

            Setting = setting;
            Gui = gui;

            object settingValue = setting.Value;
            if (settingValue is bool)
            {
                Button toggleButton = gui.GetNode<CheckButton>("ToggleNode");
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
                    toggleNodeUpdateFunction = () =>
                    {
                        if (setting.Value is bool vBool)
                        {
                            toggleButton.ToggleMode = vBool;
                        }
                    };
                }
            }

            ActionMapKey = null;
            DefaultInputProcessingEnabled = true;
        }

        public override void _Input(InputEvent @event)
        {
            if (isDisposed == true || DefaultInputProcessingEnabled == false) return;

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

                    if (toggleNodeUpdateFunction != null)
                    {
                        toggleNodeUpdateFunction();
                    }
                }
            }

            base._Input(@event);
        }

        public new void Dispose()
        {
            isDisposed = true;
            togglePressedDisconnectFunction();

            QueueFree();
            base.Dispose();
        }
    }
}
