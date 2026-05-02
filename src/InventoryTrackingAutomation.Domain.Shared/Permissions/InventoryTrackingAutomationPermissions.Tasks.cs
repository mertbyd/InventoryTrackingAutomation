namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class Tasks
    {
        public const string Default = GroupName + ".Tasks";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
        public const string Complete = Default + ".Complete";
    }
}
