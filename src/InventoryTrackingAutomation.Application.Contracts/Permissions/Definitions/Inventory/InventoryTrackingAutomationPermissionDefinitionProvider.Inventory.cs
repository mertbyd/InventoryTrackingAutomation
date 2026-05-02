using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddInventoryPermissions(PermissionGroupDefinition myGroup)
    {
        var inventoryPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Inventory.Default,
            L("Permission:Inventory"));

        inventoryPermission.AddChild(
            InventoryTrackingAutomationPermissions.Inventory.View,
            L("Permission:Inventory.View"));

        inventoryPermission.AddChild(
            InventoryTrackingAutomationPermissions.Inventory.Manage,
            L("Permission:Inventory.Manage"));
    }
}
