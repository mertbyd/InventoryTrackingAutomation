using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddTaskLinesPermissions(PermissionGroupDefinition myGroup)
    {
        var taskLinesPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.TaskLines.Default,
            L("Permission:TaskLines"));

        taskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.TaskLines.View,
            L("Permission:TaskLines.View"));

        taskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.TaskLines.Create,
            L("Permission:TaskLines.Create"));

        taskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.TaskLines.Edit,
            L("Permission:TaskLines.Edit"));

        taskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.TaskLines.Delete,
            L("Permission:TaskLines.Delete"));
    }
}
