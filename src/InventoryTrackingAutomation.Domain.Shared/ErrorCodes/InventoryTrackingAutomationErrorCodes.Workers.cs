namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Workers
    {
        public const string NotFound = Prefix + ":Worker.NotFound";
        public const string AlreadyExists = Prefix + ":Worker.AlreadyExists";
        public const string SelfAssignmentNotAllowed = Prefix + ":Worker.SelfAssignmentNotAllowed";
    }
}
