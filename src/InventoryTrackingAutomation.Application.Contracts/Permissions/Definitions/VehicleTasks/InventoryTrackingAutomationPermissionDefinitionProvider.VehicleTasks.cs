using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddVehicleTasksPermissions(PermissionGroupDefinition myGroup)
    {
        var vehicleTasksPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.VehicleTasks.Default,
            L("Permission:VehicleTasks"));

        vehicleTasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTasks.View,
            L("Permission:VehicleTasks.View"));

        vehicleTasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTasks.Manage,
            L("Permission:VehicleTasks.Manage"));
    }
}
