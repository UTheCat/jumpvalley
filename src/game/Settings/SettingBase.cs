using System;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents one setting (user toggle-able value) in Jumpvalley
    /// </summary>
    public partial class SettingBase<T>
    {
        /// <summary>
        /// The key in the localization table that points to the setting's name
        /// </summary>
        public string LocalizationId;

        /// <summary>
        /// The key in the localization table that points to the setting's description
        /// </summary>
        public string DescriptionLocalizationId;

        public T _value;

        /// <summary>
        /// The setting's value
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value)) return;

                _value = value;
            }
        }

        /// <summary>
        /// Event raised when the value of <see cref="Value"/> changes 
        /// </summary>
        public event EventHandler Changed;
    }
}
