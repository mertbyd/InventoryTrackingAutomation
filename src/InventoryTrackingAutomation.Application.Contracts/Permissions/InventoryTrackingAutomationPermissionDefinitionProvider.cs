using InventoryTrackingAutomation.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
namespace InventoryTrackingAutomation.Permissions;
public partial class InventoryTrackingAutomationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(
            InventoryTrackingAutomationPermissions.GroupName,
            L("Permission:InventoryTrackingAutomation"));

        AddMovementRequestsPermissions(myGroup);
        AddWorkflowsPermissions(myGroup);
        AddInventoryPermissions(myGroup);
        AddTasksPermissions(myGroup);
        AddTaskLinesPermissions(myGroup);
        AddVehicleTasksPermissions(myGroup);
        AddVehicleTaskLinesPermissions(myGroup);
        AddMastersPermissions(myGroup);
    }
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InventoryTrackingAutomationResource>(name);
    }
}
