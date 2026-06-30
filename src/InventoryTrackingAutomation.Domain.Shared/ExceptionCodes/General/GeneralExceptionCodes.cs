namespace InventoryTrackingAutomation.ExceptionCodes;

public static class GeneralExceptionCodes
{
    private const string GeneralErrorCodesPrefix = $"GeneralManagement.General";
    public const string InvalidOperation = $"{GeneralErrorCodesPrefix}:00001";
    public const string NotAuthorized = $"{GeneralErrorCodesPrefix}:00002";
    public const string InvalidEnumValue = $"{GeneralErrorCodesPrefix}:00003";
    public const string SoftDeleteNotSupported = $"{GeneralErrorCodesPrefix}:00004";
}
