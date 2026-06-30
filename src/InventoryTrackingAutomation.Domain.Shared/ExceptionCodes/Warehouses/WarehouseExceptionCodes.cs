namespace InventoryTrackingAutomation.ExceptionCodes;

public static class WarehouseExceptionCodes
{
    private const string WarehouseErrorCodesPrefix = $"MasterManagement.Warehouse";
    public const string NotFound = $"{WarehouseErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{WarehouseErrorCodesPrefix}:00002";
    public const string CodeNotUnique = $"{WarehouseErrorCodesPrefix}:00003";
}
