namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class TaskLines
    {
        public const string NotFound = Prefix + ":TaskLine.NotFound";
        public const string AlreadyExists = Prefix + ":TaskLine.AlreadyExists";
        public const string InsufficientRemaining = Prefix + ":TaskLine.InsufficientRemaining";
        public const string CannotDeleteAllocated = Prefix + ":TaskLine.CannotDeleteAllocated";
        public const string CannotChangeProductAllocated = Prefix + ":TaskLine.CannotChangeProductAllocated";
        public const string QuantityBelowAllocated = Prefix + ":TaskLine.QuantityBelowAllocated";
    }
}
