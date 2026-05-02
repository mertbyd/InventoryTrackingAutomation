using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddWorkflowsPermissions(PermissionGroupDefinition myGroup)
    {
        var workflowsPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Workflows.Default,
            L("Permission:Workflows"));

        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.Approve,
            L("Permission:Workflows.Approve"));

        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.Reject,
            L("Permission:Workflows.Reject"));

        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.View,
            L("Permission:Workflows.View"));
    }
}
