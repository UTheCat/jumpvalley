using System;
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

            RangeInstance.MinValue = 0;
            RangeInstance.MaxValue = 4;
            RangeInstance.Step = 0.001;

            this.camera = camera;
        }

        public override void Update(object newValue)
        {
            Console.WriteLine($"Camera sensitivity updated. New value: {(double)newValue}");

            // Type-checking is performed in RangeSetting.Update()
            base.Update(newValue);

            if (camera != null && newValue is double dVal)
            {
                camera.PanningSensitivity = (float)dVal;
            }
        }
    }
}
