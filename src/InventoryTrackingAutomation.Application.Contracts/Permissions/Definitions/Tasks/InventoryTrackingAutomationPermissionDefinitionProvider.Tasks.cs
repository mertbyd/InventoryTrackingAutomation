using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddTasksPermissions(PermissionGroupDefinition myGroup)
    {
        var tasksPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Tasks.Default,
            L("Permission:Tasks"));

        tasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.Tasks.View,
            L("Permission:Tasks.View"));

        tasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.Tasks.Manage,
            L("Permission:Tasks.Manage"));

        tasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.Tasks.Complete,
            L("Permission:Tasks.Complete"));
    }
}
