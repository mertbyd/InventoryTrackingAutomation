namespace InventoryTrackingAutomation.ExceptionCodes;

public static class InventoryTransactionExceptionCodes
{
    private const string InventoryTransactionErrorCodesPrefix = $"InventoryManagement.InventoryTransaction";
    public const string NotFound = $"{InventoryTransactionErrorCodesPrefix}:00001";
    public const string InvalidTransfer = $"{InventoryTransactionErrorCodesPrefix}:00002";
    public const string QuantityMustBePositive = $"{InventoryTransactionErrorCodesPrefix}:00003";
    public const string InvalidLocationPair = $"{InventoryTransactionErrorCodesPrefix}:00004";
    public const string ImmutableLedger = $"{InventoryTransactionErrorCodesPrefix}:00005";
}
