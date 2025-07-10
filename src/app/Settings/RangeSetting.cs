using System;

using UTheCat.Jumpvalley.App.Settings;

namespace UTheCat.Jumpvalley.App
{
    /// <summary>
    /// Numerical setting based on a defined minimum and maximum
    /// </summary>
    public partial class RangeSetting : SettingBase, IDisposable
    {
        /// <summary>
        /// The Range instance (from the Godot API) that's handling this range setting.
        /// To ensure consistency, don't directly set this object's Value property.
        /// Instead, set this <see cref="RangeSetting"/>'s Value property. 
        /// </summary>
        public Godot.Range RangeInstance { get; private set; } = new Godot.Range();

        public override object Value
        {
            get => base.Value;
            set
            {
                if (!(value is float dValue)) throw new ArgumentException("This setting's value must be a double.");

                RangeInstance.Value = dValue;
                base.Value = RangeInstance.Value;
            }
        }
    }
}
