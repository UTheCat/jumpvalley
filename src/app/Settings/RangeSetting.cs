using UTheCat.Jumpvalley.App.Settings;

namespace UTheCat.Jumpvalley.App
{
    /// <summary>
    /// Numerical setting which has a minimum and maximum.
    /// </summary>
    public partial class RangeSetting : SettingBase
    {
        private double _increment = 0;

        /// <summary>
        /// The setting's value will be rounded to a multiple of this number.
        /// </summary>
        public double Increment
        {
            get => _increment;
            set
            {
                _increment = value;
            }
        }

        private double _minValue = 0;

        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
            }
        }

        private double _maxValue = 0;

        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
            }
        }
    }
}
