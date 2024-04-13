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

			PackedScene menuButtonResource = ResourceLoader.Load<PackedScene>("res://gui/level_menu_button.tscn");

            // Add the Exit Game button
            Button exitGameButton = menuButtonResource.Instantiate<Button>();
            LevelMenuButton exitGameButtonHandler = new LevelMenuButton(exitGameButton);
			exitGameButton.Pressed += () =>
			{
				// Quit the game in the method specified in the Godot documentation:
				// https://docs.godotengine.org/en/stable/tutorials/inputs/handling_quit_requests.html
				tree.Root.PropagateNotification((int)Node.NotificationWMCloseRequest);
				tree.Quit();
			};
			exitGameButton.SelfModulate = Color.Color8(255, 100, 89, 255);
			exitGameButtonHandler.LabelNode.Text = exitGameButton.Tr("EXIT_GAME");
			exitGameButtonHandler.IconNode.Texture = GD.Load<CompressedTexture2D>("res://addons/icons/logout_white_48dp.svg");

			menuButtonResource.Dispose();

			ItemsControl.AddChild(exitGameButton);
		}
	}
}
