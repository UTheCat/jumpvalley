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

        public Window HandledWindow { get; private set; }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

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
                    HandledWindow.SizeChanged += ResizeViewport;
                }
                else
                {
                    HandledWindow.SizeChanged -= ResizeViewport;
                }
            }
        }

        private int initialViewportWidth;
        private int initialViewportHeight;

        /// <summary>
        /// Construct a HiDpiAdapter.
        /// </summary>
        /// <param name="window">The window to handle HiDPI for</param>
        public HiDpiAdapter(Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window), "Attempted to pass a Window that doesn't exist.");
            HandledWindow = window;

            initialViewportWidth = ProjectSettings.GetSettingWithOverride(VIEWPORT_WIDTH_SETTING_PATH).As<int>();
            initialViewportHeight = ProjectSettings.GetSettingWithOverride(VIEWPORT_HEIGHT_SETTING_PATH).As<int>();
        }

        private void ResizeViewport()
        {
            if (!Enabled)
            {
                HandledWindow.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
                return;
            }

            Vector2I windowSize = HandledWindow.Size;
            HandledWindow.ContentScaleMode = (windowSize.X <= initialViewportWidth && windowSize.Y <= initialViewportHeight)
                ? Window.ContentScaleModeEnum.Disabled : Window.ContentScaleModeEnum.CanvasItems;
        }

        public void Dispose()
        {
            respondsToWindowResize = false;
        }
    }
}
