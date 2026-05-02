namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class VehicleTasks
    {
        public const string Default = GroupName + ".VehicleTasks";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }
}
