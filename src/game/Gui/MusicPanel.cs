using Godot;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Class that handles the music panel's functionality.
    /// The music panel displays some information about the currently playing song.
    /// </summary>
    public partial class MusicPanel
    {
        private Control actualNode;

        public MusicPanel(Control node)
        {
            actualNode = node;
        }
    }
}
