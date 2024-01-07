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
			}

			if (SubtitleLabel != null)
			{
				SubtitleLabel.Text = actualNode.Tr("MENU_SUBTITLE");
			}

			// Add the Exit Game button
			LevelMenuButton exitGameButtonInstance = new LevelMenuButton();
			Button exitGameButton = exitGameButtonInstance.ActualButton;
			exitGameButton.Pressed += () =>
			{
				// Quit the game in the method specified in the Godot documentation:
				// https://docs.godotengine.org/en/stable/tutorials/inputs/handling_quit_requests.html
				tree.Root.PropagateNotification((int)Node.NotificationWMCloseRequest);
				tree.Quit();
			};
			exitGameButton.SelfModulate = Color.Color8(255, 100, 89, 255);
			ItemsControl.AddChild(exitGameButton);
		}
	}
}
