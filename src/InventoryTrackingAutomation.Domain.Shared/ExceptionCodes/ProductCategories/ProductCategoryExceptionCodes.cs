namespace InventoryTrackingAutomation.ExceptionCodes.ProductCategories;

public static class ProductCategoryExceptionCodes
{
    private const string ProductCategoryErrorCodesPrefix = $"LookupManagement.ProductCategory";
    public const string NotFound = $"{ProductCategoryErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{ProductCategoryErrorCodesPrefix}:00002";
    
    public static class ValidationExceptions
    {
        private const string ProductCategoryValidationExceptionsPrefix = $"{ProductCategoryErrorCodesPrefix}.ValidationExceptions";
        
        public static class Code
        {
            public const string CannotEmpty = $"{ProductCategoryValidationExceptionsPrefix}.Code:00001";
            public const string MaxLength = $"{ProductCategoryValidationExceptionsPrefix}.Code:00002";
        }

        public static class Name
        {
            public const string CannotEmpty = $"{ProductCategoryValidationExceptionsPrefix}.Name:00001";
            public const string MaxLength = $"{ProductCategoryValidationExceptionsPrefix}.Name:00002";
        }

        public static class Description
        {
            public const string MaxLength = $"{ProductCategoryValidationExceptionsPrefix}.Description:00001";
        }
    }
}
