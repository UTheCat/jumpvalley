using Godot;
using Jumpvalley.Music;
using Jumpvalley.Tweening;
using System;

namespace Jumpvalley.Players.Gui
{
    /// <summary>
    /// Class that operates the GUI bar located at the bottom of the user's screen.
    /// <br/>
    /// This set of GUI is currently called the "BottomBar"
    /// </summary>
    public partial class BottomBar: IDisposable
    {
        public static readonly string MUSIC_DESC_NO_SONG = "MUSIC\nNo song playing";

        public HBoxContainer ButtonsContainer;
        public Label DescriptionLabel;

        public Button MainMenuButton;
        public Button MusicButton;

        public Node ActualNode;
        public Node CurrentMusicPlayer;

        public Color DescriptionFontColor;
        public SceneTreeTween DescriptionOpacityTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.InOut, null);

        public string MusicDescription = MUSIC_DESC_NO_SONG;

        //private bool eventsConnected = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bottomBarNode">The actual node that represents the BottomBar</param>
        /// <param name="musicPlayer">The music player to bind the BottomBar to</param>
        public BottomBar(Node bottomBarNode, MusicPlayer musicPlayer)
        {
            ActualNode = bottomBarNode;
            CurrentMusicPlayer = musicPlayer;

            ButtonsContainer = (HBoxContainer)ActualNode.GetNode("Buttons");
            DescriptionLabel = (Label)ActualNode.GetNode("Description");

            MainMenuButton = (Button)ButtonsContainer.GetNode("Menu");
            MusicButton = (Button)ButtonsContainer.GetNode("Music");

            DescriptionLabel.Visible = false;
            DescriptionFontColor = DescriptionLabel.GetThemeColor("font_color");
            DescriptionFontColor.A = 0f;
            RefreshDescriptionColor();

            // listen to music player song changes
            musicPlayer.SongChanged += (object o, SongChangedArgs args) =>
            {
                Song song = args.NewSong;
                if (song == null)
                {
                    MusicDescription = MUSIC_DESC_NO_SONG;
                }
                else
                {
                    MusicDescription = $"MUSIC\n{song.Artists} - {song.Name}";
                }
            };

            // connect button hovering events to description label updating
            MainMenuButton.MouseEntered += () =>
            {
                DescriptionLabel.Text = "Menu";
                RefreshDescriptionOpacity();
            };
            MainMenuButton.MouseExited += RefreshDescriptionOpacity;

            MusicButton.MouseEntered += () =>
            {
                DescriptionLabel.Text = MusicDescription;
                RefreshDescriptionOpacity();
            };
            MusicButton.MouseExited += RefreshDescriptionOpacity;

            // connect description label opacity tween to hovering and clicking stuff
            DescriptionOpacityTween.Tree = ActualNode.GetTree();
            DescriptionOpacityTween.InitialValue = 0;
            DescriptionOpacityTween.FinalValue = 1;
            DescriptionOpacityTween.OnStep += (object o, float frac) =>
            {
                float opacity = (float) DescriptionOpacityTween.GetCurrentValue();

                // set visibility to false if description label is completely transparent
                DescriptionLabel.Visible = opacity == 0f;

                DescriptionFontColor.A = opacity;
                RefreshDescriptionColor();
            };
        }

        public void RefreshDescriptionColor()
        {
            DescriptionLabel.RemoveThemeColorOverride("font_color");
            DescriptionLabel.AddThemeColorOverride("font_color", DescriptionFontColor);
        }

        public bool CanShowDescription()
        {
            return (MainMenuButton.IsHovered() || MusicButton.IsHovered()
                || MainMenuButton.ButtonPressed || MusicButton.ButtonPressed);
        }

        public void RefreshDescriptionOpacity()
        {
            if (CanShowDescription())
            {
                DescriptionOpacityTween.Speed = 1;
            }
            else
            {
                DescriptionOpacityTween.Speed = -1;
            }

            DescriptionOpacityTween.Resume();
        }

        public void Dispose()
        {
            ActualNode.Dispose();
            DescriptionOpacityTween.Dispose();
        }

        /*
        public void DisconnectEvents()
        {
            if (eventsConnected)
            {
                eventsConnected = false;
            }
        }

        public void ConnectEvents()
        {
            if (!eventsConnected)
            {
                eventsConnected = true;
            }
        }

        public void Dispose()
        {
            ActualNode.Dispose();
        }

        protected void OnMenuButtonHover()
        {

        }

        protected void OnMusicButtonHover()
        {

        }
        */
    }
}
