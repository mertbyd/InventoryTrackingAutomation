namespace InventoryTrackingAutomation.ExceptionCodes;

public static class VehicleTaskExceptionCodes
{
    private const string VehicleTaskErrorCodesPrefix = $"TaskManagement.VehicleTask";
    public const string NotFound = $"{VehicleTaskErrorCodesPrefix}:00001";
    public const string VehicleAlreadyAssigned = $"{VehicleTaskErrorCodesPrefix}:00002";
    public const string CannotDeleteWithLines = $"{VehicleTaskErrorCodesPrefix}:00003";
}
