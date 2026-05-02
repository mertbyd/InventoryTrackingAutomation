namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class Workflows
    {
        public const string Default = GroupName + ".Workflows";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
        public const string View = Default + ".View";
    }
}
