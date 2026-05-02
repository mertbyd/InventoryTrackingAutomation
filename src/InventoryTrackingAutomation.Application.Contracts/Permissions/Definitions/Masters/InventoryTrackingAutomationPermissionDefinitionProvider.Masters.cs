using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddMastersPermissions(PermissionGroupDefinition myGroup)
    {
        var mastersPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Masters.Default,
            L("Permission:Masters"));

        mastersPermission.AddChild(
            InventoryTrackingAutomationPermissions.Masters.View,
            L("Permission:Masters.View"));

        mastersPermission.AddChild(
            InventoryTrackingAutomationPermissions.Masters.Manage,
            L("Permission:Masters.Manage"));
    }
}
