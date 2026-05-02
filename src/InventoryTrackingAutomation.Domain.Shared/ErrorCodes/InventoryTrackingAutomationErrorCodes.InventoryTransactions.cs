namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class InventoryTransactions
    {
        public const string NotFound = Prefix + ":InventoryTransaction.NotFound";
        public const string InvalidTransfer = Prefix + ":InventoryTransaction.InvalidTransfer";
        public const string QuantityMustBePositive = Prefix + ":InventoryTransaction.QuantityMustBePositive";
        public const string InvalidLocationPair = Prefix + ":InventoryTransaction.InvalidLocationPair";
    }
}
