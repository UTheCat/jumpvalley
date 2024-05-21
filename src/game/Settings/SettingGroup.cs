using System.Collections.Generic;

namespace JumpvalleyGame.Settings
{
    /// <summary>
    /// Class that represents a group/category of settings
    /// </summary>
    public partial class SettingGroup
    {
        public string LocalizationId;
        public List<SettingGroup> Subgroups;
        public List<SettingBase<object>> SettingList;
    }
}
