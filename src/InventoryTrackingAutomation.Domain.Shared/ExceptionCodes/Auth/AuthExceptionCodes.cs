namespace InventoryTrackingAutomation.ExceptionCodes;

public static class AuthExceptionCodes
{
    private const string AuthErrorCodesPrefix = $"AuthManagement.Auth";
    public const string UserNameAlreadyExists = $"{AuthErrorCodesPrefix}:00001";
    public const string EmailAlreadyExists = $"{AuthErrorCodesPrefix}:00002";
    public const string InvalidCredentials = $"{AuthErrorCodesPrefix}:00003";
    public const string PasswordMismatch = $"{AuthErrorCodesPrefix}:00004";
    public const string UserCreationFailed = $"{AuthErrorCodesPrefix}:00005";
    public const string RoleNotFound = $"{AuthErrorCodesPrefix}:00006";
    public const string RoleAssignmentFailed = $"{AuthErrorCodesPrefix}:00007";
    public const string TokenRequestFailed = $"{AuthErrorCodesPrefix}:00008";
    public const string MissingConfiguration = $"{AuthErrorCodesPrefix}:00009";
}
