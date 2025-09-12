using Godot;
using System;
using System.Collections.Generic;

namespace UTheCat.Jumpvalley.App.Gui
{
	/// <summary>
	/// The primary level menu, the menu that's accessed by pressing the three-dots at the bottom of the user's screen.
	/// This menu contains options like accessing app settings and exiting the app.
	/// </summary>
	public partial class PrimaryLevelMenu : LevelMenu, IDisposable
	{
		private readonly float BUTTON_Y_POS_DIFF = 52f;
		private readonly string KEYBIND_OPEN_MENU_META_NAME = "keybind_open_menu";

		private List<IDisposable> disposables;

		/// <summary>
		/// Keyboard shortcut for opening the PrimaryLevelMenu specified in the metadata of the PrimaryLevelMenu's root node
		/// </summary>
		private InputEventKey keybindOpenMenu;

		public SettingsMenu CurrentSettingsMenu;

		/// <summary>
		/// If not null, keybind_open_menu (e.g. `Esc` key) will open the menu only if
		/// the value of this <see cref="BgPanelAnimatedNodeGroup"/>'s ShouldBeVisible property
		/// is set to false.
		/// <br/><br/>
		/// This is intended to prevent keyboard shortcut conflicts. 
		/// </summary>
		public BgPanelAnimatedNodeGroup BgPanelNodeGroup = null;

		public PrimaryLevelMenu(Control actualNode, SceneTree tree) : base(actualNode, tree)
		{
			disposables = new List<IDisposable>();

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
			disposables.Add(settingsButtonNode);

			// Add the Exit App button
			Button exitAppButton = menuButtonResource.Instantiate<Button>();
			LevelMenuButton exitAppButtonHandler = new LevelMenuButton(exitAppButton);
			exitAppButton.Pressed += () =>
			{
				// Quit the app in the method specified in the Godot documentation:
				// https://docs.godotengine.org/en/stable/tutorials/inputs/handling_quit_requests.html
				tree.Root.PropagateNotification((int)Node.NotificationWMCloseRequest);
				tree.Quit();
			};
			exitAppButtonHandler.BackgroundColor = Color.Color8(255, 100, 89, 255);
			exitAppButtonHandler.Text = exitAppButton.Tr("EXIT_APP");
			exitAppButtonHandler.Icon = GD.Load<CompressedTexture2D>("res://addons/icons/logout_white_48dp.svg");
			disposables.Add(exitAppButton);

			menuButtonResource.Dispose();

			Button[] buttonList = {
				settingsButton.ActualButton,
				exitAppButton
			};
			float buttonYSize = exitAppButton.Size.Y;

			for (int i = 0; i < buttonList.Length; i++)
			{
				Button b = buttonList[i];
				b.OffsetTop = BUTTON_Y_POS_DIFF * i;
				b.OffsetBottom = b.OffsetTop + buttonYSize;
				ItemsControl.AddChild(b);
			}
		}

		public new void Dispose()
		{
			foreach (IDisposable obj in disposables)
			{
				if (obj is Node node)
				{
					node.QueueFree();
				}

				obj.Dispose();
			}

			base.Dispose();
		}

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }
	}
}
