using Godot;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that handles the music panel's functionality.
    /// The music panel displays some information about the currently playing song.
    /// </summary>
    public partial class MusicPanel : LevelMenu
    {
        private Control actualNode;

        private Label songNameLabel;
        private Label artistsLabel;
        private HSlider volumeControl;

        public MusicPanel(Control node, SceneTree tree) : base(node, tree)
        {
            actualNode = node;

            songNameLabel = node.GetNode<Label>("SongName");
            artistsLabel = node.GetNode<Label>("Artists");
            volumeControl = node.GetNode<HSlider>("VolumeControl");
        }
    }
}
