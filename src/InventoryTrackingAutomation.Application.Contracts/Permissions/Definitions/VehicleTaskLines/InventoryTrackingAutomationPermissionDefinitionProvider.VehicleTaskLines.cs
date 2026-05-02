using Volo.Abp.Authorization.Permissions;

namespace InventoryTrackingAutomation.Permissions;

public partial class InventoryTrackingAutomationPermissionDefinitionProvider
{
    private static void AddVehicleTaskLinesPermissions(PermissionGroupDefinition myGroup)
    {
        var vehicleTaskLinesPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.VehicleTaskLines.Default,
            L("Permission:VehicleTaskLines"));

        vehicleTaskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTaskLines.View,
            L("Permission:VehicleTaskLines.View"));

        vehicleTaskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTaskLines.Create,
            L("Permission:VehicleTaskLines.Create"));

        vehicleTaskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTaskLines.Edit,
            L("Permission:VehicleTaskLines.Edit"));

        vehicleTaskLinesPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTaskLines.Delete,
            L("Permission:VehicleTaskLines.Delete"));
    }
}
