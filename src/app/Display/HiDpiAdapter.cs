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
        private static readonly string VIEWPORT_WIDTH_SETTING_PATH = "display/window/size/viewport_width";
        private static readonly string VIEWPORT_HEIGHT_SETTING_PATH = "display/window/size/viewport_height";

        public SceneTree Tree { get; private set; }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                if (window == null) window = GetMainWindow();
                if (window == null) return;

                if (value)
                {
                    ResizeViewport();
                    respondsToWindowResize = true;
                }
                else
                {
                    respondsToWindowResize = false;
                    ResizeViewport();
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
                _respondsToWindowResize = value;

                if (value)
                {
                    window.SizeChanged += ResizeViewport;
                }
                else
                {
                    window.SizeChanged -= ResizeViewport;
                }
            }
        }

        private int initialViewportWidth;
        private int initialViewportHeight;

        /// <summary>
        /// Construct a HiDpiAdapter.
        /// </summary>
        /// <param name="tree">This should be the app's primary scene tree</param>
        public HiDpiAdapter(SceneTree tree)
        {
            Tree = tree;
            window = null;

            initialViewportWidth = ProjectSettings.GetSettingWithOverride(VIEWPORT_WIDTH_SETTING_PATH).As<int>();
            initialViewportHeight = ProjectSettings.GetSettingWithOverride(VIEWPORT_HEIGHT_SETTING_PATH).As<int>();
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
                window.ContentScaleMode = (windowSize.X <= initialViewportWidth && windowSize.Y <= initialViewportHeight)
                    ? Window.ContentScaleModeEnum.Disabled : Window.ContentScaleModeEnum.CanvasItems;
            }
        }

        public void Dispose()
        {
            respondsToWindowResize = false;
        }
    }
}
