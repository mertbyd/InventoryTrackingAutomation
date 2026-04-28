using Volo.Abp.Reflection;

namespace InventoryTrackingAutomation.Permissions;

public class InventoryTrackingAutomationPermissions
{
    public const string GroupName = "InventoryTrackingAutomation";

    public static class MovementRequests
    {
        public const string Default = GroupName + ".MovementRequests";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
    }

    public static class Workflows
    {
        public const string Default = GroupName + ".Workflows";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
        public const string View = Default + ".View";
    }

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }

    public static class Tasks
    {
        public const string Default = GroupName + ".Tasks";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }

    public static class VehicleTasks
    {
        public const string Default = GroupName + ".VehicleTasks";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }

    public static class Masters
    {
        public const string Default = GroupName + ".Masters";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(InventoryTrackingAutomationPermissions));
    }
}
