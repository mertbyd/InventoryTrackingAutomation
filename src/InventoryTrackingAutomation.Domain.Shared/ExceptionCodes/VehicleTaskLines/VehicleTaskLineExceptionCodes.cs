namespace InventoryTrackingAutomation.ExceptionCodes;

public static class VehicleTaskLineExceptionCodes
{
    private const string VehicleTaskLineErrorCodesPrefix = $"TaskManagement.VehicleTaskLine";
    public const string NotFound = $"{VehicleTaskLineErrorCodesPrefix}:00001";
    public const string AlreadyExists = $"{VehicleTaskLineErrorCodesPrefix}:00002";
    public const string InsufficientTaskLineRemaining = $"{VehicleTaskLineErrorCodesPrefix}:00003";
    public const string CannotDeleteReceived = $"{VehicleTaskLineErrorCodesPrefix}:00004";
    public const string QuantityMismatch = $"{VehicleTaskLineErrorCodesPrefix}:00005";
}
