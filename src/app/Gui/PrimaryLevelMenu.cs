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
		private static readonly string KEYBIND_OPEN_MENU_META_NAME = "keybind_open_menu";

		/// <summary>
		/// Keyboard shortcuts for the menu's options (other than the close button).
		/// <br/><br/>
		/// Higher index means the keyboard shortcut is used for a menu option that's
		/// lower in the option list displayed to the user.
		/// </summary>
		private static readonly Key[] MENU_OPTION_KEYBINDS = [
			Key.Key1,
			Key.Key2,
			Key.Key3,
			Key.Key4,
			Key.Key5,
			Key.Key6,
			Key.Key7,
			Key.Key8,
			Key.Key9
		];

		private List<IDisposable> disposables;
		private List<Button> menuButtons = new List<Button>();

		/// <summary>
		/// Keyboard shortcut for opening the PrimaryLevelMenu specified in the metadata of the PrimaryLevelMenu's root node.
		/// <br/><br/>
		/// Set this to null to indicate that this keybinding shouldn't be made (or to terminate the keybind).
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

		public override bool IsVisible
		{
			get => base.IsVisible;
			set
			{
				if (value) foreach (Button b in menuButtons) b.Disabled = false;

				base.IsVisible = value;
			}
		}

		public PrimaryLevelMenu(Control actualNode, SceneTree tree) : base(actualNode, tree)
		{
			disposables = new List<IDisposable>();

			ScrollContainer scrollContainer = ItemsScrollContainer;
			if (scrollContainer != null) scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;

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
				if (!IsVisible) return;
				DisableButtonsExceptFor(settingsButtonNode);

				IsVisible = false;
				CurrentSettingsMenu.IsVisible = true;
			};
			disposables.Add(settingsButtonNode);
			menuButtons.Add(settingsButtonNode);

			// Add the Exit App button
			Button exitAppButton = menuButtonResource.Instantiate<Button>();
			LevelMenuButton exitAppButtonHandler = new LevelMenuButton(exitAppButton);
			exitAppButton.Pressed += () =>
			{
				if (!IsVisible) return;

				// Quit the app in the method specified in the Godot documentation:
				// https://docs.godotengine.org/en/stable/tutorials/inputs/handling_quit_requests.html
				tree.Root.PropagateNotification((int)NotificationWMCloseRequest);
				tree.Quit();
			};
			exitAppButtonHandler.BackgroundColor = Color.Color8(255, 100, 89, 255);
			exitAppButtonHandler.Text = exitAppButton.Tr("EXIT_APP");
			exitAppButtonHandler.Icon = GD.Load<CompressedTexture2D>("res://addons/icons/logout_white_48dp.svg");
			disposables.Add(exitAppButton);
			menuButtons.Add(exitAppButton);

			menuButtonResource.Dispose();

			for (int i = 0; i < menuButtons.Count; i++)
			{
				Button b = menuButtons[i];
				if (i < MENU_OPTION_KEYBINDS.Length)
				{
					InputEventKey key = new InputEventKey();
					key.Keycode = MENU_OPTION_KEYBINDS[i];

					Shortcut shortcut = new Shortcut();
					shortcut.Events.Add(key);
					b.Shortcut = shortcut;
				}

				// We're only disabling the buttons to prevent them from playing shortcut feedback
				// if the button is pressed while the menu is being hidden.
				// Therefore, we should be fine with hiding the fact that the buttons could get disabled
				// from the end user.
				b.AddThemeStyleboxOverride("disabled", b.GetThemeStylebox("normal"));

				ScrollableItemsBoxContainer.AddChild(b);
			}

			if (actualNode.HasMeta(KEYBIND_OPEN_MENU_META_NAME))
			{
				InputEventKey keybind = actualNode.GetMeta(KEYBIND_OPEN_MENU_META_NAME).As<InputEventKey>();
				if (keybind != null) keybindOpenMenu = keybind;
			}

			// No need to call SetButtonsDisabled as it's already called when base constructor runs (which sets IsVisible to false)
		}

		/// <summary>
		/// Disables every button except for <paramref name="bStayEnabled"/>.
		/// <br/><br/>
		/// Use this to prevent shortcut feedback for menu buttons that are selected while menu is being hidden.
		/// </summary>
		private void DisableButtonsExceptFor(Button bStayEnabled)
		{
			foreach (Button b in menuButtons)
			{
				if (b != bStayEnabled) b.Disabled = true;
			}
		}

		public new void Dispose()
		{
			keybindOpenMenu = null;

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
			BgPanelAnimatedNodeGroup bgpNodeGroup = BgPanelNodeGroup;
			if ((bgpNodeGroup == null || !bgpNodeGroup.ShouldBeVisible)
				&& @event is InputEventKey keyInput
				&& keyInput.Keycode == keybindOpenMenu.Keycode
				&& keyInput.IsPressed()
				)
			{
				IsVisible = true;
				return;
			}

			base._Input(@event);
		}
	}
}
