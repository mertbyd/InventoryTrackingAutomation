namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Warehouses
    {
        public const string NotFound = Prefix + ":Warehouse.NotFound";
        public const string AlreadyExists = Prefix + ":Warehouse.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Warehouse.CodeNotUnique";
    }
}
