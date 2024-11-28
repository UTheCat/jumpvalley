using Godot;
using System;

using JumpvalleyApp.Players.Camera;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Indicator that's shown when the player's camera's rotation
    /// is responding to user input
    /// and hidden otherwise.
    /// </summary>
    public partial class CameraTurnIndicator : IDisposable
    {
        private UserInputCamera userInputCamera;
        private Control actualControl;

        public CameraTurnIndicator(UserInputCamera camera, Control initActualControl)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera), $"Attempted to pass a {nameof(UserInputCamera)} that doesn't exist.");
            if (initActualControl == null) throw new ArgumentNullException(nameof(initActualControl), $"Attempted to pass a {nameof(Control)} that doesn't exist.");

            userInputCamera = camera;
            actualControl = initActualControl;

            ToggleIndicatorVisibility(userInputCamera.IsTurningCamera);
            userInputCamera.IsTurningCameraToggled += HandleIsTurningCameraToggled;
        }

        private void ToggleIndicatorVisibility(bool isVisible)
        {
            actualControl.Visible = isVisible;
        }

        private void HandleIsTurningCameraToggled(object _o, bool enabled)
        {
            ToggleIndicatorVisibility(enabled);
        }

        public void Dispose()
        {
            userInputCamera.IsTurningCameraToggled -= HandleIsTurningCameraToggled;
        }
    }
}
