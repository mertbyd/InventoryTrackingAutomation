namespace InventoryTrackingAutomation.ExceptionCodes;

public static class VehicleTypeExceptionCodes
{
    private const string VehicleTypeErrorCodesPrefix = $"VehicleManagement.VehicleType";
    public const string NotFound = $"{VehicleTypeErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{VehicleTypeErrorCodesPrefix}:00002";
    
    public static class ValidationExceptions
    {
        private const string VehicleTypeValidationExceptionsPrefix = $"{VehicleTypeErrorCodesPrefix}.ValidationExceptions";
        
        public static class Code
        {
            public const string CannotEmpty = $"{VehicleTypeValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{VehicleTypeValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{VehicleTypeValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{VehicleTypeValidationExceptionsPrefix}.Name:00002";
        }

        public static class Description
        {
            public const string MaxLength = $"{VehicleTypeValidationExceptionsPrefix}.Description:00001";
        }
    }
}
