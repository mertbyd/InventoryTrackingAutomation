namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Departments
    {
        public const string NotFound = Prefix + ":Department.NotFound";
        public const string AlreadyExists = Prefix + ":Department.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Department.CodeNotUnique";
    }
}
