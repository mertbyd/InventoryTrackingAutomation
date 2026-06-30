namespace InventoryTrackingAutomation.ExceptionCodes;

public static class StockLocationExceptionCodes
{
    private const string StockLocationErrorCodesPrefix = $"InventoryManagement.StockLocation";
    public const string NotFound = $"{StockLocationErrorCodesPrefix}:00001";
    public const string InvalidLocation = $"{StockLocationErrorCodesPrefix}:00002";
    public const string InsufficientStock = $"{StockLocationErrorCodesPrefix}:00003";
    public const string DuplicateLocation = $"{StockLocationErrorCodesPrefix}:00004";
    public const string InvalidQuantity = $"{StockLocationErrorCodesPrefix}:00005";
    public const string UnsupportedLocationType = $"{StockLocationErrorCodesPrefix}:00006";
    public const string DeleteNotSupported = $"{StockLocationErrorCodesPrefix}:00007";
}
