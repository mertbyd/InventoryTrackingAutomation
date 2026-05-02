namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class StockLocations
    {
        public const string NotFound = Prefix + ":StockLocation.NotFound";
        public const string InvalidLocation = Prefix + ":StockLocation.InvalidLocation";
        public const string InsufficientStock = Prefix + ":StockLocation.InsufficientStock";
        public const string DuplicateLocation = Prefix + ":StockLocation.DuplicateLocation";
        public const string InvalidQuantity = Prefix + ":StockLocation.InvalidQuantity";
        public const string UnsupportedLocationType = Prefix + ":StockLocation.UnsupportedLocationType";
    }
}
