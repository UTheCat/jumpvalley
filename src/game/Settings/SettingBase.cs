using System;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents one setting (user toggle-able value) in Jumpvalley
    /// </summary>
    public partial class SettingBase
    {
        /// <summary>
        /// The internal identifier of the setting
        /// </summary>
        public string Id;

        /// <summary>
        /// The key in the localization table that points to the setting's name
        /// </summary>
        public string LocalizationId;

        /// <summary>
        /// The key in the localization table that points to the setting's description
        /// </summary>
        public string DescriptionLocalizationId;

        public object _value;

        /// <summary>
        /// The setting's value
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value)) return;

                _value = value;
                RaiseChanged();
            }
        }

        /// <summary>
        /// Event raised when the value of <see cref="Value"/> changes 
        /// </summary>
        public event EventHandler Changed;

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
