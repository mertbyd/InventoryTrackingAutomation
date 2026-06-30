namespace InventoryTrackingAutomation.ExceptionCodes;

public static class TaskLineExceptionCodes
{
    private const string TaskLineErrorCodesPrefix = $"TaskManagement.TaskLine";
    public const string NotFound = $"{TaskLineErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{TaskLineErrorCodesPrefix}:00002";
    public const string InsufficientRemaining = $"{TaskLineErrorCodesPrefix}:00003";
    public const string CannotDeleteAllocated = $"{TaskLineErrorCodesPrefix}:00004";
    public const string CannotChangeProductAllocated = $"{TaskLineErrorCodesPrefix}:00005";
    public const string QuantityBelowAllocated = $"{TaskLineErrorCodesPrefix}:00006";
}
