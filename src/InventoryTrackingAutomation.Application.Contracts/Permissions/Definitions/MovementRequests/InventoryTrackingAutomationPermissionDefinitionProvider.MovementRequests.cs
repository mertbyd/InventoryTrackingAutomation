using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddMovementRequestsPermissions(PermissionGroupDefinition myGroup)
    {
        var movementRequestsPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.MovementRequests.Default,
            L("Permission:MovementRequests"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Create,
            L("Permission:MovementRequests.Create"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Edit,
            L("Permission:MovementRequests.Edit"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Delete,
            L("Permission:MovementRequests.Delete"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.View,
            L("Permission:MovementRequests.View"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Dispatch,
            L("Permission:MovementRequests.Dispatch"));

        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Receive,
            L("Permission:MovementRequests.Receive"));
    }
}
