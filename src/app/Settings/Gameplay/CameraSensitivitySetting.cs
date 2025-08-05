using UTheCat.Jumpvalley.Core.Players.Camera;

namespace UTheCat.Jumpvalley.App.Settings.Gameplay
{
    public partial class CameraSensitivitySetting : RangeSetting
    {
        private BaseCamera camera;

        public CameraSensitivitySetting(BaseCamera camera)
        {
            Value = 1.0;
            Id = "camera_sensitivity";
            LocalizationId = "SETTINGS_CAMERA_SENSITIVITY";
            this.camera = camera;
        }

        public override void Update(object newValue)
        {
            // Type-checking is performed in RangeSetting.Update()
            base.Update(newValue);

            camera.PanningSensitivity = (float)newValue;
        }
    }
}
