using Godot;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Handler for the level menu buttons, the styled button that appears on a Jumpvalley level menu.
    /// </summary>
    public partial class LevelMenuButton
    {
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

        /// <summary>
        /// The button's background color
        /// </summary>
        public Color BackgroundColor
        {
            get => ActualButton.SelfModulate;
            set => ActualButton.SelfModulate = value;
        }

        public string Text
        {
            get => LabelNode.Text;
            set => LabelNode.Text = value;
        }

        public Texture2D Icon
        {
            get => IconNode.Texture;
            set => IconNode.Texture = value;
        }

        public LevelMenuButton(Button actualButton)
        {
            ActualButton = actualButton;
            LabelNode = actualButton.GetNode<Label>("Label");
            IconNode = actualButton.GetNode<TextureRect>("Icon");
        }
    }
}
