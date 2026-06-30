namespace InventoryTrackingAutomation.ExceptionCodes;

public static class DepartmentExceptionCodes
{
    private const string DepartmentErrorCodesPrefix = $"LookupManagement.Department";
    public const string NotFound = $"{DepartmentErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{DepartmentErrorCodesPrefix}:00002";
    
    public static class ValidationExceptions
    {
        private const string DepartmentValidationExceptionsPrefix = $"{DepartmentErrorCodesPrefix}.ValidationExceptions";
        
        public static class Code
        {
            public const string CannotEmpty = $"{DepartmentValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{DepartmentValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{DepartmentValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{DepartmentValidationExceptionsPrefix}.Name:00002";
        }

        public static class Description
        {
            public const string MaxLength = $"{DepartmentValidationExceptionsPrefix}.Description:00001";
        }
    }
}
