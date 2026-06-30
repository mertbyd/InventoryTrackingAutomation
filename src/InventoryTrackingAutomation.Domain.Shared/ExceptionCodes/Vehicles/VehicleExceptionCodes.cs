namespace InventoryTrackingAutomation.ExceptionCodes;

public static class VehicleExceptionCodes
{
    private const string VehicleErrorCodesPrefix = $"MasterManagement.Vehicle";
    public const string NotFound = $"{VehicleErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{VehicleErrorCodesPrefix}:00002";

    public static class ValidationExceptions
    {
        private const string VehicleValidationExceptionsPrefix = $"{VehicleErrorCodesPrefix}.ValidationExceptions";

        public static class PlateNumber
        {
            public const string CannotEmpty = $"{VehicleValidationExceptionsPrefix}.PlateNumber:00001";
            public const string MaxLength = $"{VehicleValidationExceptionsPrefix}.PlateNumber:00002";
        }

        public static class VehicleType
        {
            public const string CannotEmpty = $"{VehicleValidationExceptionsPrefix}.VehicleType:00001";
        }
    }

    public const string PlateNumberNotUnique = $"{VehicleErrorCodesPrefix}:00003";
}
