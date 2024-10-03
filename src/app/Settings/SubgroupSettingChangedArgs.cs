namespace JumpvalleyApp.Settings
{
    /// <summary>
    /// Event arguments for <see cref="SettingGroup.SubgroupSettingChanged"/> 
    /// </summary>
    public partial class SubgroupSettingChangedArgs
    {
        /// <summary>
        /// The subgroup that had one of its settings changed
        /// </summary>
        public SettingGroup Subgroup;

        /// <summary>
        /// The setting within the subgroup that had its value changed
        /// </summary>
        public SettingBase Setting;

        public SubgroupSettingChangedArgs(SettingGroup subgroup, SettingBase setting)
        {
            Subgroup = subgroup;
            Setting = setting;
        }
    }
}
