namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class WorkerTypes
    {
        public const string NotFound = Prefix + ":WorkerType.NotFound";
        public const string AlreadyExists = Prefix + ":WorkerType.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":WorkerType.CodeNotUnique";
    }
}
