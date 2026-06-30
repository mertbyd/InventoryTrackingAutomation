namespace InventoryTrackingAutomation.ExceptionCodes;

public static class WorkerExceptionCodes
{
    private const string WorkerErrorCodesPrefix = $"MasterManagement.Worker";
    public const string NotFound = $"{WorkerErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{WorkerErrorCodesPrefix}:00002";

    public static class ValidationExceptions
    {
        private const string WorkerValidationExceptionsPrefix = $"{WorkerErrorCodesPrefix}.ValidationExceptions";

        public static class RegistrationNumber
        {
            public const string MaxLength = $"{WorkerValidationExceptionsPrefix}.RegistrationNumber:00001";
        }

        public static class WorkerType
        {
            public const string CannotEmpty = $"{WorkerValidationExceptionsPrefix}.WorkerType:00001";
        }
    }

    public const string SelfAssignmentNotAllowed = $"{WorkerErrorCodesPrefix}:00003";
}
