using UTheCat.Jumpvalley.Core.Music;

namespace UTheCat.Jumpvalley.App.Settings.Audio
{
    /// <summary>
    /// Setting that allows the user to control how loud the in-app music is.
    /// </summary>
    public partial class MusicVolumeControl : RangeSetting
    {
        private MusicPlayer musicPlayer;

        public MusicVolumeControl(MusicPlayer musicPlayer)
        {
            RangeInstance.MinValue = 0;
            RangeInstance.MaxValue = 1;
            RangeInstance.Step = 0.01;
            RangeInstance.AllowLesser = false;
            RangeInstance.AllowGreater = false;

            Value = 1.0;
            Id = "music_volume";
            LocalizationId = "SETTINGS_MUSIC_VOLUME";

            this.musicPlayer = musicPlayer;
        }

        public override void Update(object newValue)
        {
            base.Update(newValue);

            if (musicPlayer != null && newValue is double dVal) musicPlayer.VolumeScale = dVal;
        }
    }
}
