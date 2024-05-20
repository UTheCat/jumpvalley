using Godot;

namespace JumpvalleyGame.Gui
{
	/// <summary>
	/// The primary level menu, the menu that's accessed by pressing the three-dots at the bottom of the user's screen.
	/// This menu contains options like accessing game settings and exiting the game.
	/// </summary>
	public partial class PrimaryLevelMenu : LevelMenu
	{
		private readonly int BUTTON_Y_POS_DIFF = 52;

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

			// Add the Settings button
			LevelMenuButton settingsButton = new LevelMenuButton(menuButtonResource.Instantiate<Button>())
			{
				Text = actualNode.Tr("SETTINGS_MENU_TITLE"),
				Icon = GD.Load<CompressedTexture2D>("res://addons/icons/settings_48dp.svg"),
				BackgroundColor = Color.FromHsv(48f / 360f, 0.65f, 1f)
			};

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
			exitGameButtonHandler.BackgroundColor = Color.Color8(255, 100, 89, 255);
			exitGameButtonHandler.Text = exitGameButton.Tr("EXIT_GAME");
			exitGameButtonHandler.Icon = GD.Load<CompressedTexture2D>("res://addons/icons/logout_white_48dp.svg");

			menuButtonResource.Dispose();

			Button[] buttonList = {
				settingsButton.ActualButton,
				exitGameButton
			};

			for (int i = 0; i < buttonList.Length; i++)
			{
				Button b = buttonList[i];
				b.Position = new Vector2(0, BUTTON_Y_POS_DIFF * i);
				ItemsControl.AddChild(b);
			}
		}
	}
}
