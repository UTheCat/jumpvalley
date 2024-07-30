using System;
using Godot;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents one setting (user toggle-able value) in Jumpvalley
    /// </summary>
    public partial class SettingBase : Node
    {
        private string _id;

        /// <summary>
        /// The internal identifier of the setting
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                Name = value;
            }
        }

        /// <summary>
        /// The key in the localization table that points to the setting's name
        /// </summary>
        public string LocalizationId;

        /// <summary>
        /// The key in the localization table that points to the setting's description
        /// </summary>
        public string DescriptionLocalizationId;

        /// <summary>
        /// The action map key (typically one corresponding to a keybind)
        /// to associate with this setting.
        /// </summary>
        public string ActionMapKey;

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
                Update(value);
                RaiseChanged();
            }
        }

        /// <summary>
        /// Update function for responding to changes to the setting's value.
        /// It's called whenever <see cref="Value"/> is changed.
        /// <br/><br/>
        /// This function should be overriden by classes inheriting from <see cref="SettingBase"/>. 
        /// </summary>
        /// <param name="newValue">The new value of the <see cref="Value"/> variable</param>
        public virtual void Update(object newValue) { }

        /// <summary>
        /// Calls the <see cref="Update(object)"/> method using the current value
        /// of the <see cref="Value"/> variable. 
        /// </summary>
        public void Update()
        {
            Update(Value);
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
