using Godot;

using UTheCat.Jumpvalley.Core.Levels;

namespace JumpvalleyApp.Testing
{
    /// <summary>
    /// A test that allows a user to see what the difficulty colors of Jumpvalley's primary difficulties would look like.
    /// </summary>
    public partial class DifficultyColorTest : System.IDisposable
    {
        private Panel panel;

        public DifficultyColorTest()
        {
            
        }

        public void Dispose()
        {
            panel.Dispose();
        }
    }
}
