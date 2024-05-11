using Godot;
using Jumpvalley.Music;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that handles the music panel's functionality.
    /// The music panel displays some information about the currently playing song.
    /// </summary>
    public partial class MusicPanel : LevelMenu
    {
        /// <summary>
        /// The MusicPlayer we're working with
        /// </summary>
        private MusicPlayer musicPlayer;

        private Control actualNode;

        private Label songNameLabel;
        private Label artistsLabel;
        private HSlider volumeControl;

        public MusicPanel(MusicPlayer musicPlayer, Control node, SceneTree tree) : base(node, tree)
        {
            this.musicPlayer = musicPlayer;
            actualNode = node;

            songNameLabel = node.GetNode<Label>("SongName");
            artistsLabel = node.GetNode<Label>("Artists");
            volumeControl = node.GetNode<HSlider>("VolumeControl");
        }

        public void Update()
        {
            if (musicPlayer != null)
            {
                Song currentSong = musicPlayer.CurrentPlaylist?.CurrentSong;
            }
        }
    }
}
