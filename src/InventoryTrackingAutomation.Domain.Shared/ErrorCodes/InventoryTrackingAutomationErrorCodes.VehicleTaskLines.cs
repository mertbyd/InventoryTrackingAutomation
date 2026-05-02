namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class VehicleTaskLines
    {
        public const string NotFound = Prefix + ":VehicleTaskLine.NotFound";
        public const string AlreadyExists = Prefix + ":VehicleTaskLine.AlreadyExists";
        public const string InsufficientTaskLineRemaining = Prefix + ":VehicleTaskLine.InsufficientTaskLineRemaining";
        public const string CannotDeleteReceived = Prefix + ":VehicleTaskLine.CannotDeleteReceived";
        public const string QuantityMismatch = Prefix + ":VehicleTaskLine.QuantityMismatch";
    }
}
