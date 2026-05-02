namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissions
{
    public static class MovementRequests
    {
        public const string Default = GroupName + ".MovementRequests";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string Dispatch = Default + ".Dispatch";
        public const string Receive = Default + ".Receive";
    }
}
