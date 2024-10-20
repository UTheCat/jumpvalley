using Godot;
using System;

namespace JumpvalleyApp.Display
{
    /// <summary>
    /// Class responsible for handling cases where a user is running the Jumpvalley app on a HiDPI display,
    /// affecting window width and height
    /// </summary>
    public partial class HiDpiAdapter : IDisposable
    {
        private static readonly Vector2I MAX_WINDOW_SIZE = new Vector2I(1920, 1080);

        public SceneTree Tree { get; private set; }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                Window window = GetMainWindow();
                if (value)
                {
                    window.MaxSize = MAX_WINDOW_SIZE;
                    window.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;

                    respondsToWindowResize = true;
                }
                else
                {
                    respondsToWindowResize = false;

                    window.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
                    window.MaxSize = Vector2I.Zero;
                }
            }
        }

        private Window window;

        private bool _respondsToWindowResize;
        private bool respondsToWindowResize
        {
            get => _respondsToWindowResize;
            set
            {
                if (_respondsToWindowResize == value) return;

                if (value)
                {
                    if (window == null) window = GetMainWindow();

                    ResizeViewport();
                    window.SizeChanged += ResizeViewport;
                }
                else
                {
                    ResizeViewport();
                    window.SizeChanged -= ResizeViewport;
                }
            }
        }

        /// <summary>
        /// Construct a HiDpiAdapter.
        /// </summary>
        /// <param name="tree">This should be the app's primary scene tree</param>
        public HiDpiAdapter(SceneTree tree)
        {
            Tree = tree;
            window = null;
        }

        /// <summary>
        /// The Jumpvalley app currently only has one window per process,
        /// so this function returns that one window.
        /// </summary>
        private Window GetMainWindow() => (Tree == null) ? null : Tree.Root;

        private void ResizeViewport()
        {
            if (window != null)
            {
                if (!Enabled)
                {
                    window.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
                    return;
                }

                Vector2I windowSize = window.Size;
                window.ContentScaleMode = (windowSize.X <= MAX_WINDOW_SIZE.X && windowSize.Y <= MAX_WINDOW_SIZE.Y)
                    ? Window.ContentScaleModeEnum.Disabled : Window.ContentScaleModeEnum.Viewport;
            }
        }

        public void Dispose()
        {
            respondsToWindowResize = false;
        }
    }
}
