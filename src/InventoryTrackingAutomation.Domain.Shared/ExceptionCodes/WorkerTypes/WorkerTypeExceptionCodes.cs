namespace InventoryTrackingAutomation.ExceptionCodes;

public static class WorkerTypeExceptionCodes
{
    private const string WorkerTypeErrorCodesPrefix = $"LookupManagement.WorkerType";
    public const string NotFound = $"{WorkerTypeErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{WorkerTypeErrorCodesPrefix}:00002";
    
    public static class ValidationExceptions
    {
        private const string WorkerTypeValidationExceptionsPrefix = $"{WorkerTypeErrorCodesPrefix}.ValidationExceptions";
        
        public static class Code
        {
            public const string CannotEmpty = $"{WorkerTypeValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{WorkerTypeValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{WorkerTypeValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{WorkerTypeValidationExceptionsPrefix}.Name:00002";
        }

        public static class Description
        {
            public const string MaxLength = $"{WorkerTypeValidationExceptionsPrefix}.Description:00001";
        }
    }
}
