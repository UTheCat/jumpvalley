using System;
using Godot;
using Jumpvalley.Music;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that handles the music panel's functionality.
    /// The music panel displays some information about the currently playing song.
    /// </summary>
    public partial class MusicPanel : LevelMenu, IDisposable
    {
        /// <summary>
        /// The MusicPlayer we're working with
        /// </summary>
        private MusicPlayer musicPlayer;

        private Control actualNode;

        private Label songNameLabel;
        private Label artistsLabel;
        private Label menuTitleLabel;
        private HSlider volumeSlider;

        public MusicPanel(MusicPlayer musicPlayer, Control node, SceneTree tree) : base(node, tree)
        {
            this.musicPlayer = musicPlayer;
            actualNode = node;

            songNameLabel = node.GetNode<Label>("SongName");
            artistsLabel = node.GetNode<Label>("Artists");
            menuTitleLabel = node.GetNode<Label>("MenuTitle");
            volumeSlider = node.GetNode<HSlider>("Volume/Slider");

            Update();
            musicPlayer.SongChanged += HandleSongChanged;
        }

        public void Update()
        {
            if (musicPlayer != null)
            {
                Song currentSong = musicPlayer.CurrentPlaylist?.CurrentSong;

                if (currentSong == null)
                {
                    songNameLabel.Text = actualNode.Tr("NO_SONG_PLAYING");
                    artistsLabel.Visible = false;
                }
                else
                {
                    SongInfo info = currentSong.Info;
                    if (info == null)
                    {
                        songNameLabel.Text = actualNode.Tr("PLAYING_A_SONG");
                        artistsLabel.Visible = false;
                    }
                    else
                    {
                        songNameLabel.Text = info.Name;
                        artistsLabel.Text = info.Artists;
                        artistsLabel.Visible = true;
                    }
                }
            }
        }

        private void HandleSongChanged(object _o, SongChangedArgs _args)
        {
            Update();
        }

        public new void Dispose()
        {
            musicPlayer.SongChanged -= HandleSongChanged;

            base.Dispose();
        }
    }
}
