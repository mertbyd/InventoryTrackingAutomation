namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Vehicles
    {
        public const string NotFound = Prefix + ":Vehicle.NotFound";
        public const string AlreadyExists = Prefix + ":Vehicle.AlreadyExists";
        public const string PlateNumberNotUnique = Prefix + ":Vehicle.PlateNumberNotUnique";
    }
}
