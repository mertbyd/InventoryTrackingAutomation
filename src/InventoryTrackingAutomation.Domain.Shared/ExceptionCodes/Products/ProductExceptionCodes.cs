namespace InventoryTrackingAutomation.ExceptionCodes;

public static class ProductExceptionCodes
{
    private const string ProductErrorCodesPrefix = $"MasterManagement.Product";
    public const string NotFound = $"{ProductErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{ProductErrorCodesPrefix}:00002";
    public const string CodeNotUnique = $"{ProductErrorCodesPrefix}:00003";

    public static class ValidationExceptions
    {
        private const string ProductValidationExceptionsPrefix = $"{ProductErrorCodesPrefix}.ValidationExceptions";

        public static class Code
        {
            public const string CannotEmpty = $"{ProductValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{ProductValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{ProductValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{ProductValidationExceptionsPrefix}.Name:00002";
        }

        public static class UnitType
        {
            public const string CannotEmpty = $"{ProductValidationExceptionsPrefix}.UnitType:00001";
        }
    }
}
