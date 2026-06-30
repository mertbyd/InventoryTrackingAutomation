namespace InventoryTrackingAutomation.ExceptionCodes;

public static class InventoryTaskExceptionCodes
{
    private const string InventoryTaskErrorCodesPrefix = $"TaskManagement.InventoryTask";
    public const string NotFound = $"{InventoryTaskErrorCodesPrefix}:00001";
    public const string CodeNotUnique = $"{InventoryTaskErrorCodesPrefix}:00002";
    public const string CannotUpdateCompletedOrCancelled = $"{InventoryTaskErrorCodesPrefix}:00003";
}
