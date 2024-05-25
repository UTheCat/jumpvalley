using Godot;
using Jumpvalley.Animation;

using JumpvalleyGame.Settings;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Handler for Jumpvalley's settings menu
    /// </summary>
    public partial class SettingsMenu : AnimatedNode
    {
        private SettingGroup settings;

        private Control settingListNode;

        public SettingsMenu(Node actualNode, SettingGroup settings) : base(actualNode)
        {
            this.settings = settings;
            settingListNode = actualNode.GetNode<Control>("ScrollContainer/VBoxContainer/SettingList");
        }

        /// <summary>
        /// Generates the actual GUI nodes that will display the game's settings
        /// </summary>
        public void Populate()
        {
            
        }
    }
}
