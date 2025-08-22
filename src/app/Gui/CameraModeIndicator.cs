using Godot;
using System;

using UTheCat.Jumpvalley.App.Players.Camera;

namespace UTheCat.Jumpvalley.App.Gui
{
    /// <summary>
    /// Indicator that's shown when the player's camera's rotation is responding to user input and hidden otherwise.
    /// <br/><br/>
    /// Indicator appearance changes when switching between first-person and third-person.
    /// </summary>
    public partial class CameraModeIndicator : IDisposable
    {
        private UserInputCamera userInputCamera;
        private Control actualControl;
        private TextureRect cameraModeTexture;

        private CompressedTexture2D cameraTurnDecal;
        private CompressedTexture2D cameraTurnFirstPersonDecal;

        public CameraModeIndicator(UserInputCamera camera, Control initActualControl)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera), $"Attempted to pass a {nameof(UserInputCamera)} that doesn't exist.");
            if (initActualControl == null) throw new ArgumentNullException(nameof(initActualControl), $"Attempted to pass a {nameof(Control)} that doesn't exist.");

            userInputCamera = camera;
            actualControl = initActualControl;
            cameraModeTexture = initActualControl.GetNode<TextureRect>("ModeTexture");
            cameraTurnDecal = ResourceLoader.Load<CompressedTexture2D>("res://decals/camera_turn.svg");
            cameraTurnFirstPersonDecal = ResourceLoader.Load<CompressedTexture2D>("res://decals/camera_turn_first_person.svg");

            ToggleIndicatorVisibility(userInputCamera.IsTurningCamera);
            userInputCamera.IsTurningCameraToggled += HandleIsTurningCameraToggled;
            userInputCamera.ZoomOutDistanceChanged += HandleZoomOutDistanceChanged;
        }

        private void ToggleIndicatorVisibility(bool isVisible)
        {
            actualControl.Visible = isVisible;
        }

        private void HandleIsTurningCameraToggled(object _o, bool enabled)
        {
            ToggleIndicatorVisibility(enabled);
        }

        private void HandleZoomOutDistanceChanged(object _o, float zoomOutDist)
        {
            cameraModeTexture.Texture = (zoomOutDist <= 0f) ? cameraTurnFirstPersonDecal : cameraTurnDecal;
        }

        public void Dispose()
        {
            userInputCamera.IsTurningCameraToggled -= HandleIsTurningCameraToggled;
            userInputCamera.ZoomOutDistanceChanged -= HandleZoomOutDistanceChanged;

            cameraTurnDecal.Dispose();
            cameraTurnFirstPersonDecal.Dispose();
        }
    }
}
