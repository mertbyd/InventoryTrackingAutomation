namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class MovementRequests
    {
        public const string NotFound = Prefix + ":MovementRequest.NotFound";
        public const string InvalidStatus = Prefix + ":MovementRequest.InvalidStatus";
        public const string RequestNumberNotUnique = Prefix + ":MovementRequest.RequestNumberNotUnique";
        public const string TargetRequired = Prefix + ":MovementRequest.TargetRequired";
        public const string VehicleRequired = Prefix + ":MovementRequest.VehicleRequired";
        public const string InvalidStateTransition = Prefix + ":MovementRequest.InvalidStateTransition";
        public const string DispatchNotAllowed = Prefix + ":MovementRequest.DispatchNotAllowed";
        public const string ReceiveNotAllowed = Prefix + ":MovementRequest.ReceiveNotAllowed";
        public const string ReturnReceiveLineRequired = Prefix + ":MovementRequest.ReturnReceiveLineRequired";
        public const string QuantityMismatch = Prefix + ":MovementRequest.QuantityMismatch";
    }
}
