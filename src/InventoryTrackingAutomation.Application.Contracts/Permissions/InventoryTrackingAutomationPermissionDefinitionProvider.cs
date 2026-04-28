using InventoryTrackingAutomation.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace InventoryTrackingAutomation.Permissions;

public class InventoryTrackingAutomationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(InventoryTrackingAutomationPermissions.GroupName, L("Permission:InventoryTrackingAutomation"));

        // Hareket Talepleri (Movement Requests)
        var movementRequestsPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.MovementRequests.Default,
            L("Permission:MovementRequests")
        );

        // Hareket talebi oluşturma
        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Create,
            L("Permission:MovementRequests.Create")
        );

        // Hareket talebini düzenleme
        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Edit,
            L("Permission:MovementRequests.Edit")
        );

        // Hareket talebini silme
        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.Delete,
            L("Permission:MovementRequests.Delete")
        );

        // Hareket talebini görüntüleme
        movementRequestsPermission.AddChild(
            InventoryTrackingAutomationPermissions.MovementRequests.View,
            L("Permission:MovementRequests.View")
        );

        // İş Akışları (Workflows)
        var workflowsPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Workflows.Default,
            L("Permission:Workflows")
        );

        // İş akışını onaylama
        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.Approve,
            L("Permission:Workflows.Approve")
        );

        // İş akışını reddetme
        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.Reject,
            L("Permission:Workflows.Reject")
        );

        // İş akışını görüntüleme
        workflowsPermission.AddChild(
            InventoryTrackingAutomationPermissions.Workflows.View,
            L("Permission:Workflows.View")
        );

        // Envanteri (Inventory)
        var inventoryPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Inventory.Default,
            L("Permission:Inventory")
        );

        // Envanteri görüntüleme
        inventoryPermission.AddChild(
            InventoryTrackingAutomationPermissions.Inventory.View,
            L("Permission:Inventory.View")
        );

        // Envanteri yönetme
        inventoryPermission.AddChild(
            InventoryTrackingAutomationPermissions.Inventory.Manage,
            L("Permission:Inventory.Manage")
        );

        // Gorevler (Tasks)
        var tasksPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Tasks.Default,
            L("Permission:Tasks")
        );

        // Gorevleri goruntuleme
        tasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.Tasks.View,
            L("Permission:Tasks.View")
        );

        // Gorevleri yonetme
        tasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.Tasks.Manage,
            L("Permission:Tasks.Manage")
        );

        // Arac-gorev atamalari (VehicleTasks)
        var vehicleTasksPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.VehicleTasks.Default,
            L("Permission:VehicleTasks")
        );

        // Arac-gorev atamalarini goruntuleme
        vehicleTasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTasks.View,
            L("Permission:VehicleTasks.View")
        );

        // Arac-gorev atamalarini yonetme
        vehicleTasksPermission.AddChild(
            InventoryTrackingAutomationPermissions.VehicleTasks.Manage,
            L("Permission:VehicleTasks.Manage")
        );

        // Masterler (Masters - Araç, Departman vb.)
        var mastersPermission = myGroup.AddPermission(
            InventoryTrackingAutomationPermissions.Masters.Default,
            L("Permission:Masters")
        );

        // Master verileri görüntüleme
        mastersPermission.AddChild(
            InventoryTrackingAutomationPermissions.Masters.View,
            L("Permission:Masters.View")
        );

        // Master verileri yönetme
        mastersPermission.AddChild(
            InventoryTrackingAutomationPermissions.Masters.Manage,
            L("Permission:Masters.Manage")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InventoryTrackingAutomationResource>(name);
    }
}
