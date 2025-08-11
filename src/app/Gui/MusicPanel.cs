using System;
using Godot;
using UTheCat.Jumpvalley.App.Settings.Audio;
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
        private MusicVolumeControl musicVolumeSetting = null;

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

        private void HandleSongChanged(object _o, SongChangedArgs _args) => Update();

        private void HandleVolumeSliderChanged(double newVolume)
        {
            if (musicPlayer != null)
            {
                if (musicVolumeSetting == null)
                {
                    musicPlayer.VolumeScale = newVolume;
                    UpdateVolumePercentageText();
                }
                else
                {
                    // musicVolumeSetting will update the music player's volume for us,
                    // assuming musicVolumeSetting is handling the same music player
                    // as this music panel handler.
                    musicVolumeSetting.Value = newVolume;
                }
            }
        }

        private void OnExternalVolumeSettingChanged(object _o, EventArgs _e)
        {
            if (musicVolumeSetting != null && musicVolumeSetting.Value is double newVolume)
            {
                volumeSlider.SetValueNoSignal(newVolume);
                volumePercentage.Text = $"{(int)(newVolume * 100)}%";

                // We don't need to update musicPlayer.VolumeScale at this point since
                // musicVolumeSetting has already done that.
            }
        }

        public void UnbindMusicVolumeSliderFromSetting()
        {
            if (musicVolumeSetting == null) return;

            musicVolumeSetting.Changed -= OnExternalVolumeSettingChanged;
            musicVolumeSetting = null;
        }

        /// <summary>
        /// Binds this music panel's volume slider with a <see cref="MusicVolumeControl"/>. 
        /// </summary>
        public void BindMusicVolumeSliderWithSetting(MusicVolumeControl volumeSetting)
        {
            if (musicVolumeSetting != null) return;

            musicVolumeSetting = volumeSetting;
            musicVolumeSetting.Changed += OnExternalVolumeSettingChanged;
        }

        public new void Dispose()
        {
            musicPlayer.SongChanged -= HandleSongChanged;
            UnbindMusicVolumeSliderFromSetting();

            base.Dispose();
        }
    }
}
