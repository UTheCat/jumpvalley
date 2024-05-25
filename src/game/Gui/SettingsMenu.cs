using System;
using Godot;
using Jumpvalley.Animation;

using JumpvalleyGame.Settings;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Handler for Jumpvalley's settings menu
    /// </summary>
    public partial class SettingsMenu : AnimatedNode, IDisposable
    {
        private SettingGroup settings;
        private Control settingListNode;
        private PackedScene categoryScene;
        private PackedScene checkButtonSettingScene;

        public SettingsMenu(Node actualNode, SettingGroup settings) : base(actualNode)
        {
            this.settings = settings;
            settingListNode = actualNode.GetNode<Control>("ScrollContainer/VBoxContainer/SettingList");

            categoryScene = LoadSettingNodeScene("setting_category_scene");
            checkButtonSettingScene = LoadSettingNodeScene("check_button_setting_scene");
        }

        private PackedScene LoadSettingNodeScene(string metadataName)
        {
            return ResourceLoader.Load<PackedScene>(settingListNode.GetMeta(metadataName).As<string>());
        }

        private void Populate(SettingGroup settingGroup)
        {
            Node actualNode = ActualNode;

            // Add category title
            Label categoryLabel = categoryScene.Instantiate<Label>();
            categoryLabel.Text = actualNode.Tr(settingGroup.LocalizationId);
            settingListNode.AddChild(categoryLabel);

            foreach (SettingBase setting in settingGroup.SettingList)
            {
                if (setting.Value is bool)
                {
                    Control checkButtonSettingNode = checkButtonSettingScene.Instantiate<Control>();
                    checkButtonSettingNode.GetNode<Label>("Title").Text = actualNode.Tr(setting.LocalizationId);
                    settingListNode.AddChild(checkButtonSettingNode);
                }
            }

            foreach (SettingGroup subgroup in settingGroup.Subgroups)
            {
                Populate(subgroup);
            }
        }

        /// <summary>
        /// Generates the actual GUI nodes that will display the game's settings
        /// </summary>
        public void Populate()
        {
            Populate(settings);
        }

        public void Dispose()
        {
            categoryScene.Dispose();
            checkButtonSettingScene.Dispose();
        }
    }
}
