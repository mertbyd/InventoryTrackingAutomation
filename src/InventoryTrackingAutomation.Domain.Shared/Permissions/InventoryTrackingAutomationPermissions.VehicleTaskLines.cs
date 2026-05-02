namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class VehicleTaskLines
    {
        public const string Default = GroupName + ".VehicleTaskLines";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
