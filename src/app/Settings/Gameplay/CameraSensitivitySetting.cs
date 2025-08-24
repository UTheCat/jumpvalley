using UTheCat.Jumpvalley.Core.Players.Camera;

namespace UTheCat.Jumpvalley.App.Settings.Gameplay
{
    public partial class CameraSensitivitySetting : RangeSetting
    {
        private BaseCamera camera;

        public CameraSensitivitySetting(BaseCamera camera)
        {
            RangeInstance.MinValue = 0;
            RangeInstance.MaxValue = 10;
            RangeInstance.Step = 0.001;
            RangeInstance.AllowLesser = true;
            RangeInstance.AllowGreater = true;

            Value = 1.0;
            Id = "camera_sensitivity";
            LocalizationId = "SETTINGS_CAMERA_SENSITIVITY";
            NumberFormat = "0.###";

            this.camera = camera;
        }

        public override void Update(object newValue)
        {
            // Type-checking is performed in RangeSetting.Update()
            base.Update(newValue);

            if (camera != null && newValue is double dVal)
            {
                camera.PanningSensitivity = (float)dVal;
            }
        }
    }
}
