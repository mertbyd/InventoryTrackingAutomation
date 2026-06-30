namespace InventoryTrackingAutomation.ExceptionCodes;

public static class UnitTypeExceptionCodes
{
    private const string UnitTypeErrorCodesPrefix = $"LookupManagement.UnitType";
    public const string NotFound = $"{UnitTypeErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{UnitTypeErrorCodesPrefix}:00002";
    
    public static class ValidationExceptions
    {
        private const string UnitTypeValidationExceptionsPrefix = $"{UnitTypeErrorCodesPrefix}.ValidationExceptions";
        
        public static class Code
        {
            public const string CannotEmpty = $"{UnitTypeValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{UnitTypeValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{UnitTypeValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{UnitTypeValidationExceptionsPrefix}.Name:00002";
        }

        public static class Description
        {
            public const string MaxLength = $"{UnitTypeValidationExceptionsPrefix}.Description:00001";
        }
    }
}
