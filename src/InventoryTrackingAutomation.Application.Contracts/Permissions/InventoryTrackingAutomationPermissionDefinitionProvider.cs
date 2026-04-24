using InventoryTrackingAutomation.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace InventoryTrackingAutomation.Permissions;

public class InventoryTrackingAutomationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(InventoryTrackingAutomationPermissions.GroupName, L("Permission:InventoryTrackingAutomation"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InventoryTrackingAutomationResource>(name);
    }
}
