namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }
}
