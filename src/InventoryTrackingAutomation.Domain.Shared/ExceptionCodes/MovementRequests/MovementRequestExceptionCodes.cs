namespace InventoryTrackingAutomation.ExceptionCodes;

public static class MovementRequestExceptionCodes
{
    private const string MovementRequestErrorCodesPrefix = $"MovementManagement.MovementRequest";
    public const string NotFound = $"{MovementRequestErrorCodesPrefix}:00001";
    public const string InvalidStatus = $"{MovementRequestErrorCodesPrefix}:00002";
    public const string RequestNumberNotUnique = $"{MovementRequestErrorCodesPrefix}:00003";
    public const string TargetRequired = $"{MovementRequestErrorCodesPrefix}:00004";
    public const string VehicleRequired = $"{MovementRequestErrorCodesPrefix}:00005";
    public const string InvalidStateTransition = $"{MovementRequestErrorCodesPrefix}:00006";
    public const string DispatchNotAllowed = $"{MovementRequestErrorCodesPrefix}:00007";
    public const string ReceiveNotAllowed = $"{MovementRequestErrorCodesPrefix}:00008";
    public const string ReturnReceiveLineRequired = $"{MovementRequestErrorCodesPrefix}:00009";
    public const string QuantityMismatch = $"{MovementRequestErrorCodesPrefix}:00010";

    public static class ValidationExceptions
    {
        private const string MovementRequestValidationExceptionsPrefix = $"{MovementRequestErrorCodesPrefix}.ValidationExceptions";

        public static class VehicleTaskId
        {
            public const string CannotEmpty = $"{MovementRequestValidationExceptionsPrefix}.VehicleTaskId:00001";
        }
    }
}
