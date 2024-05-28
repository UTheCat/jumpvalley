using System;
using Godot;
using Jumpvalley.Animation;
using Jumpvalley.Tweening;

using JumpvalleyGame.Settings;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Handler for Jumpvalley's settings menu
    /// </summary>
    public partial class SettingsMenu : AnimatedNode, IDisposable
    {
        private Control menu;
        private SettingGroup settings;
        private Control settingListNode;
        private PackedScene categoryScene;
        private PackedScene checkButtonSettingScene;

        private SceneTreeTween positionTween;

        public override bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                base.IsVisible = value;

                if (value)
                {
                    positionTween.Speed = 1f;
                }
                else
                {
                    positionTween.Speed = -1f;
                }

                positionTween.Resume();
            }
        }

        public SettingsMenu(Control actualNode, SceneTree tree, SettingGroup settings) : base(actualNode)
        {
            menu = actualNode;
            this.settings = settings;
            settingListNode = actualNode.GetNode<Control>("ScrollContainer/VBoxContainer/SettingList");

            categoryScene = LoadSettingNodeScene("setting_category_scene");
            checkButtonSettingScene = LoadSettingNodeScene("check_button_setting_scene");

            menu.Visible = false;
            SetMenuPosition(1f);

            positionTween = new SceneTreeTween(0.25, Tween.TransitionType.Quad, Tween.EaseType.Out, tree);
            positionTween.InitialValue = 1;
            positionTween.FinalValue = 0;
            positionTween.OnStep += (object o, float frac) =>
            {
                float pos = (float)positionTween.GetCurrentValue();
                menu.Visible = pos < 1f;
                SetMenuPosition(pos);
            };
        }

        private void SetMenuPosition(float anchorTop)
        {
            menu.AnchorTop = anchorTop;
            menu.AnchorBottom = anchorTop + 1f;
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
