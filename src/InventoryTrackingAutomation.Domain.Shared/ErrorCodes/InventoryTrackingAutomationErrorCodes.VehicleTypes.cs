namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class VehicleTypes
    {
        public const string NotFound = Prefix + ":VehicleType.NotFound";
        public const string AlreadyExists = Prefix + ":VehicleType.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":VehicleType.CodeNotUnique";
    }
}
