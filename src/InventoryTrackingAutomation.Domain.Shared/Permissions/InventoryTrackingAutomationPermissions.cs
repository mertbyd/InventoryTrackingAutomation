using Volo.Abp.Reflection;

namespace InventoryTrackingAutomation.Permissions;

/// <summary>
/// Uygulama permission sabitlerini merkezi olarak tutar.
/// </summary>
public partial class InventoryTrackingAutomationPermissions
{
    public const string GroupName = "InventoryTrackingAutomation";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(InventoryTrackingAutomationPermissions));
    }
}
