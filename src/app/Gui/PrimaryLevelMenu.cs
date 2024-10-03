using Godot;

namespace JumpvalleyApp.Gui
{
	/// <summary>
	/// The primary level menu, the menu that's accessed by pressing the three-dots at the bottom of the user's screen.
	/// This menu contains options like accessing game settings and exiting the game.
	/// </summary>
	public partial class PrimaryLevelMenu : LevelMenu
	{
		private readonly float BUTTON_Y_POS_DIFF = 52f;

		public SettingsMenu CurrentSettingsMenu;

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
			Button settingsButtonNode = menuButtonResource.Instantiate<Button>();
			LevelMenuButton settingsButton = new LevelMenuButton(settingsButtonNode)
			{
				Text = actualNode.Tr("SETTINGS_MENU_TITLE"),
				Icon = GD.Load<CompressedTexture2D>("res://addons/icons/settings_48dp.svg"),
				BackgroundColor = Color.FromHsv(48f / 360f, 0.65f, 1f)
			};
			settingsButtonNode.Pressed += () =>
			{
				if (IsVisible == false) return;
				IsVisible = false;
				CurrentSettingsMenu.IsVisible = true;
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
			float buttonYSize = exitGameButton.Size.Y;

			for (int i = 0; i < buttonList.Length; i++)
			{
				Button b = buttonList[i];
				b.OffsetTop = BUTTON_Y_POS_DIFF * i;
				b.OffsetBottom = b.OffsetTop + buttonYSize;
				ItemsControl.AddChild(b);
			}
		}
	}
}
