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
        public List<SettingBase> SettingList;

        public SettingGroup()
        {
            Subgroups = new List<SettingGroup>();
            SettingList = new List<SettingBase>();
        }

        /// <summary>
        /// Removes a setting from the setting list
        /// </summary>
        /// <param name="setting"></param>
        public void Remove(SettingBase setting)
        {
            if (setting != null)
            {
                int index = SettingList.IndexOf(setting);
                if (index >= 0)
                {
                    setting.Changed -= HandleSettingChanged;
                    SettingList.Remove(setting);
                }
            }
        }

        /// <summary>
        /// Adds a setting to the setting list
        /// </summary>
        /// <param name="setting"></param>
        public void Add(SettingBase setting)
        {
            if (setting == null || SettingList.Contains(setting)) return;

            SettingList.Add(setting);
            setting.Changed += HandleSettingChanged;
        }

        public void RemoveSettingGroup(SettingGroup group)
        {
            if (group != null)
            {
                int index = Subgroups.IndexOf(group);
                if (index >= 0)
                {
                    group.SettingChanged -= HandleSettingChangedFromSubgroup;
                }
            }
        }

        public void AddSettingGroup(SettingGroup group)
        {
            if (group == null || Subgroups.Contains(group)) return;

            Subgroups.Add(group);
            group.SettingChanged += HandleSettingChangedFromSubgroup;
        }

        public void Dispose()
        {
            foreach (SettingBase s in SettingList)
            {
                s.Changed -= HandleSettingChanged;
            }

            SettingList.Clear();

            foreach (SettingGroup group in Subgroups)
            {
                group.SettingChanged -= HandleSettingChangedFromSubgroup;
            }

            Subgroups.Clear();
        }

        private void HandleSettingChanged(object o, EventArgs _e)
        {
            SettingBase setting = o as SettingBase;
            if (setting != null)
            {
                RaiseSettingChanged(setting);
            }
        }

        private void HandleSettingChangedFromSubgroup(object o, SettingBase setting)
        {
            SettingGroup group = o as SettingGroup;
            if (group != null)
            {
                RaiseSubgroupSettingChanged(
                    new SubgroupSettingChangedArgs(group, setting)
                );
            }
        }

        /// <summary>
        /// Fired when one of the setting group's settings changes
        /// <br/>
        /// The EventArgs parameter is the <see cref="SettingBase"/> that had its value property changed. 
        /// </summary>
        public event EventHandler<SettingBase> SettingChanged;

        /// <summary>
        /// Fired when one of the subgroup's <see cref="SettingChanged"/> event gets raised. 
        /// </summary>
        public event EventHandler<SubgroupSettingChangedArgs> SubgroupSettingChanged;

        protected void RaiseSettingChanged(SettingBase setting)
        {
            SettingChanged?.Invoke(this, setting);
        }

        protected void RaiseSubgroupSettingChanged(SubgroupSettingChangedArgs args)
        {
            SubgroupSettingChanged?.Invoke(this, args);
        }
    }
}
