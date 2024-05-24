using System;
using System.Collections.Generic;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents a group/category of settings
    /// </summary>
    public partial class SettingGroup : IDisposable
    {
        public string Id;
        public string LocalizationId;
        public List<SettingGroup> Subgroups;
        public List<SettingBase<object>> SettingList;

        /// <summary>
        /// Removes a setting from the setting list
        /// </summary>
        /// <param name="setting"></param>
        public void Remove(SettingBase<object> setting)
        {
            if (setting != null)
            {
                int index = SettingList.IndexOf(setting);
                if (index >= 0)
                {
                    SettingList.Remove(setting);
                }
            }
        }

        /// <summary>
        /// Adds a setting to the setting list
        /// </summary>
        /// <param name="setting"></param>
        public void Add(SettingBase<object> setting)
        {
            if (setting == null) return;

            SettingList.Add(setting);
            setting.Changed += HandleSettingChanged;
        }

        public void Dispose()
        {
            foreach (SettingBase<object> s in SettingList)
            {
                s.Changed -= HandleSettingChanged;
            }

            SettingList.Clear();
        }

        private void HandleSettingChanged(object o, EventArgs _e)
        {
            SettingBase<object> setting = o as SettingBase<object>;
            if (setting != null)
            {
                RaiseSettingChanged(setting);
            }
        }

        /// <summary>
        /// Fired when one of the setting group's settings changes
        /// <br/>
        /// The EventArgs parameter is the setting that was changed as a <see cref="SettingBase{T}"/>. 
        /// </summary>
        public event EventHandler<SettingBase<object>> SettingChanged;

        protected void RaiseSettingChanged(SettingBase<object> setting)
        {
            SettingChanged?.Invoke(this, setting);
        }
    }
}
