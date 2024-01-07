using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// Handler for the level menu buttons, the styled button that appears on a Jumpvalley level menu.
    /// </summary>
    public partial class LevelMenuButton
    {
        /// <summary>
        /// The path to the scene file that defines the default layout and style of the level menu button
        /// </summary>
        public static readonly string DEFAULT_LEVEL_MENU_BUTTON_SCENE = "res://gui/level_menu_button.tscn";

        private static PackedScene buttonScene;

        /// <summary>
        /// The actual Godot button instance that serves as the button's root node.
        /// </summary>
        public Button ActualButton { get; private set; }

        /// <summary>
        /// The text label displaying the button's text
        /// </summary>
        public Label LabelNode { get; private set; }

        /// <summary>
        /// The texture rectangle displaying the button's icon
        /// </summary>
        public TextureRect IconNode { get; private set; }

        private void UpdateNodeVariables()
        {
            LabelNode = ActualButton.GetNode<Label>("Label");
            IconNode = ActualButton.GetNode<TextureRect>("Icon");
        }

        public LevelMenuButton(Button actualButton)
        {
            ActualButton = actualButton;
            UpdateNodeVariables();
        }

        public LevelMenuButton()
        {
            LoadDefaultLevelMenuButtonScene();

            if (buttonScene != null)
            {
                Node node = buttonScene.Instantiate();
                if (node is Button)
                {
                    ActualButton = (Button)node;
                    UpdateNodeVariables();
                }
            }
        }

        /// <summary>
        /// Preloads the default menu button scene from the resource filesystem.
        /// Loading is done only if it has not occurred yet.
        /// </summary>
        public static void LoadDefaultLevelMenuButtonScene()
        {
            if (buttonScene != null) return;

            Resource res = ResourceLoader.Load(DEFAULT_LEVEL_MENU_BUTTON_SCENE);
            if (res is PackedScene && buttonScene == null)
            {
                buttonScene = (PackedScene)res;
            }
        }
    }
}
