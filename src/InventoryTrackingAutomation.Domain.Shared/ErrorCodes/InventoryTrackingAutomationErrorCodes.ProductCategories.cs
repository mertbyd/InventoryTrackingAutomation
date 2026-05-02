namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class ProductCategories
    {
        public const string NotFound = Prefix + ":ProductCategory.NotFound";
        public const string AlreadyExists = Prefix + ":ProductCategory.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":ProductCategory.CodeNotUnique";
    }
}
