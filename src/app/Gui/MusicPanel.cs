using System;
using Godot;
using UTheCat.Jumpvalley.Core.Music;

namespace UTheCat.Jumpvalley.App.Gui
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

        private HSlider volumeSlider;
        private Label volumePercentage;

        public MusicPanel(MusicPlayer musicPlayer, Control node, SceneTree tree) : base(node, tree)
        {
            this.musicPlayer = musicPlayer;
            actualNode = node;

            songNameLabel = node.GetNode<Label>("SongName");
            artistsLabel = node.GetNode<Label>("Artists");

            Node volumeNode = node.GetNode("Volume");
            volumeSlider = volumeNode.GetNode<HSlider>("Slider");
            volumePercentage = volumeNode.GetNode<Label>("Percentage");

            Update();
            musicPlayer.SongChanged += HandleSongChanged;

            volumeSlider.MinValue = 0;
            volumeSlider.MaxValue = 1;
            volumeSlider.Step = 0.01;
            if (musicPlayer != null)
            {
                volumeSlider.Value = musicPlayer.VolumeScale;
                UpdateVolumePercentageText();
                volumeSlider.ValueChanged += HandleVolumeSliderChanged;
            }
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

        private void UpdateVolumePercentageText()
        {
            volumePercentage.Text = $"{(int)(volumeSlider.Value * 100)}%";
        }

        private void HandleSongChanged(object _o, SongChangedArgs _args)
        {
            Update();
        }

        private void HandleVolumeSliderChanged(double newVolume)
        {
            if (musicPlayer != null)
            {
                musicPlayer.VolumeScale = newVolume;
                UpdateVolumePercentageText();
            }
        }

        public new void Dispose()
        {
            musicPlayer.SongChanged -= HandleSongChanged;

            base.Dispose();
        }
    }
}
