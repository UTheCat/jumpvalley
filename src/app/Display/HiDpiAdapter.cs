using Godot;

namespace JumpvalleyApp.Display
{
    /// <summary>
    /// Class responsible for handling cases where a user is running the Jumpvalley app on a HiDPI display,
    /// affecting window width and height
    /// </summary>
    public partial class HiDpiAdapter
    {
        public SceneTree Tree { get; private set; }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                
            }
        }

        /// <summary>
        /// Construct a HiDpiAdapter.
        /// </summary>
        /// <param name="tree">This should be the app's primary scene tree</param>
        public HiDpiAdapter(SceneTree tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// The Jumpvalley app currently only has one window per process,
        /// so this function returns that one window.
        /// </summary>
        private Window GetMainWindow() => (Tree == null) ? null : Tree.Root;
    }
}
