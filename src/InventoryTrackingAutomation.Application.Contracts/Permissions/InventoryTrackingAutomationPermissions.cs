using Volo.Abp.Reflection;

namespace InventoryTrackingAutomation.Permissions;

public class InventoryTrackingAutomationPermissions
{
    public const string GroupName = "InventoryTrackingAutomation";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(InventoryTrackingAutomationPermissions));
    }
}
