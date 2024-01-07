using Godot;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// The primary level menu, the menu that's accessed by pressing the three-dots at the bottom of the user's screen.
    /// This menu contains options like accessing game settings and exiting the game.
    /// </summary>
    public partial class PrimaryLevelMenu: LevelMenu
    {
        public PrimaryLevelMenu(Control actualNode, SceneTree tree) : base(actualNode, tree)
        {
            if (TitleLabel != null)
            {
                TitleLabel.Text = actualNode.Tr("MENU_TITLE");
                SubtitleLabel.Text = actualNode.Tr("MENU_SUBTITLE");
            }
        }
    }
}
