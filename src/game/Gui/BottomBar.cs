using Godot;
using Jumpvalley.Animation;
using Jumpvalley.Music;
using Jumpvalley.Tweening;
using System;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that operates the GUI bar located at the bottom of the user's screen.
    /// <br/>
    /// This set of GUI is currently called the "BottomBar"
    /// </summary>
    public partial class BottomBar : IDisposable
    {
        public static readonly string MUSIC_DESC_NO_SONG = "MUSIC\nNo song playing";

        public enum LastHoveredButton
        {
            None, Menu, Music
        }

        public HBoxContainer ButtonsContainer;
        public Label DescriptionLabel;

        public Button MainMenuButton;
        public Button MusicButton;

        public Node ActualNode;
        public Node CurrentMusicPlayer;

        public Color DescriptionFontColor;
        public SceneTreeTween DescriptionOpacityTween = new SceneTreeTween(0.25, Tween.TransitionType.Linear, Tween.EaseType.InOut, null);

        public string MusicDescription = MUSIC_DESC_NO_SONG;

        public bool MainMenuButtonHovering = false;
        public bool MusicButtonHovering = false;

        public LastHoveredButton LastHovered = LastHoveredButton.None;

        public AnimatedNodeGroup AnimatedNodes;

        //private bool eventsConnected = false;

        /// <summary>
        /// Constructs a new instance of the BottomBar operator
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
            Playlist currentPlaylist = musicPlayer.CurrentPlaylist;
            if (currentPlaylist != null)
            {
                UpdateMusicDescription(currentPlaylist.CurrentSong);
            }
            else
            {
                UpdateMusicDescription(null);
            }

            musicPlayer.SongChanged += (object o, SongChangedArgs args) =>
            {
                UpdateMusicDescription(args.NewSong);
            };

            // allow the bottom bar's menu button to toggle the primary level menu
            MainMenuButton.Pressed += () =>
            {
                AnimatedNodeGroup nodes = AnimatedNodes;
                if (nodes != null)
                {
                    if (nodes.IsNodeVisible("primary_level_menu"))
                    {
                        nodes.Hide("primary_level_menu");
                    }
                    else
                    {
                        nodes.Show("primary_level_menu");
                    }
                }
            };

            // connect button hovering events to description label updating
            MainMenuButton.MouseEntered += () =>
            {
                MainMenuButtonHovering = true;
                LastHovered = LastHoveredButton.Menu;

                DescriptionLabel.Text = ActualNode.Tr("BOTTOM_BAR_MENU");
                RefreshDescriptionOpacity();
            };
            MainMenuButton.MouseExited += () =>
            {
                MainMenuButtonHovering = false;
                RefreshDescriptionOpacity();
            };

            // allow music panel visibility to be toggled by the music button
            MusicButton.Pressed += () =>
            {
                
            };

            MusicButton.MouseEntered += () =>
            {
                MusicButtonHovering = true;
                LastHovered = LastHoveredButton.Music;

                DescriptionLabel.Text = MusicDescription;
                RefreshDescriptionOpacity();
            };
            MusicButton.MouseExited += () =>
            {
                MusicButtonHovering = false;
                RefreshDescriptionOpacity();
            };

            SceneTree actualNodeTree = ActualNode.GetTree();

            // connect description label opacity tween to hovering and clicking stuff
            DescriptionOpacityTween.Tree = actualNodeTree;
            DescriptionOpacityTween.InitialValue = 0;
            DescriptionOpacityTween.FinalValue = 1;
            DescriptionOpacityTween.OnStep += (object o, float frac) =>
            {
                float opacity = (float) DescriptionOpacityTween.GetCurrentValue();

                // set visibility to false if description label is completely transparent
                DescriptionLabel.Visible = opacity > 0f;

                DescriptionFontColor.A = opacity;
                RefreshDescriptionColor();
            };
        }

        public void UpdateMusicDescription(Song song)
        {
            string musicDesc = ActualNode.Tr("BOTTOM_BAR_MUSIC") + "\n{0}";

            if (song == null)
            {
                MusicDescription = string.Format(musicDesc, ActualNode.Tr("NO_SONG_PLAYING"));
            }
            else
            {
                MusicDescription = string.Format(musicDesc, song.GetAttributionString());
            }

            if (LastHovered == LastHoveredButton.Music && DescriptionLabel.Visible)
            {
                DescriptionLabel.Text = MusicDescription;
            }
        }

        public void RefreshDescriptionColor()
        {
            DescriptionLabel.RemoveThemeColorOverride("font_color");
            DescriptionLabel.AddThemeColorOverride("font_color", DescriptionFontColor);
        }

        public bool CanShowDescription()
        {
            return MainMenuButtonHovering || MusicButtonHovering;
            /*
            return MainMenuButton.IsHovered() || MusicButton.IsHovered()
                || MainMenuButton.ButtonPressed || MusicButton.ButtonPressed;
            */
        }

        public void RefreshDescriptionOpacity()
        {
            if (CanShowDescription())
            {
                //Console.WriteLine("show");
                DescriptionOpacityTween.Speed = 1;
            }
            else
            {
                //Console.WriteLine("hide");
                DescriptionOpacityTween.Speed = -1;
            }

            DescriptionOpacityTween.Resume();
        }

        public void Dispose()
        {
            //ActualNode.Dispose();
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
