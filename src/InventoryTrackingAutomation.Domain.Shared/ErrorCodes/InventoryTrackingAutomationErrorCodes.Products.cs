namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Products
    {
        public const string NotFound = Prefix + ":Product.NotFound";
        public const string AlreadyExists = Prefix + ":Product.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Product.CodeNotUnique";
    }
}
