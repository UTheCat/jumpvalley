using Godot;

using JumpvalleyGame.Settings;

namespace JumpvalleyGame.Gui.Settings
{
    /// <summary>
    /// Class that allows user input to change a setting
    /// </summary>
    public partial class SettingUiHandler : Node
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

        public SettingUiHandler(SettingBase setting, Node gui)
        {
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
    }
}
