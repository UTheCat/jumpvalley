using System;
using System.Collections.Generic;
using Godot;
using UTheCat.Jumpvalley.Core.Animation;
using UTheCat.Jumpvalley.Core.Tweening;

using JumpvalleyApp.Gui.Settings;
using JumpvalleyApp.Settings;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Handler for Jumpvalley's settings menu
    /// </summary>
    public partial class SettingsMenu : AnimatedNode, IDisposable
    {
        private Control menu;
        private SettingGroup settings;
        private Control settingListNode;
        //private Control background;
        private Control scrollContainer;
        private Button closeButton;

        private PackedScene categoryScene;
        private PackedScene checkButtonSettingScene;

        private SceneTreeTween positionTween;
        //private SceneTreeTween closeButtonTween;

        private List<SettingUiHandler> settingUiHandlers;

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
            //background = actualNode.GetNode<Control>("Background");
            scrollContainer = actualNode.GetNode<Control>("ScrollContainer");
            settingListNode = scrollContainer.GetNode<Control>("VBoxContainer/SettingList");
            closeButton = actualNode.GetNode<Button>("CloseButton");

            categoryScene = LoadSettingNodeScene("setting_category_scene");
            checkButtonSettingScene = LoadSettingNodeScene("check_button_setting_scene");

            settingUiHandlers = new List<SettingUiHandler>();

            menu.Visible = false;
            SetMenuPosition(-1f);

            positionTween = new SceneTreeTween(0.25, Tween.TransitionType.Quad, Tween.EaseType.Out, tree);
            positionTween.InitialValue = -1;
            positionTween.FinalValue = 0;
            positionTween.OnStep += (object o, float frac) =>
            {
                float pos = (float)positionTween.GetCurrentValue();
                menu.Visible = pos < 1f;
                SetMenuPosition(pos);
            };

            closeButton.Pressed += HandleCloseButtonPressed;
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
            float nodeMinSizeX = scrollContainer.Size.X;

            // Add category title
            if (settingGroup.ShouldDisplayTitle)
            {
                Label categoryLabel = categoryScene.Instantiate<Label>();
                categoryLabel.Text = actualNode.Tr(settingGroup.LocalizationId);

                Vector2 minSize = categoryLabel.CustomMinimumSize;
                minSize.X = nodeMinSizeX;
                categoryLabel.CustomMinimumSize = minSize;

                settingListNode.AddChild(categoryLabel);
            }

            foreach (SettingBase setting in settingGroup.SettingList)
            {
                Control settingNode = null;

                if (setting.Value is bool)
                {
                    settingNode = checkButtonSettingScene.Instantiate<Control>();
                    settingNode.GetNode<Label>("Title").Text = actualNode.Tr(setting.LocalizationId);
                }

                if (settingNode != null)
                {
                    Vector2 minSize = settingNode.CustomMinimumSize;
                    minSize.X = nodeMinSizeX;
                    settingNode.CustomMinimumSize = minSize;

                    SettingUiHandler handler = new SettingUiHandler(setting, settingNode)
                    {
                        ActionMapKey = setting.ActionMapKey
                    };
                    settingUiHandlers.Add(handler);

                    settingNode.AddChild(handler);
                    settingListNode.AddChild(settingNode);
                }
            }

            foreach (SettingGroup subgroup in settingGroup.Subgroups)
            {
                Populate(subgroup);
            }
        }

        private void HandleCloseButtonPressed()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Generates the actual GUI nodes that will display the app's settings
        /// </summary>
        public void Populate()
        {
            Populate(settings);
        }

        public void Dispose()
        {
            closeButton.Pressed -= HandleCloseButtonPressed;

            positionTween.Dispose();
            categoryScene.Dispose();
            checkButtonSettingScene.Dispose();

            foreach (SettingUiHandler handler in settingUiHandlers)
            {
                handler.Dispose();
            }
            settingUiHandlers.Clear();
        }
    }
}
