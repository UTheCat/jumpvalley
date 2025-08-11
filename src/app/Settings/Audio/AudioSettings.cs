using UTheCat.Jumpvalley.Core.Music;

namespace UTheCat.Jumpvalley.App.Settings.Audio
{
    public partial class AudioSettings : SettingGroup
    {
        public MusicPlayer MusicPlayer;

        public AudioSettings()
        {
            Id = "audio";
            LocalizationId = "SETTINGS_GROUP_AUDIO";
        }

        public void Initialize()
        {
            Add(new MusicVolumeControl(MusicPlayer));
        }
    }
}
