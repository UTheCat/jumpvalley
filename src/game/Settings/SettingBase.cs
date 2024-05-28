using System;
using Godot;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents one setting (user toggle-able value) in Jumpvalley
    /// </summary>
    public partial class SettingBase : Node
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

        /// <summary>
        /// Update function for responding to changes to the setting's value.
        /// This function should be overriden by classes inheriting from <see cref="SettingBase"/>. 
        /// </summary>
        public virtual void Update() { }

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
